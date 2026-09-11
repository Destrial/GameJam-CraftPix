using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Destrial
{
    public class UIAutoSelect : MonoBehaviour
    {
   
   
        [SerializeField] private Button firstSelectedButton;

        void OnEnable()
        {
            // Check if EventSystem exists in the scene
            if (EventSystem.current != null && firstSelectedButton != null)
            {
                // Clear current selection first to avoid bugs
                EventSystem.current.SetSelectedGameObject(null);
            
                // Select the new button
                firstSelectedButton.Select();
            }
        }
    }
    
}
