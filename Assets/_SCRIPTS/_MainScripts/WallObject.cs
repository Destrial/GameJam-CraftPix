using UnityEngine;
using UnityEngine.Tilemaps;

namespace Destrial
{

    public class WallObject : CellObject
    {
       // public Tile ObstacleTile;
       
        [SerializeField] SpriteRenderer _spriteRenderer;
        public Sprite DestroySprite1;
        public Sprite DestroySprite2;
        public int MaxHealth = 3;
        public int PlayerDmg = 3;
        private int _healthPoint;
        private Tile _originalTile;

        private Vector2Int myPos;
        
        [SerializeField] AudioSource _audioSource;
     
        [SerializeField] AudioClip _audioImpact;
       

        public override void Init(Vector2Int cell)
        {
          
            base.Init(cell);
            _healthPoint = MaxHealth;
            myPos = cell;
            _originalTile = GameManager.Instance.BoardManager.GetCellTile(cell);
          //  GameManager.Instance.BoardManager.SetCellTile(cell, ObstacleTile);
        }

        public override bool PlayerWantsToEnter()
        {
            _healthPoint -= 1;
            _audioSource.PlayOneShot(_audioImpact);
            if (_healthPoint == 2)
            {
                _spriteRenderer.sprite = DestroySprite1;
                GameManager.Instance.ChangeLife(-PlayerDmg);
                GameManager.Instance.BoardManager.Player.GetHurt(-PlayerDmg);
                //GameManager.Instance.BoardManager.SetCellTile(myPos, DestroyTile1);
            }
            else if (_healthPoint == 1)
            {
                _spriteRenderer.sprite = DestroySprite2;
                GameManager.Instance.ChangeLife(-PlayerDmg);
                GameManager.Instance.BoardManager.Player.GetHurt(-PlayerDmg);
               // GameManager.Instance.BoardManager.SetCellTile(myPos, DestroyTile2);
            }

            if (_healthPoint > 0)
            {
                return false;
            }

          //  GameManager.Instance.BoardManager.SetCellTile(_cell, _originalTile);
            GameManager.Instance.BoardManager.FreeBoard(_cell,true);
            GameManager.Instance.ChangeLife(-PlayerDmg);
            GameManager.Instance.BoardManager.Player.GetHurt(-PlayerDmg);
            GameManager.Instance.WallDestroyed();
            Destroy(gameObject);
            return true;
        }
    }
}
