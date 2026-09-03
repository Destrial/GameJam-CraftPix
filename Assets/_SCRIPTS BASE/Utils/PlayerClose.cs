using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Destrial
{
    public class PlayerClose : MonoBehaviour
    {
        [SerializeField] GameObject icon;
        [SerializeField] 
        private float _testTime=1.0f;
        [SerializeField] 
        public float TestRadius=2.0f;
        private float _actualTime;

        [SerializeField] private LayerMask _playerMask;
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(GoStartTimer());
        }


        IEnumerator GoStartTimer()
        {
            while (true)
            {
                WaitForSeconds wait = new WaitForSeconds(_testTime);
                TestPlayerProximity();
                yield return wait;
            }
            
        }

        void TestPlayerProximity()
        {
            PlayerManagerBase player=GetClosestPlayerInRadius(TestRadius);
        
            if (player!=null)
            {
                icon.SetActive(true);
            }
            else
            {
                icon.SetActive(false);
            }
        }
        
        
        private PlayerManagerBase GetClosestPlayerInRadius(float radius)
        {
            // Find all 2D colliders inside the circle around the player (using _playerMask if set)
            Collider2D[] hits = _playerMask.value != 0
                ? Physics2D.OverlapCircleAll(transform.position, radius, _playerMask)
                : Physics2D.OverlapCircleAll(transform.position, radius);

            PlayerManagerBase closestRat = null;
            float minSqrDistance = Mathf.Infinity;
            Vector2 playerPosition = transform.position;

            foreach (Collider2D hit in hits)
            {
                // Check if the collider or its parent belongs to a RatController2D
                PlayerManagerBase rat = hit.GetComponentInParent<PlayerManagerBase>();
                if (rat != null)
                {
                    float sqrDistance = ((Vector2)rat.transform.position - playerPosition).sqrMagnitude;
                    if (sqrDistance < minSqrDistance)
                    {
                        minSqrDistance = sqrDistance;
                        closestRat = rat;
                    }
                }
            }

            return closestRat;
        }
       
    }
}
