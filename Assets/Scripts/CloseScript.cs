using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;  // Import this to use UI elements like Button

public class CloseScript : MonoBehaviour
{
    public GameObject panel;  // Reference to the panel to be closed
    public Button closeButton;  // Reference to the close button
    public bool activeOnStart = true;

    void Start()
    {
        // Add a listener to the close button
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(ClosePanel);
        }

        // Optionally, ensure the panel is visible at the start
        if (panel != null)
        {
            panel.SetActive(activeOnStart);
        }
    }

    // Method to close the panel
    public void ClosePanel()
    {
        if (panel != null)
        {
            panel.SetActive(false);  // Hide the panel
        }
    }
}
