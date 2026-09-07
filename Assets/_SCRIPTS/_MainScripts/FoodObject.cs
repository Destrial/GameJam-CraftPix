using UnityEngine;

namespace Destrial
{

    public class FoodObject : CellObject
    {
        public int AmountGranted = 10;

        [SerializeField] AudioSource _audioSource;
        [SerializeField] AudioClip[] _audioCrunch;
        public override void PlayerEntered()
        {
            GameManager.Instance.ChangeLife(AmountGranted);
            GameManager.Instance.AudioPickup(true);

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