using UnityEngine;
using UnityEngine.Tilemaps;

namespace Destrial
{

    public class WallObject : CellObject
    {
        public Tile ObstacleTile;
        public int MaxHealth = 3;

        private int _healthPoint;
        private Tile _originalTile;

        public override void Init(Vector2Int cell)
        {
            base.Init(cell);
            _healthPoint = MaxHealth;

            _originalTile = GameManager.Instance.BoardManager.GetCellTile(cell);
            GameManager.Instance.BoardManager.SetCellTile(cell, ObstacleTile);
        }

        public override bool PlayerWantsToEnter()
        {
            _healthPoint -= 1;

            if (_healthPoint > 0)
            {
                return false;
            }

            GameManager.Instance.BoardManager.SetCellTile(_cell, _originalTile);
            Destroy(gameObject);
            return true;
        }
    }
}
