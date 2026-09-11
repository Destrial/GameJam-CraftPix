using UnityEngine;

namespace Destrial
{

    public class TurnManager
    {
        public event System.Action OnTick;

        public event System.Action OnMobDie;
        public event System.Action OnPickup;
        public event System.Action OnDestroy;
        
        public event System.Action OnLevelUp;
        
        
        public int TurnCount;

        public TurnManager() //constructeur
        {
            TurnCount = 1;
        }

        public void DestroyWall()
        {
            OnDestroy?.Invoke();
        }

        
        public void LevelUp()
        {
            OnLevelUp?.Invoke();
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
            TurnCount += 1;
            OnTick?.Invoke();



          //  Debug.Log("Current turn count : " + _turnCount);
        }
    }
}
