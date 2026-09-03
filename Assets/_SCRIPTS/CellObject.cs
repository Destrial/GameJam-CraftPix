using UnityEngine;

namespace Destrial
{

    public class CellObject : MonoBehaviour
    {
        //Called when the player enter the cell in which that object is
        protected Vector2Int _cell;

        public virtual void Init(Vector2Int cell)
        {
            _cell = cell;
        }

        public virtual void PlayerEntered()
        {

        }

        public virtual bool PlayerWantsToEnter()
        {
            return true;
        }



    }
}
