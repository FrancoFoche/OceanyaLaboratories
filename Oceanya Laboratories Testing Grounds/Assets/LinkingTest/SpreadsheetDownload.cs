using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;

public class SpreadsheetDownload : MonoBehaviour
{
    public const string spreadsheetID = "1Z7WgIPyjJTTlP46cvcOzgV90WIyGRmcH6AC-amhTKNs";
    public const string sheetID = "1742534412";
    [HideInInspector]
    public const string url = "http://docs.google.com/spreadsheets/d/" + spreadsheetID + "/export?format=csv&gid=" + sheetID;
    public TextMeshProUGUI textOutput;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Started");
        StartCoroutine(Download());
    }

    IEnumerator Download()
    {
        UnityWebRequest request = UnityWebRequest.Get("https://docs.google.com/spreadsheets/d/1Z7WgIPyjJTTlP46cvcOzgV90WIyGRmcH6AC-amhTKNs");

        yield return request.SendWebRequest();

        if (request.isNetworkError)
        {
            Debug.LogError("Well shit. Download error: " + request.error);
        }
        else
        {
            Debug.Log("Set text");
            textOutput.text = request.downloadHandler.text;
        }

        request.Dispose();
    }
}
