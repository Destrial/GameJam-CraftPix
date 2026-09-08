using UnityEngine;

namespace Destrial
{

    public class TurnManager
    {
        public event System.Action OnTick;

        public event System.Action OnMobDie;
        public event System.Action OnPickup;
        public event System.Action OnDestroy;
        
        
        private int _turnCount;

        public TurnManager() //constructeur
        {
            _turnCount = 1;
        }

        public void DestroyWall()
        {
            OnDestroy?.Invoke();
        }

        public void MobDie()
        {
            OnMobDie?.Invoke();
        }

        public void Pickup()
        {
            OnPickup?.Invoke();
        }

        public void Tick()
        {
            _turnCount += 1;
            OnTick?.Invoke();



          //  Debug.Log("Current turn count : " + _turnCount);
        }
    }
}
