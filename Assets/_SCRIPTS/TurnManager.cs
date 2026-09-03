using UnityEngine;

namespace Destrial
{

    public class TurnManager
    {
        public event System.Action OnTick;
        private int _turnCount;

        public TurnManager() //constructeur
        {
            _turnCount = 1;
        }

        public void Tick()
        {
            _turnCount += 1;
            OnTick?.Invoke();



            Debug.Log("Current turn count : " + _turnCount);
        }
    }
}
