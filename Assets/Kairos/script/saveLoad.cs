using GooglePlayGames;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using UnityEngine.SocialPlatforms;
using GooglePlayGames.BasicApi;

//using GooglePlayGames;
//using GooglePlayGames.BasicApi;

public class saveLoad : MonoBehaviour {

    public static saveLoad SL;
    public float highscore;
    public int adCounter = 0;
    public bool rated = false;
    public bool sound=true;
    public List<int> shown = new List<int>();
    public int interstitialAdCnt;
    // later implementation
    void Awake()
    {
        DontDestroyOnLoad(this);
        SL = this;
        load();
    }
    private void Start()
    {
#if UNITY_ANDROID
        PlayGamesPlatform.DebugLogEnabled = false;
        PlayGamesPlatform.Activate();
        ConnectToGoogleServices();
#else
        // IOS CODE
#endif
    }
    private void Update()
    {
        
        //Debug.Log(System.DateTime.Now.ToString("yyyy/MM/dd"));
    }
    public void ConnectToGoogleServices()
    {
        Social.Active.localUser.Authenticate((bool success) =>{
        });
    }
    public void reportScore(float Score)
    {
        if (Social.localUser.authenticated)
        {
            //scoreboard
            Social.ReportScore(Convert.ToInt32(highscore), "CgkIqMDE0r4GEAIQAA", (bool success) =>
             {
             });
            // achievement
            if (highscore > 10)
            {
                Social.ReportProgress("CgkIqMDE0r4GEAIQAw", 100.0f, (bool success) =>
                {
                });
            }
            if (highscore > 30)
            {
                Social.ReportProgress("CgkIqMDE0r4GEAIQBA", 100.0f, (bool success) =>
                {
                });
            }
            if (highscore > 60)
            {
                Social.ReportProgress("CgkIqMDE0r4GEAIQBg", 100.0f, (bool success) =>
                {
                });
            }
        }
    }
    public void save()
    {
        BinaryFormatter bf = new BinaryFormatter();
        FileStream file;
        if (File.Exists(Application.persistentDataPath + "/playerInfo.dat"))
        {
            file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
            PlayerData data = new PlayerData();
            if (!shown.Contains(0))
            {
                shown.Add(0);
            }
            data.highscore = highscore;
            data.adCounter = adCounter;
            data.rated = rated;
            data.sound = sound;
            data.shown = shown;
            data.interstitialAdCnt = interstitialAdCnt;
            bf.Serialize(file, data);   
            file.Close();
        }
        else
        {
            file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.CreateNew);
            PlayerData data = new PlayerData();
            if (!shown.Contains(0))
            {
                shown.Add(0);
            }
            data.highscore = highscore;
            data.adCounter = adCounter;
            data.rated = rated;
            data.sound = sound;
            data.shown = shown;
            data.interstitialAdCnt = interstitialAdCnt;
            bf.Serialize(file, data);
            file.Close();
        }
    }
    public void load()
    {
        if(File.Exists(Application.persistentDataPath+ "/playerInfo.dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/playerInfo.dat", FileMode.Open);
            PlayerData data = (PlayerData)bf.Deserialize(file);
            file.Close();
            highscore = data.highscore;
            adCounter = data.adCounter;
            sound = data.sound;
            rated = data.rated;
            shown = data.shown;
            interstitialAdCnt = data.interstitialAdCnt;
            if (shown == null)
            {
                shown = new List<int>();
                shown.Add(0);
            }
        }
        else
        {
            save();
        }
    }
    public void updateHighScore(float score)
    {
        if(score > highscore)
        {
            highscore = score;
            save();
        }
        reportScore(highscore);
    }
}

[Serializable]
class PlayerData
{
    public float highscore;
    public int adCounter;
    public bool rated;
    public bool sound;
    public List<int> shown;
    public int interstitialAdCnt;
}