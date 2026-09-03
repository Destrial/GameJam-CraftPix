using UnityEngine;
using UnityEngine.UIElements;

namespace UnityLearn
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; } // SINGLETON

        public UIDocument UIDoc;
        private Label m_FoodLabel;
        private VisualElement m_GameOverPanel;
        private Label m_GameOverMessage;

        public BoardManager BoardManager;
        public PlayerController PlayerController;
        private int m_FoodAmount = 100;
        [SerializeField] private int m_StartFood = 20;
        private int m_CurrentLevel = 0;

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

            m_FoodLabel = UIDoc.rootVisualElement.Q<Label>("FoodLabel");

            m_GameOverPanel = UIDoc.rootVisualElement.Q<VisualElement>("GameOverPanel");
            m_GameOverMessage = m_GameOverPanel.Q<Label>("GameOverMessage");

            StartNewGame();
        }

        public void StartNewGame()
        {
            m_GameOverPanel.style.visibility = Visibility.Hidden;

            m_CurrentLevel = 1;
            m_FoodAmount = m_StartFood;
            m_FoodLabel.text = "Food : " + m_FoodAmount;

            BoardManager.Clean();
            BoardManager.Init();

            PlayerController.Init();
            PlayerController.Spawn(BoardManager, new Vector2Int(1, 1));
        }

        public void NewLevel()
        {
            BoardManager.Clean();
            BoardManager.Init();
            PlayerController.Spawn(BoardManager, new Vector2Int(1, 1));

            m_CurrentLevel++;
        }

        void OnTurnHappen()
        {
            ChangeFood(-1);
        }

        public void ChangeFood(int amount)
        {
            m_FoodAmount += amount;
            m_FoodLabel.text = "Food : " + m_FoodAmount;

            if (m_FoodAmount <= 0)
            {
                PlayerController.GameOver();
                m_GameOverPanel.style.visibility = Visibility.Visible;
                m_GameOverMessage.text = "<size=32>Game Over!</size>\n\nYou traveled through\n\n " + m_CurrentLevel +
                                         " levels";

            }
        }
    }
}