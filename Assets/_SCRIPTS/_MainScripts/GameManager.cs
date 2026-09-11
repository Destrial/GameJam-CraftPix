using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;

namespace Destrial
{

    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { get; private set; } // SINGLETON

//public UIDocument UIDoc;
      //  private Label _lifeLabel;
      //  private VisualElement _gameOverPanel;
      //  private Label _gameOverMessage;

        public BoardManager BoardManager;
        public PlayerController PlayerController;

      
        public int CurrentLevel = 0;

        public Vector2Int PlayerSpawnPosition;
     
        public TurnManager TurnManager { get; private set; }

        public HashSet<Enemy> Enemies;
        [SerializeField] private float _attackSpeed;
        bool isWaiting = false;
        
        
        public UIManager MyUIManager;
        
        // STATS
        public int PlayerCurrentHealth = 10;
        [SerializeField] private int _startingHealth = 20;

        public int PlayerDMG = 1;

        public int PlayerDefense = 0;
        
        //public int ThrowDMG = 1;

        //public int PlayerSpeed = 1;    (Implement maybe later)

        
        // Counters
        public int KillAmount = 0;

        public int DestroyAmount = 0;

        public int FoodAmount = 0;
        
        public int PickupAmount = 0;
        
        
        public int AttacksAmount = 0;

        public int HitsTakenAmount = 0;
        

        
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
            TurnManager.OnMobDie += OnMobDieHappen;
            TurnManager.OnPickup += OnPickUpHappen;
            TurnManager.OnDestroy += OnDestroyHappen;
            TurnManager.OnLevelUp += OnLevelUpHappen;

          //  _lifeLabel = UIDoc.rootVisualElement.Q<Label>("LifeLabel");

          //  _gameOverPanel = UIDoc.rootVisualElement.Q<VisualElement>("GameOverPanel");
           // _gameOverMessage = _gameOverPanel.Q<Label>("GameOverMessage");

            StartNewGame();
            
            _audioSource.clip = musicClip;
            _audioSource.loop = true;
            _audioSource.volume = musicVolume;
            _audioSource.Play();
        }


        public void OnLevelUpHappen()
        {
            
        }

        public void StartNewGame()
        {
         //   _gameOverPanel.style.visibility = Visibility.Hidden;
        
            CurrentLevel = 1;
            PlayerCurrentHealth = _startingHealth;
            MyUIManager.Init();
          //  _lifeLabel.text = "Health : " + _currentHealthAmount;
             
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
            CurrentLevel++;
        }

        void OnTurnHappen()
        {
            
            StartCoroutine(StartTimerAttack());
        }

        void OnMobDieHappen()
        {
            MyUIManager.RefreshKills();

            if (KillAmount == 10)
            {
                TurnManager.LevelUp();
                
            }
        }

        void OnPickUpHappen()
        {
            MyUIManager.RefreshPickup();
        }

        void OnDestroyHappen()
        {
            MyUIManager.RefreshDestroy();
        }
        
        public void ChangeLife(int amount)
        {
            PlayerCurrentHealth += amount;
            MyUIManager.ShowLife();
         
           // _lifeLabel.text = "Health : " + _currentHealthAmount;

            if (PlayerCurrentHealth <= 0)
            {
                PlayerController.GameOver();
                MyUIManager.ShowGameOver();
              

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