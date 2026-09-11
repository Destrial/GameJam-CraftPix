using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Destrial
{
    public class PsychedelicUI : MonoBehaviour
    {
        private Image uiImage;
        private Tween psychedelicTween;
        [Header("Configuration")]
        [SerializeField] private float speedMultiplier = 10f; // Plus ce chiffre est haut, plus ça flashe vite
        [Header("Temps")]
        [SerializeField] private float durationBeforeStop = 4f; // S'arrête après X secondes
        
        void OnEnable()
        {
            uiImage = GetComponent<Image>();
            StartPsychedelicEffect();
        }

        void StartPsychedelicEffect()
        {
            float hueProgress = 0f;

            // 1. Lance l'effet de flash infini
            psychedelicTween = DOTween.To(() => hueProgress, x => hueProgress = x, 1f, 6f)
                .SetEase(Ease.Linear)
                .SetLoops(-1, LoopType.Incremental) 
                .OnUpdate(() =>
                {
                    float currentHue = (hueProgress * speedMultiplier) % 1f; 
                    uiImage.color = Color.HSVToRGB(currentHue, 1f, 1f);
                });

            // 2. Planifie l'arrêt exact du flash après 'durationBeforeStop' secondes
            DOVirtual.DelayedCall(durationBeforeStop, () =>
            {
                StopPsychedelicEffect();
            });
        }

        void StopPsychedelicEffect()
        {
            if (psychedelicTween != null && psychedelicTween.IsActive())
            {
                psychedelicTween.Kill();
                // Optionnel : remettez ici une couleur par défaut si vous ne voulez pas figer l'image sur la couleur du flash
            }
        }

        void OnDisable()
        {
            // Always kill your loops when the object is destroyed to prevent memory leaks
            psychedelicTween?.Kill();
        }
    }
}
