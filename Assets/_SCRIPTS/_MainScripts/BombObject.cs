using UnityEngine;

namespace Destrial
{

    public class BombObject : CellObject
    {
        public int AmountGranted = 20;

        [SerializeField] AudioSource _audioSource;
        [SerializeField] AudioClip[] _audioBoom;


        void Start()
        {
            PlayerEntered();
        }
        public override void PlayerEntered()
        {
            GameManager.Instance.ChangeLife(-AmountGranted);
            GameManager.Instance.AudioPickup(true);
            _audioSource.PlayOneShot(_audioBoom[Random.Range(0, _audioBoom.Length)], GameManager.Instance.sfxVolume);
            Invoke("DestroyMe", 1.5f);

        }
        
       

        void DestroyMe()
        {
            Destroy(gameObject);
            
        }

        public override void RatEntered()
        {
            Destroy(gameObject);
           
            GameManager.Instance.AudioPickup(false);
        }
    }

}