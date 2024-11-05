using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SendToGoogle : MonoBehaviour
{

    public string URL;

    public string entrySessionID;
    public string entryMostCommonSizeState;
    public string entryRespawnCount1;
    public string entryRespawnCount2;
    public string entryRespawnCount3;
    public string entryRespawnCount4;
    public string entryRespawnCount5;
    public string entryHeatmapCoords;

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


    public void Send(string mostCommonSizeState, string respawnCount1, string respawnCount2, string respawnCount3, string respawnCount4, string respawnCount5, string heatmapCoords)
    {
        StartCoroutine(Post(_sessionID.ToString(), mostCommonSizeState, respawnCount1, respawnCount2, respawnCount3, respawnCount4, respawnCount5, heatmapCoords));
    }



    private IEnumerator Post(string sessionID, string mostCommonSizeState, string respawnCount1, string respawnCount2, string respawnCount3, string respawnCount4, string respawnCount5, string heatmapCoords)
    {
        // Create the form and enter responses
        WWWForm form = new WWWForm();
        // form.AddField("entry.1289655618", sessionID); // sessionID field
        // form.AddField("entry.1889672031", mostCommonSizeState); // most common size state
        // form.AddField("entry.976579775", respawnCount1); // respawn count for checkpoint 1
        // form.AddField("entry.693963586", respawnCount2); // respawn count for checkpoint 2
        // form.AddField("entry.527034931", respawnCount3); // respawn count for checkpoint 3
        // form.AddField("entry.1977834576", respawnCount4); // respawn count for checkpoint 4
        // form.AddField("entry.2003516449", respawnCount5); // respawn count for checkpoint 5
        // form.AddField("entry.776173049", heatmapCoords); // heatmap coordinates

        form.AddField(entrySessionID, sessionID); // sessionID field
        form.AddField(entryMostCommonSizeState, mostCommonSizeState); // most common size state
        if (entryRespawnCount1 != null)
        {
            Debug.Log("entryRespawnCount1 is not null, adding an entry");
            form.AddField(entryRespawnCount1, respawnCount1); // respawn count for checkpoint 1
        }
        if (entryRespawnCount2 != null)
        {
            Debug.Log("entryRespawnCount2 is not null, adding an entry");
            form.AddField(entryRespawnCount2, respawnCount2); // respawn count for checkpoint 2
        }
        if (entryRespawnCount3 != null)
        {
            Debug.Log("entryRespawnCount3 is not null, adding an entry");
            form.AddField(entryRespawnCount3, respawnCount3); // respawn count for checkpoint 3
        }
        if (entryRespawnCount4 != null)
        {
            Debug.Log("entryRespawnCount4 is not null, adding an entry");
            form.AddField(entryRespawnCount4, respawnCount4); // respawn count for checkpoint 4
        }
        if (entryRespawnCount5 != null)
        {
            Debug.Log("entryRespawnCount5 is not null, adding an entry");
            form.AddField(entryRespawnCount5, respawnCount5); // respawn count for checkpoint 5
        }
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


}
