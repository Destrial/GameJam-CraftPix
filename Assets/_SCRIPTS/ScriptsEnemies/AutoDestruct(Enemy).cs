using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Destrial
{
    public class AutoDestruct_Enemy : MonoBehaviour
    {
        private Animator _animator;
        
        
        [SerializeField] private float _timeToDestruct;
        [SerializeField] Enemy.EnemyType myType;
        
        // Start is called before the first frame update
        void Start()
        {
            _animator = GetComponent<Animator>();
            switch (myType)
            {
                case Enemy.EnemyType.Rat:
                    _animator.SetBool("RatDied", true);
                    break;
                case Enemy.EnemyType.Goblin:
                    _animator.SetTrigger("BasicGobDied");
                    break;
            }
           
            
            Invoke("Destruct", _timeToDestruct);
        }

        void Destruct()
        {
            Destroy(gameObject);    
        }
       
    }
}
