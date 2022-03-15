using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.Events;
public class GenericAd : MonoBehaviour
{
    [SerializeField] public string gameID = "My game id";
    [SerializeField] public string adID = "My_AD";

    public UnityEvent onFinished = null;
    public UnityEvent onSkipped = null;
    public UnityEvent onFailed = null;
    
    
    private void Start()
    {
        if (Application.platform == RuntimePlatform.WindowsEditor)
            Advertisement.Initialize(gameID, true);
        else if (Application.platform == RuntimePlatform.Android)
            Advertisement.Initialize(gameID, false);
    }

    public void ShowAd()
    {
        StartCoroutine(WaitToShowAd());
    }

    IEnumerator WaitToShowAd()
    {
        while (!Advertisement.IsReady(adID)) 
        {
            Debug.Log(gameObject.name + ": Waiting to show ad...");
            yield return new WaitForEndOfFrame();
        }
        

        Debug.Log(gameObject.name + ": Showing ad.");
        ShowOptions options = new ShowOptions();
        options.resultCallback = Result;
        Advertisement.Show(adID, options);
    }

    void Result(ShowResult _result)
    {
        switch (_result)
        {
            case ShowResult.Failed:
                Debug.Log(gameObject.name + ": Failed ad.");
                onFailed?.Invoke();
                break;
            case ShowResult.Skipped:
                Debug.Log(gameObject.name + ": Skipped ad.");
                onSkipped?.Invoke();
                break;
            case ShowResult.Finished:
                Debug.Log(gameObject.name + ": Finished ad.");
                onFinished?.Invoke();
                break;
        }
    }
}
