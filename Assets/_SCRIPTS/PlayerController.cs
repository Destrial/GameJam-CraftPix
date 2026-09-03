using UnityEngine;
using UnityEngine.InputSystem;
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
            _animator.SetBool("Moving", _isMoving);
        }

        public void Init()
        {
            _isMoving = false;
            _animator.SetBool("Moving", false);
            _isGameOver = false;

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
            if (_isMoving)
            {
                transform.position = Vector3.MoveTowards(transform.position, _moveTarget, _moveSpeed * Time.deltaTime);

                if (transform.position == _moveTarget)
                {
                    _isMoving = false;
                    _animator.SetBool("Moving", false);
                    var cellData = _board.GetCellData(CellPosition);
                    if (cellData.ContainedObject != null)
                        cellData.ContainedObject.PlayerEntered();
                }

                return;
            }
            
            Vector2 moveInput = _myInputs.Player.Move.ReadValue<Vector2>();
            
            Vector2Int newCellTarget = CellPosition;
            bool hasMoved = false;

            if (moveInput.y>0)
            {
                newCellTarget.y += 1;
                hasMoved = true;
            }
            else if (moveInput.y<0)
            {
                newCellTarget.y -= 1;
                hasMoved = true;
            }
            else if (moveInput.x>0)
            {
                newCellTarget.x += 1;
                hasMoved = true;
            }
            else if (moveInput.x<0)
            {
                newCellTarget.x -= 1;
                hasMoved = true;
            }

            if (hasMoved)
            {
                Debug.Log("Moving to " + newCellTarget);
                //check if the new position is passable, then move there if it is.
                BoardManager.CellData cellData = _board.GetCellData(newCellTarget);

                if (cellData != null && cellData.Passable)
                {
                    GameManager.Instance.TurnManager.Tick();

                    if (cellData.ContainedObject == null)
                    {
                        MoveTo(newCellTarget, false);
                    }
                    else if (cellData.ContainedObject.PlayerWantsToEnter())
                    {
                        MoveTo(newCellTarget, false);
                        //Call PlayerEntered AFTER moving the player! Otherwise not in cell yet
                        cellData.ContainedObject.PlayerEntered();
                    }
                }
            }

           
        }

        public void GameOver()
        {
            _isGameOver = true;
        }
    }
}