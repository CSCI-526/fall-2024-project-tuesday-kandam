using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;

public class SendToGoogle : MonoBehaviour
{

    public string URL;

    public string entrySessionID;
    public string entryMostCommonSizeState;
    // public string entryRespawnCount1;
    // public string entryRespawnCount2;
    // public string entryRespawnCount3;
    // public string entryRespawnCount4;
    // public string entryRespawnCount5;
    public string entryMCSDataOverall;
    public string entryMCSDataPerCheckpoint;
    public string entryRespawnData;
    public string entryDiamondsCollected;
    public string entryHeatmapCoords;

    public bool debugEnabled = true;

    private long _sessionID;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }


    private void Awake()
    {
        // Assign sessionID to identify playtests
        _sessionID = System.DateTime.Now.Ticks;
        // Send();
    }


    public void Send(string mostCommonSizeState, string MCSDataOverall, string MCSDataPerCheckpoint, string RespawnCounts, string DiamondsCollected, string heatmapCoords)
    {

        if (debugEnabled)
        {

            WriteDataToFile(_sessionID.ToString(), mostCommonSizeState, MCSDataOverall, MCSDataPerCheckpoint, RespawnCounts, DiamondsCollected, heatmapCoords);
        }
        StartCoroutine(Post(_sessionID.ToString(), mostCommonSizeState, MCSDataOverall, MCSDataPerCheckpoint, RespawnCounts, DiamondsCollected, heatmapCoords));
    }



    private IEnumerator Post(string sessionID, string mostCommonSizeState, string MCSDataOverall, string MCSDataPerCheckpoint, string RespawnCounts, string DiamondsCollected, string heatmapCoords)
    {
        // Create the form and enter responses
        WWWForm form = new WWWForm();

        form.AddField(entrySessionID, sessionID); // sessionID field
        form.AddField(entryMostCommonSizeState, mostCommonSizeState); // most common size state
        form.AddField(entryMCSDataOverall, MCSDataOverall); // most common size state data overall
        form.AddField(entryMCSDataPerCheckpoint, MCSDataPerCheckpoint); // most common size state data per checkpoint
        form.AddField(entryRespawnData, RespawnCounts); // respawn counts
        form.AddField(entryDiamondsCollected, DiamondsCollected); // diamonds collected
        form.AddField(entryHeatmapCoords, heatmapCoords); // heatmap coordinates
        // Send responses and verify result
        using (UnityWebRequest www = UnityWebRequest.Post(URL, form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log("Form upload complete!");
            }
        }
    }
    private void WriteDataToFile(string sessionID, string mostCommonSizeState, string MCSDataOverall, string MCSDataPerCheckpoint, string RespawnCounts, string DiamondsCollected, string heatmapCoords)
    {
        string dateTime = System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
        string fileName = "GameDataLog_" + dateTime + ".txt";
        string logsDirectory = "Assets/Scripts/DebugLogs/";
        string filePath = logsDirectory + fileName;
        Debug.Log("Writing data to file: " + filePath);
        if (!System.IO.Directory.Exists(logsDirectory))
        {
            System.IO.Directory.CreateDirectory(logsDirectory);
        }

        // Prepare the data to write
        string dataToWrite = "Session ID: " + sessionID + "\n" +
                             "Scene Name: " + SceneManager.GetActiveScene().name + "\n" +
                             "Most Common Size State: " + mostCommonSizeState + "\n" +
                             "MCS Data Overall: " + MCSDataOverall + "\n" +
                             "MCS Data Per Checkpoint: " + MCSDataPerCheckpoint + "\n" +
                             "Respawn Counts: " + RespawnCounts + "\n" +
                             "Diamonds Collected: " + DiamondsCollected + "\n" +
                             "Heatmap Coordinates: " + heatmapCoords + "\n" +
                             "--------------------------------------------------\n";

        // Append the data to the file
        System.IO.File.AppendAllText(filePath, dataToWrite);

        Debug.Log("Data written to file: " + filePath);
    }




}
