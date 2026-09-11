using UnityEngine;

namespace Destrial
{

    public class FoodObject : CellObject
    {
        public int AmountGranted = 5;

        [SerializeField] AudioSource _audioSource;
        [SerializeField] AudioClip[] _audioCrunch;
        public override void PlayerEntered()
        {
            GameManager.Instance.ChangeLife(AmountGranted);
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