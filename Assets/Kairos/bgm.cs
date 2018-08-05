using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class bgm : MonoBehaviour {
    public bool audioBegin = false;
    AudioSource audioS;
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        audioS = GetComponent<AudioSource>();
    }
    // Update is called once per frame
    public void playBGM()
    {
        audioBegin = true;
        audioS.Play();
    }
    public void stopBGM()
    {
        audioBegin = false;
        audioS.Stop();
    }

}
