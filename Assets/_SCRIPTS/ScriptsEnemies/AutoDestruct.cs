using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Destrial
{
    public class AutoDestruct : MonoBehaviour
    {
        private Animator _animator;
        
        
        [SerializeField] private float _timeToDestruct;
        
        
        // Start is called before the first frame update
        void Start()
        {
            _animator = GetComponent<Animator>();
            _animator.SetBool("RatDied", true);
            
            Invoke("Destruct", _timeToDestruct);
        }

        void Destruct()
        {
            Destroy(gameObject);    
        }
       
    }
}
