using UnityEngine;
using UnityEngine.Tilemaps;

namespace Destrial
{

    public class ExitCellObject : CellObject
    {
        public Tile EndTile;
        public BoardManager.RoomSide RoomSide;
        public override void Init(Vector2Int coord)
        {
            base.Init(coord);
            GameManager.Instance.BoardManager.SetCellTile(coord, EndTile);
        }

        public override void PlayerEntered()
        {
            GameManager.Instance.PlayerController.MyState = PlayerController.PlayerState.ChangeRoom;
            switch (RoomSide)
            {
                case BoardManager.RoomSide.Bottom:
                    GameManager.Instance.BoardManager.PlayerSide = BoardManager.RoomSide.Top;
                    break;
                case BoardManager.RoomSide.Right:
                    GameManager.Instance.BoardManager.PlayerSide = BoardManager.RoomSide.Left;
                    break;
                case BoardManager.RoomSide.Top:
                    GameManager.Instance.BoardManager.PlayerSide = BoardManager.RoomSide.Bottom;
                    break;
                case BoardManager.RoomSide.Left:
                    GameManager.Instance.BoardManager.PlayerSide = BoardManager.RoomSide.Right;

                    break;
            }
            
            GameManager.Instance.NewLevel();
        }
    }
}
