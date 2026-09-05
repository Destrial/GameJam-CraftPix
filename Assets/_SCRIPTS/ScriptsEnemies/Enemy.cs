using UnityEngine;
using System.Collections;


namespace Destrial
{

    public class Enemy : CellObject
    {
        //First choose which enemy it is (NOT YET IMPLEMENTED)
        public enum EnemyType {Rat, Goblin, BladeGoblin, ShamanGoblin}
        
        private Animator _animator;
        private Vector2Int _newDirection; //Not used yet, WE NEEED DIRECTIONAL MOVEMENT


        [SerializeField] private GameObject _deathPrefab;
        
        
        
        
        
        public int Health = 3;

        private int _currentHealth;
        

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            
            GameManager.Instance.TurnManager.OnTick += TurnHappened;  //EVENT
        }

        private void OnDestroy()
        {
            GameManager.Instance.TurnManager.OnTick -= TurnHappened;
        }

        public override void Init(Vector2Int coord)
        {
            base.Init(coord);
            _currentHealth = Health;
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
                if (absXDist > absYDist)
                {
                    if (!TryMoveInX(xDist))
                    {
                        //if our move was not successful (so no move and not attack)
                        //we try to move along Y
                        TryMoveInY(yDist);
                    }
                }
                else
                {
                    if (!TryMoveInY(yDist))
                    {
                        TryMoveInX(xDist);
                    }
                }
            }
        }

        bool TryMoveInX(int xDist)
        {
            //try to get closer in x

            //player to our right
            if (xDist > 0)
            {
                return MoveTo(_cell + Vector2Int.right);
            }

            //player to our left
            return MoveTo(_cell + Vector2Int.left);
        }

        bool TryMoveInY(int yDist)
        {
            //try to get closer in y

            //player on top
            if (yDist > 0)
            {
                return MoveTo(_cell + Vector2Int.up);
            }

            //player below
            return MoveTo(_cell + Vector2Int.down);
        }
    }
}