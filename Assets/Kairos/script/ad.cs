using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

public class ad : MonoBehaviour
{
    saveLoad SaveLoad;
    public bool status = false;
    public bool isDisconnected;
    private void Awake()
    {
        DontDestroyOnLoad(this);
    }
    public void Start()
    {
        SaveLoad = GameObject.Find("SaveLoad").GetComponent<saveLoad>();
        if (Advertisement.isSupported)
        {
            if(Application.platform == RuntimePlatform.Android)
            {
                Advertisement.Initialize("1313182", false);
            }
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
                Advertisement.Initialize("1313183", false);
            }
        }
    }
    private void Update()
    {
        if (Advertisement.isSupported && !Advertisement.isInitialized)
        {
            if (Advertisement.isSupported)
            {
                if (Application.platform == RuntimePlatform.Android)
                {
                    Advertisement.Initialize("1313182", false);
                    isDisconnected = false;
                }
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    Advertisement.Initialize("1313183", false);
                    isDisconnected = false;
                }
            }
        }
    }
    public void ShowAd()
    {
        if (Advertisement.IsReady())
        {
            Advertisement.Show("video");
        }
    }
    public void ShowRewardedAd()
    {
        StartCoroutine(check());
    }
    private void reward(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("Finished");
                break;
            case ShowResult.Skipped:
                break;
            case ShowResult.Failed:
                break;
        }
    }
    IEnumerator check()
    {
        while (!Advertisement.isInitialized || !Advertisement.IsReady())
        {
            isDisconnected = true;
            yield return new WaitForSecondsRealtime(0.5f);
        }
        if (Advertisement.IsReady("rewardedVideo"))
        {
            //Time.timeScale = 0;
            isDisconnected = false;
            status = true;
            var options = new ShowOptions { resultCallback = reward };
            Advertisement.Show("rewardedVideo", options);
            yield return new WaitForSecondsRealtime(0.5f);
        }
    }
}