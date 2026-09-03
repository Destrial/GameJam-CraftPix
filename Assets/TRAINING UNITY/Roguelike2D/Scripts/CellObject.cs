using UnityEngine;

public class CellObject : MonoBehaviour
{
    //Called when the player enter the cell in which that object is
    protected Vector2Int m_Cell;

    public virtual void Init(Vector2Int cell)
    {
        m_Cell = cell;
    }
    
    public virtual void PlayerEntered()
    {
      
    }
    
    public virtual bool PlayerWantsToEnter()
    {
        return true;
    }
    
    
    
}
