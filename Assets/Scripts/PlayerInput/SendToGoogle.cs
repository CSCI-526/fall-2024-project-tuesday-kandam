using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SendToGoogle : MonoBehaviour
{

    [SerializeField] private string URL;

    private long _sessionID;
    private int _testInt;
    private bool _testBool;
    private float _testFloat;
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


    public void Send(string mostCommonSizeState, string respawnCount1, string respawnCount2, string respawnCount3, string respawnCount4, string respawnCount5)
    {
        StartCoroutine(Post(_sessionID.ToString(), mostCommonSizeState, respawnCount1, respawnCount2, respawnCount3, respawnCount4, respawnCount5));
    }



    private IEnumerator Post(string sessionID, string mostCommonSizeState, string respawnCount1, string respawnCount2, string respawnCount3, string respawnCount4, string respawnCount5)
    {
        // Create the form and enter responses
        WWWForm form = new WWWForm();
        form.AddField("entry.1289655618", sessionID); // sessionID field
        form.AddField("entry.1889672031", mostCommonSizeState); // most common size state
        form.AddField("entry.976579775", respawnCount1); // respawn count for checkpoint 1
        form.AddField("entry.693963586", respawnCount2); // respawn count for checkpoint 2
        form.AddField("entry.527034931", respawnCount3); // respawn count for checkpoint 3
        form.AddField("entry.1977834576", respawnCount4); // respawn count for checkpoint 4
        form.AddField("entry.2003516449", respawnCount5); // respawn count for checkpoint 5
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
