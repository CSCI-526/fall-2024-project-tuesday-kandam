using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneScript : MonoBehaviour
{
    public void OnPlayButton()
    {
        SceneManager.LoadScene("1. LevelSelectionScene");
    }
}
