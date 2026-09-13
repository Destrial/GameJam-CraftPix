using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System.Collections;
using DG.Tweening;
using UnityEditor;

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


        public int RoomLevel = 0;
        public int GameLevel = 1;
            

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
        public int PlayerLevel = 1;
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
        public int TotalGrow = 0;
        
        //DECA-Counters
        public bool decaKillActivated = false;
        public bool decaGrowthActivated = false;

        public int GrowthTIME = 0;
        [SerializeField] private int _decaGrowDuration = 20;
        
       [SerializeField] AudioSource _audioSource;
       [SerializeField] AudioSource _audioSourceMusic;
       [SerializeField] AudioSource _audioSourceVoice;
       [SerializeField] AudioClip _audioDecaGrowth;
       [SerializeField] AudioClip _audioDecaKill;
       [SerializeField] AudioClip _audioDecaLoot;
       [SerializeField] AudioClip _audioDecaDestroy;
       [SerializeField] AudioClip _audioLevelUp;
       [SerializeField] AudioClip _audioLose;
       [SerializeField] AudioClip[] _audioPickUp;
       [SerializeField] AudioClip[] _audioBadPickUp;
       [SerializeField] AudioClip[] _audioMobDeath;
       [SerializeField] AudioClip[] _audioDestroy;
       [SerializeField] AudioClip _audioNextLevel;
       [SerializeField] AudioClip[] _audioHit;
       
       [SerializeField] AudioClip[] _audioINSTAKILL;
       
       [SerializeField] AudioClip _audioStopGrow;
         
       [SerializeField] AudioClip _musicDungeon;
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
            // 1. Reveal the hardware mouse pointer
            UnityEngine.Cursor.visible = true;

            // 2. Unlock the cursor so it can move freely across the screen
            UnityEngine.Cursor.lockState = CursorLockMode.None; 
           

          //  _lifeLabel = UIDoc.rootVisualElement.Q<Label>("LifeLabel");

          //  _gameOverPanel = UIDoc.rootVisualElement.Q<VisualElement>("GameOverPanel");
           // _gameOverMessage = _gameOverPanel.Q<Label>("GameOverMessage");

          //  StartNewGame();
            BoardManager.Player.MyState=PlayerController.PlayerState.Wait;
            _audioSourceMusic.clip = musicClip;
            _audioSourceMusic.loop = true;
            _audioSourceMusic.volume = musicVolume;
            _audioSourceMusic.Play();
        }


       

        public void StartNewGame()
        {
            UnityEngine.Cursor.visible = false;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked; 
        
         _audioSourceMusic.clip = _musicDungeon;
         _audioSourceMusic.loop = true;
         _audioSourceMusic.volume = musicVolume;
         _audioSourceMusic.Play();
            DOTween.KillAll(complete: true); 
            RoomLevel = 1;
            GameLevel = 1;
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
            PlayerLevel = 1;
            AttacksAmount = 0;
            HitsTakenAmount = 0;
            TurnManager.TurnCount = 0;
            AmountGrow = 0;
            TotalGrow = 0;
            MyUIManager.Init();
          //  _lifeLabel.text = "Health : " + _currentHealthAmount;
             
            BoardManager.Clean();
            BoardManager.Init();
          
            PlayerController.Init();
            PlayerController.Spawn(BoardManager, PlayerSpawnPosition);
           
        }

        public void NewLevel()
        {
            RoomLevel++;
            GameLevel=1+RoomLevel/ 10;
            BoardManager.Clean();
            BoardManager.Init();
           
         
            PlayerController.Spawn(BoardManager, PlayerSpawnPosition);
            PlayerController.Init();
            _audioSource.PlayOneShot(_audioNextLevel, sfxVolume);
            MyUIManager.RefreshALL();
       
        }

        void OnTurnHappen()
        {
        //    Debug.Log("Turn: "+TurnManager.TurnCount);
            StartCoroutine(StartTimerAttack());
            if (decaGrowthActivated)
            {
                GrowthTIME--;
                if (GrowthTIME <= 0)
                {
                    decaGrowthActivated = false;
                    MyUIManager.HidePower();
                 
                    BoardManager.Player.transform.DOScale(1f, 0.5f).SetEase(Ease.OutCubic);
                    _audioSource.PlayOneShot(_audioStopGrow, sfxVolume);
                }   
            }
        }

        void OnMobDieHappen()
        {
            MyUIManager.RefreshKills();
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
                if (decaGrowthActivated)
                {
                    amount = 0;

                }
                else if (amount > 0) //degat minimum -1
                {
                    amount = -1;
                }
              
            }

           
            BoardManager.Player.DMGTXT.Initialize(amount);

           
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
                _audioSource.PlayOneShot(_audioLose, sfxVolume);
          //  Debug.Log("GAME OVER: "+_audioLose+" "+sfxVolume);

            }
        }
        
        
        IEnumerator StartTimerAttack()
        {
          
          
           BoardManager.Player.GoWait();
   
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
            if (PlayerController.MyState != PlayerController.PlayerState.Death)
            {
                BoardManager.Player.GoIdle();
            }
            
        }

        public void MobDeath(Enemy.EnemyType typeMob,bool normalSound)
        {
            if (normalSound)
            {
                _audioSource.PlayOneShot(_audioMobDeath[Random.Range(0, _audioMobDeath.Length)], sfxVolume);
            }

            else
            {
              
                _audioSource.PlayOneShot(_audioINSTAKILL[Random.Range(0, _audioINSTAKILL.Length)],sfxVolume);
            }
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
       
      
        public void LevelUp()
        {
            BoardManager.Player.DecaTXT.Initialize(DecaTxt.DecaType.LevelUP);
            BoardManager.Player.GoWait();
            PlayerLevel++;
             TurnManager.LevelUp(); //event
        
          
             MyUIManager.ShowLevelUp(true);
        }
        

        public void AddDestroy()
        {
            DestroyAmount++;
            MyUIManager.RefreshDestroy();
            cumulDestroy++;
            if (cumulDestroy == 10)
            {
                cumulDestroy = 0;
                AddXP();
                AddMegaGROW();
              
                BoardManager.Player.DecaTXT.Initialize(DecaTxt.DecaType.DecaDestroy);
                if (!decaGrowthActivated)
                {
                    _audioSource.PlayOneShot(_audioDecaDestroy, sfxVolume);
                    MyUIManager.ShowPower(DecaTxt.DecaType.DecaDestroy);
                }

                MaxHealth++;
             
                MyUIManager.ShowLife();
                /// GO REWARD DESTROY
                /// DROP ITEM
            }

        }

        public void AddPickup()
        {
            PickupAmount++;
            MyUIManager.RefreshPickup();
            cumulPickup++;
            if (cumulPickup == 10)
            {
                cumulPickup = 0;
                AddXP();
                AddMegaGROW();
                PlayerCurrentHealth = MaxHealth;
                MyUIManager.ShowLife();
               
                BoardManager.Player.DecaTXT.Initialize(DecaTxt.DecaType.DecaLoot);
                if (!decaGrowthActivated)
                {
                    _audioSourceVoice.PlayOneShot(_audioDecaLoot, sfxVolume);
                    MyUIManager.ShowPower(DecaTxt.DecaType.DecaLoot);
                }

                PlayerCurrentHealth=MaxHealth;
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
           
            if (cumulKill == 10)
            {
                AddMegaGROW();
                cumulKill = 0;
              
               BoardManager.Player.DecaTXT.Initialize(DecaTxt.DecaType.DecaKill);
               if (!decaGrowthActivated)
               {
                   _audioSourceVoice.PlayOneShot(_audioDecaKill, sfxVolume);
                   MyUIManager.ShowPower(DecaTxt.DecaType.DecaKill);
               }

               decaKillActivated = true;
               




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
            if (cumulXP == 10)
            {
//Debug.Log("LEVEL UP");
                cumulXP = 0;
                LevelUp();
                _audioSource.PlayOneShot(_audioLevelUp, sfxVolume);
            }
        }

        void AddMegaGROW()
        {
            AmountGrow++;
          
            if (AmountGrow == 3)
            {
                AmountGrow = 0;
                TotalGrow++;
                _audioSourceVoice.PlayOneShot(_audioDecaGrowth, sfxVolume);
                BoardManager.Player.DecaTXT.Initialize(DecaTxt.DecaType.DecaGrowth);
                MyUIManager.ShowPower(DecaTxt.DecaType.DecaGrowth);
                decaGrowthActivated = true;
                GrowthTIME = _decaGrowDuration;
                BoardManager.Player.transform.DOScale(1.8f, 0.5f).SetEase(Ease.OutCubic);
                //MEGA GROW
                // INVULENRABILITY 10 TOURS + VITESSE x2 + ATTTX2
            }
            MyUIManager.RefreshGrow();
        }


        public void HitSound()
        {
            _audioSource.PlayOneShot(_audioHit[Random.Range(0, _audioHit.Length)], sfxVolume);
        }


        public void PlayIntro()
        {
            _audioSourceMusic.clip = musicClip;
            _audioSourceMusic.loop = true;
            _audioSourceMusic.volume = musicVolume;
            _audioSourceMusic.Play();
        }
       
    }
}