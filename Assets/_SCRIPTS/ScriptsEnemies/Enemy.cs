using UnityEngine;
using System.Collections;
using DG.Tweening;


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

        [SerializeField] private EnemyType _myEnemyType;
        [SerializeField] private GameObject _deathPrefab;
        [SerializeField] private GameObject _bloodPrefab;
        
       

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
        public int Damage = 5;
        private int _currentHealth;
        [SerializeField] AudioSource _audioSource;
        [SerializeField] AudioClip[] _audioMove;
        [SerializeField] AudioClip[] _audioAttack;
        [SerializeField] AudioClip[] _audioHurt;
      
        [SerializeField] AudioClip[] _audioImpact;
    [SerializeField] SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _audioSource = GetComponent<AudioSource>();

        //    GameManager.Instance.TurnManager.OnTick += TurnHappened; //EVENT
        }

        private void OnDestroy()
        {
          // GameManager.Instance.TurnManager.OnTick -= TurnHappened;
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
                    _animator.SetBool("ContinuousWalk", false);
                 

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
            GameManager.Instance.Enemies.Add(this);
        }

        public override bool PlayerWantsToEnter()   
        {
            _currentHealth -= 1;
            _animator.SetTrigger("Hurt");
            _audioSource.PlayOneShot(_audioImpact[Random.Range(0, _audioImpact.Length)],GameManager.Instance.sfxVolume);
            _audioSource.PlayOneShot(_audioHurt[Random.Range(0, _audioHurt.Length)],GameManager.Instance.sfxVolume);
            Instantiate(_bloodPrefab, transform.position, Quaternion.identity);

            
            if (_currentHealth <= 0)
            {
                _animator.SetTrigger("Die");
                GameManager.Instance.MobDeath(_myEnemyType);
                Instantiate(_deathPrefab, transform.position, Quaternion.identity);
                GameManager.Instance.Enemies.Remove(this);
                Destroy(gameObject);
                GameManager.Instance.TurnManager.MobDie();
               
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
            _board.FreeBoard(_cell,true);
        
           
            var currentCell = _board.GetCellData(_cell);
            
            
            var targetCell = _board.GetCellData(cell);
            if (targetCell.ContainedObject != null)
            {
                targetCell.ContainedObject.RatEntered();
            }
           
            //remove enemy from current cell
            currentCell.ContainedObject = null;
            //add it to the next cell
            targetCell.ContainedObject = this;
           
            
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
            _board.FreeBoard(_cell,false);
            _audioSource.PlayOneShot(_audioMove[Random.Range(0, _audioMove.Length)],GameManager.Instance.sfxVolume);
        }

        public bool CanAttack()
        {
            var playerCell = GameManager.Instance.PlayerController.CellPosition;

            int xDist = playerCell.x - _cell.x;
            int yDist = playerCell.y - _cell.y;

            int absXDist = Mathf.Abs(xDist);
            int absYDist = Mathf.Abs(yDist);

            if ((xDist == 0 && absYDist == 1)
                || (yDist == 0 && absXDist == 1))

            {
                return true;
            }
            else
            {
                return false;
            }
        }
        
        
        public void TurnHappenedAttack()
        {
          
                //Enemy is adjacent to the player, attacks.
               
                //// /!\ MUST ADD CODE SO Enemy DOESN'T ATTACK IF PLAYER HIT HIM FIRST
                //
                var playerCell = GameManager.Instance.PlayerController.CellPosition;
                _newDirection= playerCell - _cell;
                _board.PerformGridAttack(_cell,playerCell,this.transform, _spriteRenderer);
                _audioSource.PlayOneShot(_audioAttack[Random.Range(0, _audioAttack.Length)],GameManager.Instance.sfxVolume);
                _animator.SetFloat("mov_x", _newDirection.x);
                _animator.SetFloat("mov_y", _newDirection.y);
                _animator.SetTrigger("Attack");
                GameManager.Instance.ChangeLife(-Damage); //DAMAGE THE PLAYER
                GameManager.Instance.BoardManager.Player.GetHurt(-Damage); // CHANGE UI
           
        }

        public void TurnHappenedMove()
        {
            var playerCell = GameManager.Instance.PlayerController.CellPosition;
            _newCellTarget = _board.FindNextMove(_cell,  playerCell);

            if (_newCellTarget != Vector2Int.zero)
            {
                _newDirection.x= _newCellTarget.x - _cell.x;
                _newDirection.y = _newCellTarget.y - _cell.y;
                GoMoveTo(_newCellTarget, false);
            }
        }
        
      
      
    }
}