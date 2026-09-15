using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Destrial
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField] GameObject _startPanel;
        [SerializeField] GameObject _gameOverPanel;
        [SerializeField] GameObject _levelUpPanel;
        [SerializeField] GameObject _gamePanel;
        [SerializeField] GameObject _settingPanel;

        [SerializeField] private TextMeshProUGUI _dieByTXT;
        [SerializeField] private TextMeshProUGUI _dieStatsTXT;
        [SerializeField] private TextMeshProUGUI _dieMegaGrowthTXT;

        [SerializeField] private TextMeshProUGUI _lifeTXT;
        [SerializeField] private TextMeshProUGUI _attackTXT;
        [SerializeField] private TextMeshProUGUI _defTXT;
        [SerializeField] private TextMeshProUGUI _levelTXT;
        [SerializeField] private TextMeshProUGUI _roomTXT;
        [SerializeField] private TextMeshProUGUI _killsTXT;
        [SerializeField] private TextMeshProUGUI _destroyTXT;
        [SerializeField] private TextMeshProUGUI _pickupTXT;
        [SerializeField] private TextMeshProUGUI _xpTXT;
        [SerializeField] private TextMeshProUGUI _growTXT;
        [SerializeField] private Image _killsBAR;
        [SerializeField] private Image _destroyBAR;
        [SerializeField] private Image _pickupBAR;
        [SerializeField] private Image _xpBAR;
        [SerializeField] private Image _lifeBAR;
        [SerializeField] GameManager _gameManager;

        [SerializeField] private GameObject[] Powers;
        [SerializeField] private TextMeshProUGUI _growthTurnTXT;
        [SerializeField] private float _hidePowerTime = 2f;
        
        [SerializeField] private Slider _volumeSlider;
        [SerializeField] private Slider _musicSlider;
      

        void Start()
        {
            _volumeSlider.value = GameManager.Instance.sfxVolume;

            // Listen for slider value updates
            _volumeSlider.onValueChanged.AddListener(SetSFXAudioVolume);
            
            _musicSlider.value = GameManager.Instance.musicVolume;

            // Listen for slider value updates
            _musicSlider.onValueChanged.AddListener(SetMusicAudioVolume);
        }

        public void CloseSettings()
        {
            _settingPanel.SetActive(false);
            _startPanel.SetActive(true);
        }

        public void OpenSettings()
        {
            _settingPanel.SetActive(true);
            _startPanel.SetActive(false);
        }
        
        public void SetSFXAudioVolume(float value)
        {
          GameManager.Instance.sfxVolume=_volumeSlider.value;
          GameManager.Instance.HitSound();
        }
        
        public void SetMusicAudioVolume(float value)
        {
            GameManager.Instance.musicVolume=_musicSlider.value;
            GameManager.Instance.PlayIntro();
        }
        
        public void RefreshKills()
        {
            _killsTXT.text = "" + _gameManager.KillAmount % 10f + "/10";
            _killsBAR.fillAmount = (_gameManager.KillAmount % 10f) / 10f;
        }

        public void RefreshPickup()
        {
            _pickupTXT.text = "" + _gameManager.PickupAmount % 10f + "/10";
            _pickupBAR.fillAmount = (_gameManager.PickupAmount % 10f) / 10f;
        }

        public void RefreshDestroy()
        {
            _destroyTXT.text = "" + _gameManager.DestroyAmount % 10f + "/10";
            _destroyBAR.fillAmount = (_gameManager.DestroyAmount % 10f) / 10f;
        }

        public void RefreshXP()
        {
            _xpTXT.text = "" + _gameManager.XPAmount % 10f + "/10";
            _xpBAR.fillAmount = (_gameManager.XPAmount % 10f) / 10f;
            _levelTXT.text = "" + _gameManager.PlayerLevel;
        }



        public void ShowLife()
        {
            float calc = (float)_gameManager.PlayerCurrentHealth / (float)_gameManager.MaxHealth;
            _lifeTXT.text = "" + _gameManager.PlayerCurrentHealth + "/" + _gameManager.MaxHealth;
            _lifeBAR.fillAmount = calc;

        }

        public void ShowPower(DecaTxt.DecaType type)
        {
          HidePower();

            switch (type)
            {
                case DecaTxt.DecaType.DecaKill:
                    Powers[0].SetActive(true);
                  
                    break;
                case DecaTxt.DecaType.DecaLoot:
                    Powers[1].SetActive(true);
                    Invoke("HidePower", _hidePowerTime);
                    
                    break;
                case DecaTxt.DecaType.DecaDestroy:
                    Powers[2].SetActive(true);
                    Invoke("HidePower", _hidePowerTime);
                        
                    break;
                case DecaTxt.DecaType.DecaGrowth:
                    Powers[3].SetActive(true);
                    _growthTurnTXT.text = "" + _gameManager.GrowthTIME+" TURN";
                    break;

            }

        }

        public void HidePower()
        {
            for (int i = 0; i < Powers.Length; i++) //remove all
            {
                Powers[i].SetActive(false);
            }
        }

    public void Init()
        {
            if (!_gameManager)
            {
                _gameManager = GameManager.Instance;
            }

            _gameOverPanel.SetActive(false);
            _levelUpPanel.SetActive(false);
            _startPanel.SetActive(false);
            _gamePanel.SetActive(true);
            RefreshALL();


        }

        public void RefreshALL()
        {
            ShowLife();
            RefreshKills();
            RefreshPickup();
            RefreshDestroy();
            RefreshXP();
            RefreshAttack();
            RefreshDefense();
            RefreshGrow();
            RefreshRoom();        }

        public void ShowLevelUp(bool show)
        {
            _levelUpPanel.SetActive(show);
            UnityEngine.Cursor.visible = true;
            UnityEngine.Cursor.lockState = CursorLockMode.None; 
        }

        public void RefreshGrow()
        {
            _growTXT.text= "Deca Growth "+_gameManager.AmountGrow+"/3"; 
        }

        public void RefreshRoom()
        {
            _roomTXT.text = "Room# " + _gameManager.RoomLevel;
        }
        public void ShowGameOver()
        {
            UnityEngine.Cursor.visible = true;
            UnityEngine.Cursor.lockState = CursorLockMode.None; 
            _gameOverPanel.SetActive(true);
            _gamePanel.SetActive(false);
            _dieByTXT.text = "<size=44><color=#A22532>Game Over!</color></size>\n\nYou traveled through " + _gameManager.RoomLevel +
                             " rooms. You lastest " + _gameManager.TurnManager.TurnCount + " turns, receiving " +
                             _gameManager.HitsTakenAmount + " hits, and attacking " + _gameManager.AttacksAmount +
                             " times!";
            _dieStatsTXT.text = "Player Level: "+ _gameManager.PlayerLevel+"\nKills: "+ _gameManager.KillAmount+"\nDestroys:"+ _gameManager.DestroyAmount+"\nPickup: "+ _gameManager.PickupAmount;
            _dieMegaGrowthTXT.text = "Mega Growth : "+ _gameManager.TotalGrow;
        }

        public void RestartGame()
        {
        
            _gameManager.StartNewGame();
        }

        public void ChoiceAddLife()
        {
            _gameManager.MaxHealth = _gameManager.MaxHealth + 10;
            _gameManager.ChangeLife(10);
            ShowLife();
           ShowLevelUp(false);
           UnityEngine.Cursor.visible = false;
           UnityEngine.Cursor.lockState = CursorLockMode.Locked; 
           _gameManager.BoardManager.Player.GoIdle();
        }

        public void ChoiceAddAttack()
        {
            _gameManager.PlayerAttack++;
            RefreshAttack();
            ShowLevelUp(false);
            UnityEngine.Cursor.visible = false;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked; 
            _gameManager.BoardManager.Player.GoIdle();
        }


      

        public void ChoiceAddDef()
        {
            _gameManager.PlayerDefense++;
            RefreshDefense();
            ShowLevelUp(false);
            UnityEngine.Cursor.visible = false;
            UnityEngine.Cursor.lockState = CursorLockMode.Locked; 
            _gameManager.BoardManager.Player.GoIdle();
            

        }

        void RefreshAttack()
        {

            _attackTXT.text = ""+_gameManager.PlayerAttack;
        }

        void RefreshDefense()
        {
            _defTXT.text = ""+_gameManager.PlayerDefense;
        }
    }
}
