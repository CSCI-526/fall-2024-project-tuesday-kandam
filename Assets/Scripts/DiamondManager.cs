using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DiamondManager : MonoBehaviour
{
    public int diamondCount;
    public TextMeshProUGUI diamondText;
    private int totalDiamonds;


    void Start()
    {
        totalDiamonds = GameObject.FindGameObjectsWithTag("Diamond_Tag").Length -1;
    }

    void Update()
    { 
        diamondText.text = "Diamond Count: " + diamondCount.ToString() + "/" + totalDiamonds;
    }
}
