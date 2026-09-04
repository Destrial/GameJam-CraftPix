using UnityEngine;
using UnityEngine.UIElements;

namespace Destrial
{

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; } // SINGLETON

        public UIDocument UIDoc;
        private Label _foodLabel;
        private VisualElement _gameOverPanel;
        private Label _gameOverMessage;

        public BoardManager BoardManager;
        public PlayerController PlayerController;
        private int _foodAmount = 100;
        [SerializeField] private int _startFood = 40;
        private int _currentLevel = 0;

        public Vector2Int PlayerSpawnPosition;

        public TurnManager TurnManager { get; private set; }

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            TurnManager = new TurnManager();
            TurnManager.OnTick += OnTurnHappen;

            _foodLabel = UIDoc.rootVisualElement.Q<Label>("FoodLabel");

            _gameOverPanel = UIDoc.rootVisualElement.Q<VisualElement>("GameOverPanel");
            _gameOverMessage = _gameOverPanel.Q<Label>("GameOverMessage");

            StartNewGame();
        }

        public void StartNewGame()
        {
            _gameOverPanel.style.visibility = Visibility.Hidden;

            _currentLevel = 1;
            _foodAmount = _startFood;
            _foodLabel.text = "Food : " + _foodAmount;

            BoardManager.Clean();
            BoardManager.Init();

            PlayerController.Init();
            PlayerController.Spawn(BoardManager, PlayerSpawnPosition);
        }

        public void NewLevel()
        {
            BoardManager.Clean();
            BoardManager.Init();
            PlayerController.Spawn(BoardManager, PlayerSpawnPosition);
            PlayerController.Init();
            _currentLevel++;
        }

        void OnTurnHappen()
        {
            //ChangeFood(-1);
        }

        public void ChangeFood(int amount)
        {
            _foodAmount += amount;
            _foodLabel.text = "Food : " + _foodAmount;

            if (_foodAmount <= 0)
            {
                PlayerController.GameOver();
                _gameOverPanel.style.visibility = Visibility.Visible;
                _gameOverMessage.text = "<size=32>Game Over!</size>\n\nYou traveled through\n\n " + _currentLevel +
                                         " levels";

            }
        }
    }
}