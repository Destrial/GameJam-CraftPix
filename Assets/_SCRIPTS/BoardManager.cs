using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

namespace Destrial
{


    public class BoardManager : MonoBehaviour
    {

        public class CellData
        {
            public bool Passable;
            public CellObject ContainedObject;
        }

        private CellData[,] _boardData;


        private Tilemap _tilemap;
        private Grid _grid;

        public ExitCellObject ExitCellPrefab;
        public FoodObject[] FoodPrefab;
        public WallObject[] WallPrefab;
        public Enemy EnemyPrefab;
        public int Width;
        public int Height;

        public Tile[] GroundTiles;
        public Tile[] WallTiles;

        [SerializeField] int _enemyNumber = 6;
        public PlayerController Player;
        private List<Vector2Int> _emptyCellsList;

        // Start is called before the first frame update
        public void Init()
        {
            _tilemap = GetComponentInChildren<Tilemap>();
            _grid = GetComponentInChildren<Grid>();

            _boardData = new CellData[Width, Height];
            _emptyCellsList = new List<Vector2Int>();

            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    Tile tile;
                    _boardData[x, y] = new CellData();

                    if (x == 0 || y == 0 || x == Width - 1 || y == Height - 1)
                    {
                        tile = WallTiles[Random.Range(0, WallTiles.Length)];
                        _boardData[x, y].Passable = false;
                    }
                    else
                    {
                        tile = GroundTiles[Random.Range(0, GroundTiles.Length)];
                        _boardData[x, y].Passable = true;
                        //this is a passable empty cell, add it to the list!
                        _emptyCellsList.Add(new Vector2Int(x, y));
                    }

                    _tilemap.SetTile(new Vector3Int(x, y, 0), tile);

                    //int tileNumber = Random.Range(0, GroundTiles.Length);
                    //m_Tilemap.SetTile(new Vector3Int(x, y, 0), GroundTiles[tileNumber]);
                }
            }

            //remove the starting point of the player! It's not empty, the player is there
            _emptyCellsList.Remove(new Vector2Int(1, 1));

            Vector2Int endCoord = new Vector2Int(Width - 2, Height - 2);
            AddObject(Instantiate(ExitCellPrefab), endCoord);
            _emptyCellsList.Remove(endCoord);

            GenerateWall();
            GenerateFood();
            GenerateEnemy();


        }

        public Vector3 CellToWorld(Vector2Int cellIndex)
        {
            return _grid.GetCellCenterWorld((Vector3Int)cellIndex);
        }

        public CellData GetCellData(Vector2Int cellIndex)
        {
            if (cellIndex.x < 0 || cellIndex.x >= Width
                                || cellIndex.y < 0 || cellIndex.y >= Height)
            {
                return null;
            }

            return _boardData[cellIndex.x, cellIndex.y];
        }

        void GenerateFood()
        {
            int foodCount = 5;
            for (int i = 0; i < foodCount; ++i)
            {
                int randomIndex = Random.Range(0, _emptyCellsList.Count);
                Vector2Int coord = _emptyCellsList[randomIndex];

                _emptyCellsList.RemoveAt(randomIndex);

                int numi = Random.Range(0, FoodPrefab.Length);
                FoodObject newFood = Instantiate(FoodPrefab[numi]);

                AddObject(newFood, coord);
            }
        }

        void GenerateWall()
        {
            int wallCount = Random.Range(6, 10);
            for (int i = 0; i < wallCount; ++i)
            {
                int randomIndex = Random.Range(0, _emptyCellsList.Count);
                Vector2Int coord = _emptyCellsList[randomIndex];

                _emptyCellsList.RemoveAt(randomIndex);

                int numi = Random.Range(0, WallPrefab.Length);
                WallObject newWall = Instantiate(WallPrefab[numi]);

                AddObject(newWall, coord);
            }
        }

        void GenerateEnemy()
        {
            int enemyCount = Random.Range(1, _enemyNumber);
            for (int i = 0; i < enemyCount; ++i)
            {
                int randomIndex = Random.Range(0, _emptyCellsList.Count);
                Vector2Int coord = _emptyCellsList[randomIndex];

                _emptyCellsList.RemoveAt(randomIndex);


                Enemy newEnemy = Instantiate(EnemyPrefab);

                AddObject(newEnemy, coord);
            }
        }


        public void SetCellTile(Vector2Int cellIndex, Tile tile)
        {
            _tilemap.SetTile(new Vector3Int(cellIndex.x, cellIndex.y, 0), tile);
        }

        public Tile GetCellTile(Vector2Int cellIndex)
        {
            return _tilemap.GetTile<Tile>(new Vector3Int(cellIndex.x, cellIndex.y, 0));
        }

        void AddObject(CellObject obj, Vector2Int coord)
        {
            CellData data = _boardData[coord.x, coord.y];
            obj.transform.position = CellToWorld(coord);
            data.ContainedObject = obj;
            obj.Init(coord);
        }

        public void Clean()
        {
            //no board data, so exit early, nothing to clean
            if (_boardData == null)
                return;


            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    var cellData = _boardData[x, y];

                    if (cellData.ContainedObject != null)
                    {
                        //CAREFUL! Destroy the GameObject NOT just cellData.ContainedObject
                        //Otherwise what you are destroying is the JUST CellObject COMPONENT
                        //and not the whole gameobject with sprite
                        Destroy(cellData.ContainedObject.gameObject);
                    }

                    SetCellTile(new Vector2Int(x, y), null);
                }
            }
        }
    }
}
