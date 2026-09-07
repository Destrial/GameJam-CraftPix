using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

namespace Destrial
{

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; } // SINGLETON

        public UIDocument UIDoc;
        private Label _lifeLabel;
        private VisualElement _gameOverPanel;
        private Label _gameOverMessage;

        public BoardManager BoardManager;
        public PlayerController PlayerController;

        
        private int _currentLevel = 0;

        public Vector2Int PlayerSpawnPosition;
     
        public TurnManager TurnManager { get; private set; }

        public HashSet<Enemy> Enemies;
        [SerializeField] private float _attackSpeed;
        bool isWaiting = false;
        
        
        
        
        // STATS
        private int _currentHealthAmount = 100;
        [SerializeField] private int _startingHealth = 40;

        private int _killAmount = 0;

        private int _destroyAmount = 0;
        public int CurrentHealthAmount
        {
            get { return _currentHealthAmount; }
        }
        //

       [SerializeField] AudioSource _audioSource;
       [SerializeField] AudioClip _audioDecaGrowth;
       [SerializeField] AudioClip _audioDecaLoot;
       [SerializeField] AudioClip _audioDecaDestroy;
       [SerializeField] AudioClip _audioLevelUp;
       [SerializeField] AudioClip _audioLose;
       [SerializeField] AudioClip[] _audioPickUp;
       [SerializeField] AudioClip[] _audioBadPickUp;
       [SerializeField] AudioClip[] _audioMobDeath;
       [SerializeField] AudioClip[] _audioDestroy;
       [SerializeField] AudioClip _audioNextLevel;
       
       
       public AudioClip musicClip;
     

       [Range(0f, 1f)] public float musicVolume = 0.5f;
       [Range(0f, 1f)] public float sfxVolume = 1.0f;
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

            _lifeLabel = UIDoc.rootVisualElement.Q<Label>("LifeLabel");

            _gameOverPanel = UIDoc.rootVisualElement.Q<VisualElement>("GameOverPanel");
            _gameOverMessage = _gameOverPanel.Q<Label>("GameOverMessage");

            StartNewGame();
            
            _audioSource.clip = musicClip;
            _audioSource.loop = true;
            _audioSource.volume = musicVolume;
            _audioSource.Play();
        }
        

        public void StartNewGame()
        {
            _gameOverPanel.style.visibility = Visibility.Hidden;

            _currentLevel = 1;
            _currentHealthAmount = _startingHealth;
            _lifeLabel.text = "Health : " + _currentHealthAmount;

            BoardManager.Clean();
            BoardManager.Init();

          
            PlayerController.Spawn(BoardManager, PlayerSpawnPosition);
            PlayerController.Init();
        }

        public void NewLevel()
        {
           
            BoardManager.Clean();
            BoardManager.Init();
           
         
            PlayerController.Spawn(BoardManager, PlayerSpawnPosition);
            PlayerController.Init();
            _audioSource.PlayOneShot(_audioNextLevel, sfxVolume);
         //   Debug.Log(""+PlayerController.CellPosition+"/"+PlayerSpawnPosition);
            _currentLevel++;
        }

        void OnTurnHappen()
        {
            
            StartCoroutine(StartTimerAttack());
        }

        public void ChangeLife(int amount)
        {
            _currentHealthAmount += amount;
            _lifeLabel.text = "Health : " + _currentHealthAmount;

            if (_currentHealthAmount <= 0)
            {
                PlayerController.GameOver();
                _gameOverPanel.style.visibility = Visibility.Visible;
                _gameOverMessage.text = "<size=32>Game Over!</size>\n\nYou traveled through\n\n " + _currentLevel +
                                         " levels";

            }
        }
        
        IEnumerator StartTimerAttack()
        {
          
           PlayerController.MyState = PlayerController.PlayerState.Wait;
           
   
            foreach (Enemy enemy in new List<Enemy>(Enemies))
            {
                
                if (enemy != null)
                {
                    if (enemy.CanAttack())
                    {
                        enemy.TurnHappenedAttack();
                        yield return new WaitForSeconds(_attackSpeed);
                        
                    }
                    else
                    {
                        enemy.TurnHappenedMove();
                    }
                   
                }


            }

            PlayerController.MyState = PlayerController.PlayerState.Idle;
          //  PlayerController.MyAction = PlayerController.PlayerState.Idle;
        }

        public void MobDeath(Enemy.EnemyType typeMob)
        {
            _audioSource.PlayOneShot(_audioMobDeath[Random.Range(0, _audioMobDeath.Length)], sfxVolume);
        }
        public void WallDestroyed()
        {
            _audioSource.PlayOneShot(_audioDestroy[Random.Range(0, _audioDestroy.Length)], sfxVolume);
        }
        public void AudioPickup(bool isGood)
        {
            if (isGood)
            {
                _audioSource.PlayOneShot(_audioPickUp[Random.Range(0, _audioPickUp.Length)], sfxVolume);
            }
            else
            {
                _audioSource.PlayOneShot(_audioBadPickUp[Random.Range(0, _audioBadPickUp.Length)], sfxVolume);
            }
        }
        public void AudioLoot()
        {
            _audioSource.PlayOneShot(_audioDecaLoot, sfxVolume);
        }
        public void AudioGrowth()
        {
            _audioSource.PlayOneShot(_audioDecaGrowth, sfxVolume);
        }
        public void LevelUp()
        {
            _audioSource.PlayOneShot(_audioLevelUp, sfxVolume);
        }
        public void GameOver()
        {
            _audioSource.PlayOneShot(_audioLose, sfxVolume);
        }
       
        
    }
}