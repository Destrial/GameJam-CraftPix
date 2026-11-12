using System;
using UnityEngine;

namespace Destrial
{

    public class PoisonFood : CellObject
    {
        public int AmountGranted = 5;
        public int FoodLevel = 1;
        
        [Header("Poison Settings")]
        [Tooltip("Chance for the food to be poisoned (0.02 = 2%)")]
        [UnityEngine.Range(0f, 1f)]
        public float PoisonChance = 0.7f;
        
        [Tooltip("Color tint applied when the food is poisoned")]
        public Color PoisonColor = new Color(0.2f, 1f, 0.2f, 1f); // Green tint

        [SerializeField] AudioSource _audioSource;
        [SerializeField] AudioClip[] _audioCrunch;
        [SerializeField] private SpriteRenderer _spriteRenderer;

        private bool _isPoisoned;

        private void Awake()
        {
            // Auto-fetch SpriteRenderer if not explicitly assigned in Inspector
            if (_spriteRenderer == null)
            {
                _spriteRenderer = GetComponent<SpriteRenderer>();
            }
        }

        private void OnEnable()
        {
            // Determine if this food instance is poisoned when spawned/enabled
            _isPoisoned = UnityEngine.Random.value < PoisonChance;

            if (_isPoisoned && _spriteRenderer != null)
            {
                _spriteRenderer.color = PoisonColor;
            }
        }

        public override void PlayerEntered()
        {
            if (_isPoisoned)
            {
                // Apply poison penalty
                GameManager.Instance.ChangeLife(-Mathf.Abs(AmountGranted + (FoodLevel - 1) * 5));
                Debug.Log("Poisoned" + -Mathf.Abs(AmountGranted + (FoodLevel - 1) * 5));
            }
            else
            {
                // Normal food consumption
                GameManager.Instance.ChangeLife(AmountGranted + (FoodLevel - 1) * 5);
                Debug.Log("SAFE");
            }

            GameManager.Instance.AudioPickup(true);
            GameManager.Instance.AddPickup();
            Destroy(gameObject);

            //increase food
          
        }

        public override void RatEntered()
        {
            Destroy(gameObject);
           
            GameManager.Instance.AudioPickup(false);
        }
    }

}