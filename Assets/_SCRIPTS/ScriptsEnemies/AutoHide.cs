using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

namespace Destrial
{
    public class AutoHide : MonoBehaviour
    {
       
        SpriteRenderer _spriteRenderer;
        
        [SerializeField] private float _timeToHide;
      
        
        // Start is called before the first frame update
        void OnEnable()
        {
            _spriteRenderer.color = Color.white;
            _spriteRenderer.DOColor(Color.clear, 1f).SetEase(Ease.InOutCubic).SetDelay(_timeToHide-1f);
            
            Invoke("Hide", _timeToHide);
        }

        void Hide()
        {
            gameObject.SetActive(false);    
        }
       
    }
}
