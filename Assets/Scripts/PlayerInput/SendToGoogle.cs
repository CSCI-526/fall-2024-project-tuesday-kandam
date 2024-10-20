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


    public void Send(string mostCommonSizeState, string respawnCount1, string respawnCount2)
    {
        StartCoroutine(Post(_sessionID.ToString(), mostCommonSizeState, respawnCount1, respawnCount2));
    }



    private IEnumerator Post(string sessionID, string mostCommonSizeState, string respawnCount1, string respawnCount2)
    {
        // Create the form and enter responses
        WWWForm form = new WWWForm();
        form.AddField("entry.1289655618", sessionID); // sessionID field
        form.AddField("entry.1889672031", mostCommonSizeState); // most common size state
        form.AddField("entry.976579775", respawnCount1); // respawn count for checkpoint 1
        form.AddField("entry.693963586", respawnCount2); // respawn count for checkpoint 2

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
