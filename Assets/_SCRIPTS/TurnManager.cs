using UnityEngine;

namespace Destrial
{

    public class TurnManager
    {
        public event System.Action OnTick;
        private int m_TurnCount;

        public TurnManager() //constructeur
        {
            m_TurnCount = 1;
        }

        public void Tick()
        {
            m_TurnCount += 1;
            OnTick?.Invoke();



            Debug.Log("Current turn count : " + m_TurnCount);
        }
    }
}
