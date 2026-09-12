using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace Destrial
{
    public class DecaTxt : MonoBehaviour
    {
        [SerializeField] private TextMeshPro decaText;
    
        // ENUM
        public enum DecaType
        {
            DecaKill,
            DecaLoot,
            DecaDestroy,
            DecaLevelUP,
            
            DecaGrowth,
        }
        
        
        // Configuration
        [Header("Configuration")]
        [SerializeField] private float moveDistance = 0.15f;
        [SerializeField] private float duration = 0.5f;
        [SerializeField] private float punchScaleAmount = 0.6f;
        [SerializeField]  private Color DecaColor = Color.white;
        [SerializeField]  private Color DecaGrowthColor = Color.green;
        
        
        
        //USE A INSPIRATION
        private Sequence textSequence;
        
        public void Initialize(DecaType deca)
        {
            /*
            if (displayedText = )   //CHANGE ENUM
            {
                decaText.color = DecaColor;
            }
            else
            {
                decaText.color = DecaGrowthColor;
            }
            */

            if (textSequence != null && textSequence.IsActive() && textSequence.IsPlaying())
            {
                textSequence.Kill(true); // 'true' forces the old tween to instantly complete its states
            }
            transform.localPosition=Vector3.zero;
            
            
            // 1. Set text and reset layout state
            gameObject.SetActive(true);
            decaText.text = deca.ToString();  //CHANGE ENUM
            decaText.alpha = 1f;
            transform.localScale = Vector3.one;

            
            // clean, killable sequence
         
            textSequence = DOTween.Sequence();
            // 2. Snappy visual pop (Punch Scale)
            textSequence.Append(transform.DOPunchScale(Vector3.one * punchScaleAmount, 0.15f, 10, 1));

            // 3. Smooth upward drift (Starts instantly with the punch)
            textSequence.Join(transform.DOLocalMoveY(transform.localPosition.y + moveDistance, duration)
                .SetEase(Ease.OutCubic));

            // 4. Snappy fade out near the end of the drift
            textSequence.Join(decaText.DOFade(0f, duration * 0.4f)
                .SetDelay(duration * 0.6f));

            // 5. Cleanup when done
            textSequence.OnComplete(() => gameObject.SetActive(false));
        }
        
        
        
    }
    
}
