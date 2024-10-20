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
    

    public void Send()
    {
        // Assign variables
        _testInt = Random.Range(0, 101);
        _testBool = true;
        _testFloat = Random.Range(0.0f, 10.0f);

        StartCoroutine(Post(_sessionID.ToString(), _testInt.ToString(), _testBool.ToString(), _testFloat.ToString()));
    }



private IEnumerator Post(string sessionID, string testInt, string testBool, string testFloat)
{
    // Create the form and enter responses
    WWWForm form = new WWWForm();
    form.AddField("entry.1289655618", sessionID);
    form.AddField("entry.1889672031", testInt);
    form.AddField("entry.976579775", testBool);
    form.AddField("entry.693963586", testFloat);

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
