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
        
        [SerializeField] GameObject _gameOverPanel;
        [SerializeField] GameObject _gamePanel;
        [SerializeField] private TextMeshProUGUI _dieByTXT;
        [SerializeField] private TextMeshProUGUI _dieStatsTXT;
        [SerializeField] private TextMeshProUGUI _dieMegaGrowthTXT;
     
        [SerializeField] private TextMeshProUGUI _lifeTXT;
        
        
      

        public void RefreshKills()
        {
            
        }

        public void RefreshPickup()
        {
            
        }

        public void RefreshDestroy()
        {
            
        }

        public void ShowLife()
        {
            _lifeTXT.text = "LIFE: " +  GameManager.Instance.CurrentHealthAmount;
        }

        public void Init()
        {
            _gameOverPanel.SetActive(false);
            _gamePanel.SetActive(true);
            ShowLife();
        }


        public void ShowGameOver()
        {
            _gameOverPanel.SetActive(true);
            _gamePanel.SetActive(false);
            _dieByTXT.text = "<size=32>Game Over!</size>\n\nYou traveled through\n\n " + GameManager.Instance.CurrentLevel +
                             " levels";
        }

        public void RestartGame()
        {
            GameManager.Instance.StartNewGame();
        }
    }
}
