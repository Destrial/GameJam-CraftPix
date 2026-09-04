using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Serialization;

namespace Destrial
{
    public class PlayerController : MonoBehaviour
    {
        DestrialInputs _myInputs;
        private BoardManager _board;

        public Vector2Int CellPosition;
        private Animator _animator;
        private bool _isGameOver;

        private bool _isMoving;
        private Vector3 _moveTarget;
        [SerializeField] float _moveSpeed = 1;
        public BoardManager.CellData Cell;
        private bool _isAttacking;
        [SerializeField] float _attackSpeed = 0.4f;
      
        public enum PlayerState { Idle, Moving , Attacking , Throwing, Grabbing, Sleeping, Stun}

        public PlayerState MyState;
        public PlayerState MyAction;

        private Vector2Int _newCellTarget;
        private Vector2Int _newDirection;

       private bool _cantInput;
      //  [SerializeField]
      //  private float _waitInputTime;
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _myInputs = new DestrialInputs();
        }

        private void OnEnable()
        {
            _myInputs.Enable();
        }


        private void OnDisable()
        {
            _myInputs.Disable();
        }

        public void Spawn(BoardManager boardManager, Vector2Int cell)
        {
            _board = boardManager;
            CellPosition = cell;

            //let's move to the right position...
            transform.position = _board.CellToWorld(cell);
            Cell = _board.GetCellData(cell);
        }

        public void MoveTo(Vector2Int cell, bool immediate)
        {
          //  Debug.Log("Actual Moving to " + cell);
            CellPosition = cell;

            if (immediate)
            {
                _isMoving = false;
                transform.position = _board.CellToWorld(CellPosition);
            }
            else
            {
                _isMoving = true;
                _moveTarget = _board.CellToWorld(CellPosition);
            }

            Cell = _board.GetCellData(cell);

            _animator.SetFloat("mov_x", _newDirection.x);
            _animator.SetFloat("mov_y", _newDirection.y);
            _animator.SetBool("ContinuousWalk", true);
            _animator.SetBool("Moving", _isMoving);
        }

        public void Init()
        {
            _animator.SetFloat("mov_x", 0);
            _animator.SetFloat("mov_y", 0);
            _animator.SetBool("Moving", false);

            _isGameOver = false;
            _isMoving = false;
            MyState = PlayerState.Idle;
            //_cantInput=false;
        }


        public void Update()
        {
            if (_isGameOver)
            {
                if (Keyboard.current.enterKey.wasPressedThisFrame)
                {
                    GameManager.Instance.StartNewGame();
                }

                return;
            }
            if (_cantInput) return;
            
            bool hasMoved = false;
            Vector2 moveInput = _myInputs.Player.Move.ReadValue<Vector2>();
            InputAction moveInputAction = _myInputs.Player.Move;

            _newCellTarget = CellPosition;
            
            
            if (MyState==PlayerState.Idle) //Only new input if idle
            {

               
              
                if (moveInputAction.IsPressed()) //Test if the player is pressing the move button
                {
                    if (moveInput.y > 0)
                    {
                        _newCellTarget.y += 1;
                        _newDirection = new Vector2Int(0, 1);
                        hasMoved = true;
                    }
                    else if (moveInput.y < 0)
                    {
                        _newCellTarget.y -= 1;
                        _newDirection = new Vector2Int(0, -1);
                        hasMoved = true;
                    }
                    else if (moveInput.x > 0)
                    {
                        _newCellTarget.x += 1;
                        _newDirection = new Vector2Int(1, 0);
                        hasMoved = true;
                    }
                    else if (moveInput.x < 0)
                    {
                        _newCellTarget.x -= 1;
                        _newDirection = new Vector2Int(-1, 0);
                        hasMoved = true;
                    }
                }
            }

            
            
           
            switch (MyState)
            {   
                case PlayerState.Idle:
                    if (hasMoved & !_isMoving & !_isAttacking)
                    {
                     //   Debug.Log("Want Moving to " + newCellTarget);
                        //check if the new position is passable, then move there if it is.
                        BoardManager.CellData cellData = _board.GetCellData(_newCellTarget);

                        if (cellData != null && cellData.Passable)
                        {
                          

                            if (cellData.ContainedObject == null)
                            {
                                MoveTo(_newCellTarget, false); 
                                MyState = PlayerState.Moving;
                                GameManager.Instance.TurnManager.Tick();
                            }
                            else if (cellData.ContainedObject.PlayerWantsToEnter()) // test can pass grab , enemy ,wall
                            {
                                MoveTo(_newCellTarget, false);  //tick
                                MyState = PlayerState.Moving;
                                //Call PlayerEntered AFTER moving the player! Otherwise not in cell yet
                                cellData.ContainedObject.PlayerEntered();  // only for grab
                                GameManager.Instance.TurnManager.Tick();
                                
                            }
                            else
                            {
                                MyState = PlayerState.Attacking; //wall or enemy
                                _isAttacking = true;
                                _newCellTarget=CellPosition;
                               // MoveTo(CellPosition, true); //stay in place
                            }
                        }
                        else  if (cellData != null && !cellData.Passable)
                        
                        {
                           // _cantInput = false; //hit a wall
                           _animator.SetBool("ContinuousWalk", false);
                        }
                    }
                    
                    break;
                
                case PlayerState.Moving:
                    if (_isMoving)
                    {
                        transform.position = Vector3.MoveTowards(transform.position, _moveTarget, _moveSpeed * Time.deltaTime);

                        if (transform.position == _moveTarget)
                        {
                            _isMoving = false;
                            MyState = PlayerState.Idle;
                            
                            _animator.SetFloat("mov_x", _newDirection.x);
                            _animator.SetFloat("mov_y", _newDirection.y);
                            _animator.SetBool("Moving", false);
                            if (!_myInputs.Player.Move.IsPressed())
                            {
                                _animator.SetBool("ContinuousWalk", false);
                            }

                            var cellData = _board.GetCellData(CellPosition);
                            if (cellData.ContainedObject != null)
                                cellData.ContainedObject.PlayerEntered();
                            //_cantInput = false;
                        }

                        return;
                    }
                    break;
                
                case PlayerState.Attacking:
                    if (_isAttacking)
                    {
                      //wait
                      _cantInput = true;
                      _isMoving = false;
                      GameManager.Instance.TurnManager.Tick();
                      StartCoroutine(StartTimerAttack());
                      
                      _animator.SetFloat("mov_x", _newDirection.x);
                      _animator.SetFloat("mov_y", _newDirection.y);
                      _animator.SetBool("ContinuousWalk", false);
                      _animator.SetTrigger("Attack");
                     
                    }
                    break;
                
            }
           
          

          
        }
        
       IEnumerator StartTimerAttack()
        {
               
                yield return new WaitForSeconds(_attackSpeed);
               _isAttacking = false;
               
               _isMoving = false;
               MyState = PlayerState.Idle;
               _cantInput = false;
    
        }

        

        public void GameOver()
        {
            _isGameOver = true;
        }
    }
}