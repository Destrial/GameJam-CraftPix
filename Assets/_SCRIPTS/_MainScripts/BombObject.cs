using UnityEngine;

namespace Destrial
{

    public class BombObject : CellObject
    {
        public int AmountGranted = 20;

        [SerializeField] AudioSource _audioSource;
        [SerializeField] AudioClip[] _audioBoom;
        public override void PlayerEntered()
        {
            GameManager.Instance.ChangeLife(-AmountGranted);
            GameManager.Instance.AudioPickup(true);
           
            
            Destroy(gameObject);

            //increase food
          
        }
        
        public override bool PlayerWantsToEnter()   
        {
            GameManager.Instance.ChangeLife(-AmountGranted);
            GameManager.Instance.AudioPickup(true);
           
            Destroy(gameObject);


            return false;

        }

        public override void RatEntered()
        {
            Destroy(gameObject);
           
            GameManager.Instance.AudioPickup(false);
        }
    }

}