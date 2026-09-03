using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Destrial
{
    public class AppearSlowly : MonoBehaviour
    {
        [SerializeField] private Image _image;
        [SerializeField] private float _speed=1.0f;
        [SerializeField] private Color _startColor;
        [SerializeField] private Color _endColor;
        // Start is called before the first frame update
        void OnEnable()
        {
            _image.color = _startColor;
            _image.DOColor(_endColor, _speed).SetEase(Ease.InOutCubic).SetDelay(1.64f);
          
        }

        void OnDisable()
        {
            _image.color = _startColor;

        }
       
    }
}
