using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.Serialization;
using DG.Tweening;

namespace Destrial
{
    public class PlayerController : MonoBehaviour
    {
        DestrialInputs _myInputs;
        InputAction _lookInputAction;
        InputAction _moveInputAction;
       // InputAction _attackInputAction;
        private BoardManager _board;
        [SerializeField] GameObject _DirShow;
        public Vector2Int CellPosition;
        private Animator _animator;
        private bool _isGameOver;

        private bool _isMoving;
        private Vector3 _moveTarget;
        [SerializeField] float _moveSpeed = 1;
        public BoardManager.CellData Cell;
        private bool _isAttacking;
        [SerializeField] float _attackSpeed = 0.4f;
        private SpriteRenderer _spriteRenderer;
        public enum PlayerState
        {
            Idle,
            Moving,
            Attacking,
            Throwing,
            Grabbing,
            Sleeping,
            Stun,
            Death,
            Wait 
            
        }

        public PlayerState MyState;
        public PlayerState MyAction;

        private Vector2Int _newCellTarget;
        private Vector2Int _newDirection;
        
        //private bool _cantInput;
        public bool CantNewInput;
        [SerializeField] private GameObject _bloodPrefab;
        [SerializeField] AudioSource _audioSource;
        [SerializeField] AudioClip[] _audioMove;
        [SerializeField] AudioClip[] _audioAttack;
        [SerializeField] AudioClip[] _audioHurt;
        [SerializeField] AudioClip _audioDie;

        //  [SerializeField]
        //  private float _waitInputTime;
        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _audioSource = GetComponent<AudioSource>();
            _myInputs = new DestrialInputs();
            _spriteRenderer= GetComponent<SpriteRenderer>(); 
            _moveInputAction = _myInputs.Player.Move;
           // _attackInputAction = _myInputs.Player.Attack;
            _lookInputAction = _myInputs.Player.Jump;
            
        }

        private void OnEnable()
        {
            _myInputs.Enable();
            _myInputs.Player.Attack.performed += OnAttackPerformed;
            _myInputs.Player.Jump.performed += OnLookPerformed;
            _myInputs.Player.Jump.canceled += OnStopLook;
         
            CantNewInput = false;
        }


        private void OnDisable()
        {
            _myInputs.Disable();
            _myInputs.Player.Attack.performed -= OnAttackPerformed;
            _myInputs.Player.Jump.performed -= OnLookPerformed;
            _myInputs.Player.Jump.canceled -= OnStopLook;
        }

      

        public void GameOver()
        {
            _animator.SetBool("Death", true);
            _spriteRenderer.color = new Color(0.345f,0.345f,0.345f); // gris
            _isGameOver = true;
            MyState = PlayerState.Death;
            _audioSource.PlayOneShot(_audioDie, GameManager.Instance.sfxVolume);
            Debug.Log("GAME OVER player: "+_board.Player.MyState);
        }
        
        public void GetHurt(int amount,Vector2Int modDir)
        {
            _animator.SetTrigger("Hurt");
            _animator.SetFloat("mov_x", modDir.x);
            _animator.SetFloat("mov_y", modDir.y);
            _newDirection = modDir;
           // Debug.Log("modflash"+_spriteRenderer.gameObject.name);
            _board.FlashSprite(_spriteRenderer);
            GameManager.Instance.ChangeLife(-amount); //DAMAGE THE PLAYER
           // Debug.Log("hurt "+amount);
            Instantiate(_bloodPrefab, transform.position, Quaternion.identity);
            _audioSource.PlayOneShot(_audioHurt[Random.Range(0, _audioHurt.Length)], GameManager.Instance.sfxVolume);
        }

        private void OnAttackPerformed(InputAction.CallbackContext context)
        {
            if (MyState.Equals(PlayerState.Idle) && !_isAttacking && !_isMoving)
            {
                DirectAttack();
            }
        }
        
        private void OnLookPerformed(InputAction.CallbackContext context)
        {
            if (MyState.Equals(PlayerState.Idle) && !_isAttacking && !_isMoving)
            {
                ShowLook(_newDirection);
            }
        }
        private void OnStopLook(InputAction.CallbackContext context)
        {
            
            _DirShow.SetActive(false);
            
        }

        void ShowLook(Vector2Int dir)
        {
            _DirShow.SetActive(true);
            _DirShow.transform.position = _board.CellToWorld(CellPosition + dir);
           
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
            _audioSource.PlayOneShot(_audioMove[Random.Range(0, _audioMove.Length)], GameManager.Instance.sfxVolume);
        }


        public void Init()
        {
            _animator.SetFloat("mov_x", 0);
            _animator.SetFloat("mov_y", 1);
            _newDirection = new Vector2Int(0, 1);
            _animator.SetBool("Death", false);
            _spriteRenderer.color = Color.white;
            _animator.SetBool("Moving", false);
            _moveTarget = transform.position;
            _isGameOver = false;
            _isMoving = false;
            MyState = PlayerState.Wait;
            Invoke("StartIdle", 0.5f);
        //    _cantInput = true;
            transform.localScale = new Vector3(1, 1, 1);
         //   
         
        
        }

