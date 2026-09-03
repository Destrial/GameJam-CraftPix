using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private BoardManager m_Board;
    public Vector2Int m_CellPosition;
    private Animator m_Animator;
    private bool m_IsGameOver;
    
    private bool m_IsMoving;
    private Vector3 m_MoveTarget;
    [SerializeField] float MoveSpeed = 1;
    public BoardManager.CellData Cell;
    
    private void Awake()
    {
        m_Animator = GetComponent<Animator>();
    }
    
    public void Spawn(BoardManager boardManager, Vector2Int cell)
    {
        m_Board = boardManager;
        m_CellPosition = cell;

        //let's move to the right position...
        transform.position = m_Board.CellToWorld(cell);
        Cell = m_Board.GetCellData(cell);
    }
    
    public void MoveTo(Vector2Int cell, bool immediate)
    {
        m_CellPosition = cell;

        if (immediate)
        {
            m_IsMoving = false;
            transform.position = m_Board.CellToWorld(m_CellPosition);
           

        }
        else
        {
            m_IsMoving = true;
            m_MoveTarget = m_Board.CellToWorld(m_CellPosition);
        }
        Cell = m_Board.GetCellData(cell);
        m_Animator.SetBool("Moving", m_IsMoving);
    }
    
    public void Init()
    {
        m_IsMoving = false;
        m_Animator.SetBool("Moving", false);
        m_IsGameOver = false;
    }


    public void Update()
    {
        if (m_IsGameOver)
        {
            if (Keyboard.current.enterKey.wasPressedThisFrame)
            {
                GameManager.Instance.StartNewGame();
            }
            return;
        }
        Vector2Int newCellTarget = m_CellPosition;
        bool hasMoved = false;

        if(Keyboard.current.upArrowKey.wasPressedThisFrame)
        {
            newCellTarget.y += 1;
            hasMoved = true;
        }
        else if(Keyboard.current.downArrowKey.wasPressedThisFrame)
        {
            newCellTarget.y -= 1;
            hasMoved = true;
        }
        else if (Keyboard.current.rightArrowKey.wasPressedThisFrame)
        {
            newCellTarget.x += 1;
            hasMoved = true;
        }
        else if (Keyboard.current.leftArrowKey.wasPressedThisFrame)
        {
            newCellTarget.x -= 1;
            hasMoved = true;
        }

        if(hasMoved)
        {
           
            //check if the new position is passable, then move there if it is.
            BoardManager.CellData cellData = m_Board.GetCellData(newCellTarget);

            if(cellData != null && cellData.Passable)
            {
                GameManager.Instance.TurnManager.Tick();

                if (cellData.ContainedObject == null)
                {
                    MoveTo(newCellTarget,false);
                }
                else if(cellData.ContainedObject.PlayerWantsToEnter())
                {
                    MoveTo(newCellTarget,false);
                    //Call PlayerEntered AFTER moving the player! Otherwise not in cell yet
                    cellData.ContainedObject.PlayerEntered();
                }
            }
        }
        
        if (m_IsMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, m_MoveTarget, MoveSpeed * Time.deltaTime);
          
            if (transform.position == m_MoveTarget)
            {
                m_IsMoving = false;
                m_Animator.SetBool("Moving", false);
                var cellData = m_Board.GetCellData(m_CellPosition);
                if(cellData.ContainedObject != null)
                    cellData.ContainedObject.PlayerEntered();
            }

            return;
        }
    }
    
    public void GameOver()
    {
        m_IsGameOver = true;
    }
}
