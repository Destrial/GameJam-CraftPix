using NUnit.Framework.Constraints;
using UnityEngine;

namespace Destrial
{

    public class BombObject : CellObject
    {
        public int AmountGranted = 20;
        public int BombLevel = 1;
        [SerializeField] AudioSource _audioSource;
        [SerializeField] AudioClip[] _audioBoom;
        private bool _isBoom;

        void Start()
        {
            _isBoom = false;
            PlayerEntered();
        }
        public override void PlayerEntered()
        {
            if (_isBoom) return;
            _isBoom = true;
            Vector2Int modDir = _cell - GameManager.Instance.BoardManager.Player.CellPosition;
            GameManager.Instance.BoardManager.Player.GetHurt(AmountGranted+(BombLevel-1)*5,modDir); // CHANGE UI            GameManager.Instance.ChangeLife(-AmountGranted);
         
            _audioSource.PlayOneShot(_audioBoom[Random.Range(0, _audioBoom.Length)], GameManager.Instance.sfxVolume);
            GameManager.Instance.BoardManager.Player.GoWait();
            Invoke("DestroyMe", 1.5f);

        }
        
       

        void DestroyMe()
        {
            Destroy(gameObject);
            GameManager.Instance.BoardManager.Player.GoIdle();
        }

        public override void RatEntered()
        {
            Destroy(gameObject);
           
            GameManager.Instance.AudioPickup(false);
        }
    }

}