        void StartIdle()
        {
            if (MyState != PlayerState.Death)
            {
                MyState = PlayerState.Idle;
            }

          
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

            if (MyState == PlayerState.Wait)
            {
                return;
            }
            //  Debug.Log("1"+ MyState);
          //  if (_cantInput) return;
         
            bool hasMoved = false;
            Vector2 moveInput = _myInputs.Player.Move.ReadValue<Vector2>();

            _newCellTarget = CellPosition;


            if (MyState == PlayerState.Idle) //Only new input if idle
            {
               
                
                if (_moveInputAction.IsPressed()) //Test if the player is pressing the move button
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

                    if (_lookInputAction.IsPressed()) //Test if the player is pressing the move button
                    {
                        ShowLook(_newDirection);
                        _animator.SetFloat("mov_x", _newDirection.x);
                        _animator.SetFloat("mov_y", _newDirection.y);
                        hasMoved = false;
                    }
                }
            }


            // Debug.Log("2 "+MyState);

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
                              //  GameManager.Instance.TurnManager.Tick();
                            }
                            else if (cellData.ContainedObject.PlayerWantsToEnter()) // test can pass grab , enemy ,wall
                            {
                                MoveTo(_newCellTarget, false); //tick
                                MyState = PlayerState.Moving;
                                //Call PlayerEntered AFTER moving the player! Otherwise not in cell yet
                                cellData.ContainedObject.PlayerEntered(); // only for grab
                               // GameManager.Instance.TurnManager.Tick();
                             //   Debug.Log("Grabbing");
                            }
                            else
                            {
                                MyState = PlayerState.Attacking; //wall or enemy
                                _isAttacking = true;
                                _newCellTarget = CellPosition;
                                // MoveTo(CellPosition, true); //stay in place
                            }
                        }
                        else if (cellData != null && !cellData.Passable)

                        {
                            _animator.SetFloat("mov_x", _newDirection.x);
                            _animator.SetFloat("mov_y", _newDirection.y);
                            // _cantInput = false; //hit a wall
                            _animator.SetBool("ContinuousWalk", false);
                        }
                    }

                    break;

                case PlayerState.Moving:
                    if (_isMoving)
                    {
                        transform.position =
                            Vector3.MoveTowards(transform.position, _moveTarget, _moveSpeed * Time.deltaTime);

                        if (transform.position == _moveTarget)
                        {
                            _isMoving = false;
                            if (MyState != PlayerState.Death)
                            {
                                MyState = PlayerState.Idle;
                            }

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
                            GameManager.Instance.TurnManager.Tick();
                            //_cantInput = false;
                        }

                        return;
                    }

                    break;

                case PlayerState.Attacking:
                    // Debug.Log("3");
                    if (_isAttacking)
                    {
                        //   Debug.Log("4 aaaa");
                        //wait
                     //   _cantInput = true;
                        
                       
                        StartCoroutine(StartTimerAttack());

                        _animator.SetFloat("mov_x", _newDirection.x);
                        _animator.SetFloat("mov_y", _newDirection.y);
                        _animator.SetBool("ContinuousWalk", false);
                        _animator.SetTrigger("Attack");
                        GameManager.Instance.AttacksAmount++;
                    }

                    break;
            }
        }

        void DirectAttack()
        {
            MyAction = PlayerState.Attacking;
            MyState = PlayerState.Attacking;
            _isAttacking = true;
            _isMoving = false;
            _newCellTarget = CellPosition + _newDirection;
            BoardManager.CellData cellData = _board.GetCellData(_newCellTarget);
            if (cellData.ContainedObject != null)
            {
                if (cellData.ContainedObject.PlayerWantsToEnter())
                {
                    //IMPORTANT  /!\
                    // C est l enemy qui fait la logique de prendre des degats juste en testant ce booleen
                } //actual call on target damage 
            }
            // Debug.Log("Attacking");

          //  _cantInput = true;
            _isMoving = false;
            GameManager.Instance.TurnManager.Tick();
            StartCoroutine(StartTimerAttack());

            _animator.SetFloat("mov_x", _newDirection.x);
            _animator.SetFloat("mov_y", _newDirection.y);
            _animator.SetBool("ContinuousWalk", false);
            _animator.SetTrigger("Attack");
            GameManager.Instance.AttacksAmount++;
            
        }

        IEnumerator StartTimerAttack()
        {
         //   _board.PerformGridAttack(CellPosition,_newCellTarget);
            MyState = PlayerState.Wait;
            _audioSource.PlayOneShot(_audioAttack[Random.Range(0, _audioAttack.Length)], GameManager.Instance.sfxVolume);
           // Debug.Log("att");
            yield return new WaitForSeconds(_attackSpeed);
            _isAttacking = false;
            _isMoving = false;
           // MyState = 
           Debug.Log("EndAttack"+ MyState);
           if (MyState != PlayerState.Death)
           {
               GameManager.Instance.TurnManager.Tick();
           }

           //  _cantInput = false;
        }
    }
}