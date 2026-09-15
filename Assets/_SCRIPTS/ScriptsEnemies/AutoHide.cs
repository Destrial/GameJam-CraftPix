using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Destrial
{
    public class AutoHide : MonoBehaviour
    {
       
        
        
        [SerializeField] private float _timeToHide;
      
        
        // Start is called before the first frame update
        void OnEnable()
        {
          
           
            
            Invoke("Hide", _timeToHide);
        }

        void Hide()
        {
            gameObject.SetActive(false);    
        }
       
    }
}
