using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class AutoSelectButton : MonoBehaviour
{
    // Drag your UI button into this field in the Unity Inspector
    [SerializeField] private Button firstSelectedButton;

    private void OnEnable()
    {
        if (firstSelectedButton != null)
        {
            // Clear current selection first to ensure a fresh highlight state
            EventSystem.current.SetSelectedGameObject(null);
            
            // Set the new selected button
            EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
        }
    }
}