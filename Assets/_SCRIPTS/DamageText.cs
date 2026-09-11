using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;

namespace Destrial
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField] private TextMeshPro damageText;
    
        // Configuration
        [Header("Settings")]
        [SerializeField] private float moveDistance = 0.13f;
        [SerializeField] private float duration = 0.25f;
        [SerializeField] private float punchScaleAmount = 0.3f;
        [SerializeField]  private Color damageColor = Color.red;
        [SerializeField]  private Color healColor = Color.green;
        private Sequence damageSequence;
        
        public void Initialize(int damageAmount)
        {
            if (damageAmount < 0)
            {
                damageText.color = damageColor;
            }
            else
            {
                damageText.color = healColor;
            }

            if (damageSequence != null && damageSequence.IsActive() && damageSequence.IsPlaying())
            {
                damageSequence.Kill(true); // 'true' forces the old tween to instantly complete its states
            }
            transform.localPosition=Vector3.zero;
            
            // 1. Set text and reset layout state
            gameObject.SetActive(true);
            damageText.text = damageAmount.ToString();
            damageText.alpha = 1f;
            transform.localScale = Vector3.one;

            // Create a clean, killable sequence
         
            damageSequence = DOTween.Sequence();
            // 2. Snappy visual pop (Punch Scale)
            damageSequence.Append(transform.DOPunchScale(Vector3.one * punchScaleAmount, 0.15f, 10, 1));

            // 3. Smooth upward drift (Starts instantly with the punch)
            damageSequence.Join(transform.DOLocalMoveY(transform.localPosition.y + moveDistance, duration)
                .SetEase(Ease.OutCubic));

            // 4. Snappy fade out near the end of the drift
            damageSequence.Join(damageText.DOFade(0f, duration * 0.4f)
                .SetDelay(duration * 0.6f));

            // 5. Cleanup when done
            damageSequence.OnComplete(() => gameObject.SetActive(false));
        }
    }
}
