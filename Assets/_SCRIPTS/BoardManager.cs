using System;
using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;
using Random = UnityEngine.Random;

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
        private bool CornerTOP_RIGHT;
        private bool CornerTOP_LEFT;
        private bool CornerBOTTOM_RIGHT;
        private bool CornerBOTTOM_LEFT;
        private Vector2Int _noRoomTOP_RIGHT;
        private Vector2Int _noRoomTOP_LEFT;
        private Vector2Int _noRoomBOTTOM_RIGHT;
        private Vector2Int _noRoomBOTTOM_LEFT;
        private bool[,] _noRoom;

        private Tilemap _tilemap;
        private Grid _grid;

        public ExitCellObject ExitCellPrefab;
        public FoodObject[] FoodPrefab;
        public WallObject[] WallDestroyPrefab;
        public Enemy EnemyPrefab;
        public int Width;
        public int Height;

        public Tile[] GroundTiles;
        public Tile[] WallTiles;
        public Tile EntranceTile;
        private Vector2Int _entranceCoord;
        
        [SerializeField] int _enemyNumber = 6;
        public PlayerController Player;
        private List<Vector2Int> _emptyCellsList;

        [SerializeField] int RoomSizeMin;
        [SerializeField] int RoomSizeMax;

        [SerializeField] float CutRoomChance = 0.6f;
        [SerializeField] float OneCornerChance = 0.5f;

        public enum RoomSide { Top, Bottom, Left, Right }
        
        public RoomSide PlayerSide;
        private List<RoomSide> _exitSides;

        private void OnEnable()
        {
            
            PlayerSide = RoomSide.Bottom;
          
        }

        // Start is called before the first frame update
        public void Init()
        {
            _tilemap = GetComponentInChildren<Tilemap>();
            _grid = GetComponentInChildren<Grid>();

            Width = Random.Range(RoomSizeMin, RoomSizeMax + 1);
            Height = Random.Range(RoomSizeMin, RoomSizeMax + 1);
            _boardData = new CellData[Width, Height];
            _noRoom = new bool[Width, Height];

            _emptyCellsList = new List<Vector2Int>();

            GenerateFloor();

            //remove the starting point of the player! It's not empty, the player is there
            _exitSides = new List<RoomSide>();
            _exitSides.Add(RoomSide.Bottom);
            _exitSides.Add(RoomSide.Top);
            _exitSides.Add(RoomSide.Left);
            _exitSides.Add(RoomSide.Right);
            
            _exitSides.Remove(PlayerSide);


          
            
            int numi = Random.Range(0, 3);

            for (int i = 0; i < numi; i++)
            {
                AddExit();
                Debug.Log("Exit Added");
            }
            
            switch (PlayerSide)  //Place player next to entrance
            {
                case RoomSide.Top:
                  
                    GameManager.Instance.PlayerSpawnPosition = new Vector2Int(Width / 2, Height - 1);
                    _tilemap.SetTile(new Vector3Int(Width / 2, Height,0), EntranceTile);
                  
                    _emptyCellsList.Remove(GameManager.Instance.PlayerSpawnPosition);
                    break;
                case RoomSide.Bottom:
                   
                    GameManager.Instance.PlayerSpawnPosition=new Vector2Int(Width/2, 1);
                    _tilemap.SetTile(new Vector3Int(Width / 2, 0,0), EntranceTile);
                    _emptyCellsList.Remove(GameManager.Instance.PlayerSpawnPosition);
                    break;
                case RoomSide.Left:
                  
                    GameManager.Instance.PlayerSpawnPosition=new Vector2Int(1, Height/2);
                    _tilemap.SetTile(new Vector3Int(0, Height/2,0), EntranceTile);
                    
                    break;
                case RoomSide.Right:     
                  
                    GameManager.Instance.PlayerSpawnPosition=new Vector2Int(Width-1, Height/2);
                    
                    _tilemap.SetTile(new Vector3Int(Width, Height/2,0), EntranceTile);
                    
                    _emptyCellsList.Remove(GameManager.Instance.PlayerSpawnPosition);
                    break;
            }
          
         
            GenerateWall();
            GenerateFood();
            GenerateEnemy();
        }

        void AddExit()
        {
            Vector2Int endCoord= new Vector2Int(Width - 2, Height - 2);
            ExitCellObject exito= Instantiate(ExitCellPrefab);
            
            RoomSide choice = _exitSides[Random.Range(0, _exitSides.Count)];
            _exitSides.Remove(choice);
            
            switch (choice)
            {

                case RoomSide.Left:
                    endCoord = new Vector2Int(0, Height/2);
                    exito.RoomSide= RoomSide.Right;
                    break;
                case RoomSide.Right:
                    endCoord = new Vector2Int(0, Height/2);
                    exito.RoomSide= RoomSide.Right;
                    break;
                case RoomSide.Top:
                    endCoord = new Vector2Int(Width/2, Height-1);
                    exito.RoomSide= RoomSide.Top;
                    break;
                case RoomSide.Bottom:
                    endCoord = new Vector2Int(Width/2, 0); 
                    exito.RoomSide= RoomSide.Bottom;
                   
                    break;

            }


            AddObject(exito, endCoord);
            _boardData[endCoord.x, endCoord.y].Passable = true;
            _emptyCellsList.Remove(endCoord);
        }

        void GenerateFloor()
        {
            Tile tile;

            //Fill Up with empty cells + border
            for (int y = 0; y < Height; ++y)
            {
                for (int x = 0; x < Width; ++x)
                {
                    _noRoom[x, y] = false;
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
                }
            }

            GenerateCorners();

            if (CornerBOTTOM_LEFT) //REMOVE BOTTOM LEFT
            {
                for (int y = 0; y < _noRoomBOTTOM_LEFT.y; ++y)
                {
                    for (int x = 0; x < _noRoomBOTTOM_LEFT.x; ++x)
                    {
                        if (x == _noRoomBOTTOM_LEFT.x - 1 || y == _noRoomBOTTOM_LEFT.y - 1)
                        {
                            tile = WallTiles[Random.Range(0, WallTiles.Length)];
                            _tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                            _boardData[x, y].Passable = false;
                            //this is a passable empty cell, add it to the list!
                            _emptyCellsList.Remove(new Vector2Int(x, y));
                        }

                        else // remove all
                        {
                            _boardData[x, y].Passable = false;
                            _emptyCellsList.Remove(new Vector2Int(x, y));
                            _tilemap.SetTile(new Vector3Int(x, y, 0), null);
                        }
                    }
                }
            }

            if (CornerTOP_LEFT) //REMOVE TOP LEFT
            {
                for (int y = Height - _noRoomTOP_LEFT.y; y < Height; ++y)
                {
                    for (int x = 0; x < _noRoomTOP_LEFT.x; ++x)
                    {
                        if (x == _noRoomTOP_LEFT.x - 1 || y == Height - _noRoomTOP_LEFT.y)
                        {
                            tile = WallTiles[Random.Range(0, WallTiles.Length)];
                            _tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                            _boardData[x, y].Passable = false;
                            //this is a passable empty cell, add it to the list!
                            _emptyCellsList.Remove(new Vector2Int(x, y));
                        }

                        else // remove all
                        {
                            _boardData[x, y].Passable = false;
                            _emptyCellsList.Remove(new Vector2Int(x, y));
                            _tilemap.SetTile(new Vector3Int(x, y, 0), null);
                        }
                    }
                }
            }

            if (CornerTOP_RIGHT) //REMOVE TOP RIGHT
            {
                for (int y = Height - _noRoomTOP_RIGHT.y; y < Height; ++y)
                {
                    for (int x = Width - _noRoomTOP_RIGHT.x; x < Width; ++x)
                    {
                        if (x == Width - _noRoomTOP_RIGHT.x || y == Height - _noRoomTOP_RIGHT.y)
                        {
                            tile = WallTiles[Random.Range(0, WallTiles.Length)];
                            _tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                            _boardData[x, y].Passable = false;
                            //this is a passable empty cell, add it to the list!
                            _emptyCellsList.Remove(new Vector2Int(x, y));
                        }

                        else // remove all
                        {
                            _boardData[x, y].Passable = false;
                            _emptyCellsList.Remove(new Vector2Int(x, y));
                            _tilemap.SetTile(new Vector3Int(x, y, 0), null);
                        }
                    }
                }
            }

            if (CornerBOTTOM_RIGHT) //REMOVE TOP RIGHT
            {
                for (int y = 0; y < _noRoomBOTTOM_RIGHT.y; ++y)
                {
                    for (int x = Width - _noRoomBOTTOM_RIGHT.x; x < Width; ++x)
                    {
                        if (x == Width - _noRoomBOTTOM_RIGHT.x || y == _noRoomBOTTOM_RIGHT.y - 1)
                        {
                            tile = WallTiles[Random.Range(0, WallTiles.Length)];
                            _tilemap.SetTile(new Vector3Int(x, y, 0), tile);
                            _boardData[x, y].Passable = false;
                            //this is a passable empty cell, add it to the list!
                            _emptyCellsList.Remove(new Vector2Int(x, y));
                        }

                        else // remove all
                        {
                            _boardData[x, y].Passable = false;
                            _emptyCellsList.Remove(new Vector2Int(x, y));
                            _tilemap.SetTile(new Vector3Int(x, y, 0), null);
                        }
                    }
                }
            }
        }

        void GenerateCorners()
        {
            CornerBOTTOM_LEFT = false;
            CornerTOP_LEFT = false;
            CornerTOP_RIGHT = false;
            CornerBOTTOM_RIGHT = false;

            //Corner Generate
            float rand1 = Random.Range(0f, 1f);
           
            if (rand1 < CutRoomChance)
            {
                float rand2 = Random.Range(0f, 1f);
                rand2 = 0;
                if (rand2 < OneCornerChance)
                {
                    int RandWidth = Random.Range(3, Width / 2);
                    int RandHeight = Random.Range(3, Height / 2);
                    int rand3 = Random.Range(0, 4);
                  
                    switch (rand3)
                    {
                        case 0: //BOOTOM LEFT
                            CornerBOTTOM_LEFT = true;
                            _noRoomBOTTOM_LEFT = new Vector2Int(RandWidth, RandHeight);
                            for (int y = 0; y < _noRoomBOTTOM_LEFT.y; ++y)
                            {
                                for (int x = 0; x < _noRoomBOTTOM_LEFT.x; ++x)
                                {
                                    _noRoom[x, y] = true;
                                }
                            }

                            break;

                        case 1:
                            CornerTOP_LEFT = true;
                            _noRoomTOP_LEFT = new Vector2Int(RandWidth, RandHeight);
                            for (int y = Height - _noRoomTOP_LEFT.y; y < Height; ++y)
                            {
                                for (int x = 0; x < _noRoomTOP_LEFT.x; ++x)
                                {
                                    _noRoom[x, y] = true;
                                }
                            }

                            break;

                        case 2:
                            CornerTOP_RIGHT = true;
                            _noRoomTOP_RIGHT = new Vector2Int(RandWidth, RandHeight);
                            for (int y = Height - _noRoomTOP_RIGHT.y; y < Height; ++y)
                            {
                                for (int x = Width - _noRoomTOP_RIGHT.x; x < Width; ++x)
                                {
                                    _noRoom[x, y] = true;
                                }
                            }

                            break;

                        case 3:
                            CornerBOTTOM_RIGHT = true;
                            _noRoomBOTTOM_RIGHT = new Vector2Int(RandWidth, RandHeight);
                            for (int y = 0; y < _noRoomBOTTOM_RIGHT.y; ++y)
                            {
                                for (int x = Width - _noRoomBOTTOM_RIGHT.x; x < Width; ++x)
                                {
                                    _noRoom[x, y] = true;
                                }
                            }

                            break;
                    }
                }
            }
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

                int numi = Random.Range(0, WallDestroyPrefab.Length);
                WallObject newWall = Instantiate(WallDestroyPrefab[numi]);

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