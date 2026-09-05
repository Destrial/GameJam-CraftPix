using UnityEngine;
using System.Collections;


namespace Destrial
{
    public class Enemy : CellObject
    {
        //First choose which enemy it is (NOT YET IMPLEMENTED)
        public enum EnemyType
        {
            Rat,
            Goblin,
            BladeGoblin,
            ShamanGoblin
        }


        [SerializeField] private GameObject _deathPrefab;

//public Vector2Int CellPosition;

        private Animator _animator;
        private bool _isMoving;

        private Vector2Int _newCellTarget;
        private Vector2Int _newDirection;
        [SerializeField] float _moveSpeed = 1;
        BoardManager _board;
        BoardManager.CellData Cell;
        Vector3 _moveTarget;

        public int Health = 3;

        private int _currentHealth;


        private void Awake()
        {
            _animator = GetComponent<Animator>();

            GameManager.Instance.TurnManager.OnTick += TurnHappened; //EVENT
        }

        private void OnDestroy()
        {
            GameManager.Instance.TurnManager.OnTick -= TurnHappened;
        }

        void Update()
        {
            if (_isMoving)
            {
                transform.position = Vector3.MoveTowards(transform.position, _moveTarget, _moveSpeed * Time.deltaTime);

                if (transform.position == _moveTarget)
                {
                    _isMoving = false;
                 //   MyState = PlayerState.Idle;
                            
                    _animator.SetFloat("mov_x", _newDirection.x);
                    _animator.SetFloat("mov_y", _newDirection.y);
                    _animator.SetBool("Moving", false);
                 

                    //_cantInput = false;
                }

                return;
            }
        }

        public override void Init(Vector2Int coord)
        {
            base.Init(coord);
            _currentHealth = Health;
            _board = GameManager.Instance.BoardManager;
            Cell = _board.GetCellData(coord);
        }

        public override bool PlayerWantsToEnter()
        {
            _currentHealth -= 1;

            if (_currentHealth <= 0)
            {
                Instantiate(_deathPrefab, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }

            return false;
        }

        /*

        bool MoveTo(Vector2Int coord)
        {
            Debug.Log("LL:" + coord);
            var board = GameManager.Instance.BoardManager;
            var targetCell = board.GetCellData(coord);

            if (targetCell == null
                || !targetCell.Passable
                || targetCell.ContainedObject != null)
            {
                return false;
            }

            //remove enemy from current cell
            var currentCell = board.GetCellData(_cell);
            currentCell.ContainedObject = null;

            //add it to the next cell
            targetCell.ContainedObject = this;
            _cell = coord;
            transform.position = board.CellToWorld(coord);

            return true;
        }
        */
        public void GoMoveTo(Vector2Int cell, bool immediate)
        {
            _cell = cell;


            if (immediate)
            {
                _isMoving = false;
                transform.position = _board.CellToWorld(_cell);
            }
            else
            {
                _isMoving = true;
                _moveTarget = _board.CellToWorld(_cell);
            }

            Cell = _board.GetCellData(cell);

            _animator.SetFloat("mov_x", _newDirection.x);
            _animator.SetFloat("mov_y", _newDirection.y);
            _animator.SetBool("ContinuousWalk", true);
            _animator.SetBool("Moving", _isMoving);
        }

        void TurnHappened()
        {
            //Public property that returns the player current cell
            var playerCell = GameManager.Instance.PlayerController.CellPosition;

            int xDist = playerCell.x - _cell.x;
            int yDist = playerCell.y - _cell.y;

            int absXDist = Mathf.Abs(xDist);
            int absYDist = Mathf.Abs(yDist);


            if ((xDist == 0 && absYDist == 1)
                || (yDist == 0 && absXDist == 1))
            {
                //Enemy is adjacent to the player, attacks.

                //// /!\ MUST ADD CODE SO Enemy DOESN'T ATTACK IF PLAYER HIT HIM FIRST
                //
                _animator.SetTrigger("Attack");
                GameManager.Instance.ChangeFood(-3);
            }

            else
            {
               _newCellTarget = _board.FindMove(_cell,  playerCell);

                if (_newCellTarget != Vector2Int.zero)
                {
                    _newDirection.x= _newCellTarget.x - _cell.x;
                    _newDirection.y = _newCellTarget.y - _cell.y;
                    GoMoveTo(_newCellTarget, false);
                }
            }
        }
    }
}