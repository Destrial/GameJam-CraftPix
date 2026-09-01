using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Destrial
{
    public class NoRotate : MonoBehaviour
    {
        private RectTransform rectTransform;
       // private Vector3 initialScale;

        void Start()
        {
            rectTransform = GetComponent<RectTransform>();
           // initialScale = rectTransform.localScale;
        }

        void LateUpdate()
        {
        /*
            // 1. Annule la rotation du parent
            rectTransform.rotation = Quaternion.identity;

            // 2. Annule le flip (échelle négative) du parent si nécessaire
            Vector3 parentRotation = transform.parent.rotation.eulerAngles;
            parentRotation=new Vector3(parentRotation.x, parentRotation.x, -parentRotation.z);
            rectTransform.rotation=Quaternion.Euler(parentRotation);
                */
        rectTransform.rotation = Quaternion.identity;
/*
        // 2. Annule le flip (échelle négative) du parent si nécessaire
        Vector3 parentScale = transform.parent != null ? transform.parent.lossyScale : Vector3.one;

        rectTransform.localScale = new Vector3(
            parentScale.x < 0 ? -initialScale.x : initialScale.x,
            parentScale.y < 0 ? -initialScale.y : initialScale.y,
            initialScale.z);
*/
        }
    }
    
}
