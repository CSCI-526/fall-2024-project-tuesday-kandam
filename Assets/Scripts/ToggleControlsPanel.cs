using UnityEngine;
using UnityEngine.UI;

public class ToggleControlsPanel : MonoBehaviour
{
    public GameObject panel; // Reference to the panel that will be toggled
    public Button toggleButton; // Reference to the button

    void Start()
    {
        

        // Disable button navigation to prevent space/enter triggering
        if (toggleButton != null)
        {
            toggleButton.onClick.AddListener(TogglePanel);

            // Disable navigation so the button doesn't respond to space/enter
            Navigation noNavigation = new Navigation();
            noNavigation.mode = Navigation.Mode.None;
            toggleButton.navigation = noNavigation;
        }
        else
        {
            Debug.LogError("Button not assigned!");
        }
    }

    // Method to toggle the panel's visibility
    public void TogglePanel()
    {
        if (panel != null)
        {
            // Toggle panel active state
            panel.SetActive(!panel.activeSelf);
        }
        else
        {
            Debug.LogError("Panel not assigned!");
        }
    }
}
