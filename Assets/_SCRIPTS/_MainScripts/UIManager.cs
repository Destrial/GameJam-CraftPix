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
        [SerializeField] private TextMeshProUGUI _dieByTXT;
        [SerializeField] private TextMeshProUGUI _dieStatsTXT;
        [SerializeField] private TextMeshProUGUI _dieMegaGrowthTXT;

        [SerializeField] private TextMeshProUGUI _lifeTXT;
        [SerializeField] private TextMeshProUGUI _attackTXT;
        [SerializeField] private TextMeshProUGUI _defTXT;
        [SerializeField] private TextMeshProUGUI _levelTXT;

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
        [SerializeField]
        GameManager _gameManager;



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
        }



        public void ShowLife()
        {
            _lifeTXT.text = "" + _gameManager.PlayerCurrentHealth;
            _lifeBAR.fillAmount = _gameManager.PlayerCurrentHealth/_gameManager.MaxHealth ;
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
            ShowLife();
            RefreshKills();
            RefreshPickup();
            RefreshDestroy();
            RefreshXP();
            RefreshAttack();
            RefreshDefense();
            RefreshGrow();



        }

        public void RefreshGrow()
        {
            //ui    
        }


        public void ShowGameOver()
        {
            _gameOverPanel.SetActive(true);
            _gamePanel.SetActive(false);
            _dieByTXT.text = "<size=32>Game Over!</size>\nYou are level " + _gameManager.CurrentLevel +
                             " traveled through " + _gameManager.CurrentLevel +
                             " levels. You lastest " + _gameManager.TurnManager.TurnCount + " turns, receiving " +
                             _gameManager.HitsTakenAmount + " hits, and attacking " + _gameManager.AttacksAmount +
                             " times!";
        }

        public void RestartGame()
        {
            _gameManager.StartNewGame();
        }

        public void ChoiceAddLife()
        {
            _gameManager.MaxHealth = _gameManager.MaxHealth + 50;
            _gameManager.ChangeLife(50);
            ShowLife();
        }

        public void ChoiceAddAttack()
        {
            _gameManager.PlayerAttack++;
            RefreshAttack();
        }



        public void ChoiceAddDef()
        {
            _gameManager.PlayerDefense++;
            RefreshDefense();

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
