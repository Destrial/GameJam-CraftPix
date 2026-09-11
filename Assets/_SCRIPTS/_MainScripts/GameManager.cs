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
        public int MaxHealth;
        public int PlayerAttack = 1;

        public int PlayerDefense = 0;
        
        public int PlayerSpeed = 1;
        
        //public int ThrowDMG = 1;

        //public int PlayerSpeed = 1;    (Implement maybe later)

        
        // Counters
        public int KillAmount = 0;
        private int cumulKill;
        public int DestroyAmount = 0;
        private int cumulDestroy;
        public int FoodAmount = 0;
        
        public int PickupAmount = 0;
        private int cumulPickup;
        public int XPAmount = 0;
        private int cumulXP;
        public int AttacksAmount = 0;

        public int HitsTakenAmount = 0;
        public int AmountGrow = 0;

        
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

          //  StartNewGame();
            BoardManager.Player.MyState=PlayerController.PlayerState.Wait;
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
            MaxHealth = _startingHealth;
            XPAmount = 0;
            cumulXP = 0;
            KillAmount = 0;
            cumulKill = 0;
            DestroyAmount = 0;
            cumulDestroy = 0;
            PickupAmount = 0;
            cumulPickup = 0;
            PlayerAttack = 1;
            PlayerDefense = 0;
            PlayerSpeed = 1;

            AttacksAmount = 0;
            HitsTakenAmount = 0;
            
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
           // Debug.Log("Before: "+amount+" "+PlayerDefense);
            HitsTakenAmount++;
            if (amount < 0) // si degats 
            {
                amount += PlayerDefense;
              
            }

           
            PlayerCurrentHealth += amount;
          //  Debug.Log("After: "+amount);
            if (PlayerCurrentHealth > MaxHealth) // si trop de soin
            {
                PlayerCurrentHealth = MaxHealth;
            }
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

        public void AddDestroy()
        {
            DestroyAmount++;
            MyUIManager.RefreshDestroy();
            cumulDestroy++;
            if (cumulDestroy > 10)
            {
                cumulDestroy = 0;
                AddXP();
                AddMegaGROW();
                /// GO REWARD DESTROY
                /// DROP ITEM
            }

        }

        public void AddPickup()
        {
            PickupAmount++;
            MyUIManager.RefreshPickup();
            cumulPickup++;
            if (cumulPickup > 10)
            {
                cumulPickup = 0;
                AddXP();
                AddMegaGROW();
                PlayerCurrentHealth = MaxHealth;
                MyUIManager.ShowLife();
                /// GO REWARDFULL HEALTH
                /// FULL LIFE
                /// 

            }
        }

        public void AddKill()
        {
            KillAmount++;
            MyUIManager.RefreshKills();
            cumulKill++;
            AddXP();
            AddMegaGROW();
            if (cumulKill > 10)
            {
                cumulKill = 0;
                
               
                /// NEW HIT  SUPER COUP
                /// ONE HIT KILLS
                /// 

            }
        }

        void AddXP()
        {
            XPAmount++;
            MyUIManager.RefreshXP();
            cumulXP++;
            if (cumulXP > 10)
            {
                cumulXP = 0;
                LevelUp();
            }
        }

        void AddMegaGROW()
        {
            AmountGrow++;
            MyUIManager.RefreshGrow();
            if (AmountGrow > 3)
            {
                AmountGrow = 0;
                //MEGA GROW
                // INVULENRABILITY 10 TOURS + VITESSE x2 + ATTTX2
            }
        }
        
    }
}