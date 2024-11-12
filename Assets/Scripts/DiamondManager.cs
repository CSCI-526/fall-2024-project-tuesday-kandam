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
        totalDiamonds = GameObject.FindGameObjectsWithTag("Diamond_Tag").Length;
        Debug.Log("Total Diamonds in Level: " + totalDiamonds);
    }

    void Update()
    { 
        diamondText.text = "Diamond Count: " + diamondCount.ToString() + "/" + totalDiamonds;
    }

    public int GetTotalDiamonds()
    {
        return totalDiamonds;
    }
}
