using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class gamecontrol : MonoBehaviour {
    public GameObject birtual;
    //share variables
    string subject; 
    string playStoreURL = "https://play.google.com/store/apps/details?id=com.Kairos.followfinger&hl=en";
    string body;
    // gameControlBools
    public bool started, moving, isInPattern, isPattern3, almostDone, gameover = false, basics = false, incremented = false;
    // hand control memory
    GameObject hand;
    // touchPosition memory define
    public Vector3 touchPosition;
    // master variable to modify speed
    public float moveSpeed;
    int pattern;
    // Timekeeper
    public float startTime,timeCounter;
    // IND images
    public Image ind1, ind2, ind3_1, ind3_2,ind4,ind5,ind6;
    public bool ind_showing = false;
    //main logo and ind and score
    public Image logo, ind;
    public float score;
    public Text scoretext;
    public Text highScoretext;
    // buttons when dead
    public Button homeB, shareB, rateB,leader;

    //sounds
    public AudioClip jump, upSound, downSound, tap;
    public AudioSource audioS;
    public Sprite on, off;
    public Image Sound;
    saveLoad SaveLoad;
    bgm bgms;
    private bool isProcessing;

    private void Awake()
    {
        SaveLoad = GameObject.Find("SaveLoad").GetComponent<saveLoad>();
        bgms = GameObject.Find("bgm").GetComponent<bgm>();
        body = playStoreURL;
    }
    void Start ()
    {
        timeCounter = 0;
        incremented = false;
        //initializing game
        Time.timeScale = 1;
        hand = GameObject.Find("hand");
        moveSpeed = Time.deltaTime;
        audioS = transform.GetComponent<AudioSource>();

        //gameControlBools
        started = false;
        gameover = false;
        isInPattern = false;
        isPattern3 = false;
        //almostDone = true;
        nextGen();

        // UI elements
        logo.enabled = true; ind.enabled = true; scoretext.enabled = false;
        ind1.enabled = false; ind2.enabled = false; ind3_1.enabled = false; ind3_2.enabled = false; ind4.enabled = false; ind5.enabled = false; ind6.enabled = false;
        if ((SaveLoad.shown.Contains(1) && SaveLoad.shown.Contains(2) && SaveLoad.shown.Contains(3) && SaveLoad.shown.Contains(4) && SaveLoad.shown.Contains(5) && SaveLoad.shown.Contains(6)))
        {
            basics = true;
        }
        
    }
    void deadButtonControl(bool swit)
    {
        homeB.gameObject.SetActive(swit);
        shareB.gameObject.SetActive(swit);
        rateB.gameObject.SetActive(swit);
        leader.gameObject.SetActive(swit);
    }
    // Update is called once per frame
    void Update() {
        if (!bgms.audioBegin&&SaveLoad.sound)
        {
            bgms.playBGM();
        }
        else if (bgms.audioBegin && !SaveLoad.sound)
        {
            bgms.stopBGM();
        }
        if (SaveLoad.sound)
        {
            Sound.sprite = on;
        }
        else
        {
            Sound.sprite = off;
        }
        if (score > SaveLoad.highscore) {
            highScoretext.text = "HIGH " + score;
        }
        else if (gameover && score <= SaveLoad.highscore)
        {
            highScoretext.text = "HIGH " + SaveLoad.highscore;
        }
        else
        {
            highScoretext.text = "";
        }

        deadButtonControl(gameover);
        if (!started)
        {
            Touch startTouch = Input.GetTouch(0);
            if (startTouch.phase == TouchPhase.Began)
            {
                logo.enabled = false;
                ind.enabled = false;
                scoretext.enabled = true;
                started = true;
                startTime = Time.time;
                Sound.enabled = false;
            }
        }

        if (!gameover)
        {
            score = Mathf.Round((Time.time - startTime) * 100f) / 100f;
            if (Time.time - startTime < 10f) { 
                moveSpeed = 0.013f + Mathf.Lerp(0f, 0.007f, (Time.time - startTime) / 10f);
            }
            else if (Time.time - startTime > 10f && Time.time - startTime < 15f)
            {
                moveSpeed = 0.02f + Mathf.Lerp(0f, 0.005f, ((Time.time - startTime)-10f) / 5f);
            }
            else if (Time.time - startTime > 15f)
            {
                moveSpeed = 0.025f;
            }
            Debug.Log(moveSpeed);
            SaveLoad.updateHighScore(score);
            scoretext.text = "" + score.ToString("F2");
        }
        if (started && !gameover) {
            //////////////////////////////////////
            if (!moving)
            {
                StartCoroutine(moveFinger());
            }
            if (Input.touchCount > 0&&!isPattern3)
            {
                Touch touch = Input.GetTouch(0);
                if(touch.phase == TouchPhase.Began||touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
                {
                    Vector2 touchedPos = Camera.main.ScreenToWorldPoint(new Vector2(touch.position.x, touch.position.y));
                    RaycastHit2D hitInfo = Physics2D.Raycast(touchedPos, Camera.main.transform.forward);
                    if (hitInfo.collider != null)
                    {
                        GameObject touchedObject = hitInfo.transform.gameObject;
                        Debug.Log("Touched " + touchedObject.transform.name);
                        gameover = false;
                    }
                    else
                    {
                        gameover = true;
                    }
                }
                else
                {
                    gameover = true;
                }
                if(touch.phase == TouchPhase.Began && SaveLoad.sound)
                {
                    audioS.PlayOneShot(tap);
                } 
            }
            if(Input.touchCount > 0)
            {
                Touch touch = Input.GetTouch(0);
                Vector2 touchedPos = Camera.main.ScreenToWorldPoint(new Vector2(touch.position.x, touch.position.y));
                birtual.transform.position = new Vector3(touchedPos.x,touchedPos.y,0f) + new Vector3(5, 0, -1);
            }
            else
            {
                birtual.transform.position = new Vector3(5, 0, 10);

            }
        }
        if (hand.transform.rotation.y > 0.28 && Input.touchCount > 0)
        {
            gameover = true;
        }
        // gameover check and action
        if (gameover)
        {
            if (Social.localUser.authenticated)
            {
                Social.ReportProgress("CgkIqMDE0r4GEAIQAg", 100.0f, (bool success) =>
                {
                });
            }
            Sound.enabled = true;
            SaveLoad.updateHighScore(score);
            Time.timeScale = 0;
            if (!incremented) {
                incremented = true;
                SaveLoad.adCounter++;
                if (SaveLoad.adCounter > 5)
                {
                    if (Social.localUser.authenticated)
                    {
                        Social.ReportProgress("CgkIqMDE0r4GEAIQBQ", 100.0f, (bool success) =>
                        {
                        });
                    }
                    SaveLoad.adCounter = 0;
                    SaveLoad.interstitialAdCnt++;
                    ad Ads = GameObject.Find("unityad").GetComponent<ad>();
                    if (SaveLoad.interstitialAdCnt > 6)
                    {
                        Ads.ShowRewardedAd();
                    }
                    else
                    {
                        Ads.ShowAd();
                    }
                }
                SaveLoad.save();
            }
        }
        ///////////////////////////////////
    }
    IEnumerator moveFinger()
    {
        moving = true;
        while (!gameover)
        {
            if (almostDone)
            {
                yield return new WaitUntil(() => !isInPattern);
                Debug.Log("pattern : " + pattern);
                isInPattern = true;
                switch (pattern)
                {
                    case 1:
                        StartCoroutine(pattern1());
                        break;
                    case 2:
                        StartCoroutine(pattern2());
                        break;
                    case 3:
                        StartCoroutine(pattern3());
                        break;
                    case 4:
                        StartCoroutine(pattern4());
                        break;
                    case 5:
                        StartCoroutine(pattern5());
                        break;
                    case 6:
                        StartCoroutine(pattern6());
                        break;
                    case 7:
                        StartCoroutine(pattern7());
                        break;
                }
            }
            yield return new WaitForFixedUpdate();
        }
    }
    void nextGen()
    {
        if (!almostDone)
        {
            almostDone = true;
            if (basics)
            {
                pattern = UnityEngine.Random.Range(1, 8);
            }
            else { 
                pattern = UnityEngine.Random.Range(1, 7);
            }
            //pattern = 7;
            Debug.Log("next : " + pattern);
            if (!basics) { 
            switch (pattern)
                {
                    case 1:
                        StartCoroutine(pattern1IND());
                        break;
                    case 2:
                        StartCoroutine(pattern2IND());
                        break;
                    case 4:
                        StartCoroutine(pattern4IND());
                        break;
                    case 5:
                        StartCoroutine(pattern5IND());
                        break;
                    case 6:
                        StartCoroutine(pattern6IND());
                        break;
                }
            }
        }
    }
    // up down
    IEnumerator pattern1()
    {

        int count = 0;
        bool up = true;
        bool sound = false;
        while (count < 6)
        {
            almostDone = false;
            if (gameover)
            {
                break;
            }
            // up;
            if (up)
            {
                //Debug.Log(up);
                if (!sound && SaveLoad.sound)
                {
                    audioS.PlayOneShot(upSound);
                    sound = true;
                }
                hand.transform.position = Vector3.Lerp(hand.transform.position, new Vector3(5f, 4f, 0f), moveSpeed * 1.5f);
                
                if (hand.transform.position.y > 3.45f)
                {
                    sound = false;
                    up = false;
                    count++;
                }
            }
            // down;

            else if (!up)
            {
                if (!sound && SaveLoad.sound)
                {
                    audioS.PlayOneShot(downSound);
                    sound = true;
                }
                //Debug.Log(up);
                hand.transform.position = Vector3.Lerp(hand.transform.position, new Vector3(5f, -3.5f, 0f), moveSpeed * 1.5f);
                if (hand.transform.position.y < -2.95f)
                {
                    sound = false;
                    up = true;
                    count++;
                }
            }
            //Debug.Log(hand.transform.position.y);
            // reset;
            yield return new WaitForFixedUpdate();
        }
        nextGen();
        Debug.Log("pattern1 almostdone");
        while (Mathf.Abs(hand.transform.position.y) > 0.03f)
        {
            hand.transform.position = Vector3.Lerp(hand.transform.position, new Vector3(5f, 0f, 0f), moveSpeed * 2f);
            yield return new WaitForSeconds(0.01f);
            //yield return new WaitForFixedUpdate();
        }
        isInPattern = false;
    }
    // rotation
    IEnumerator pattern2()
    {
        int count = 0;
        bool up = true;
        bool sound = false;
        while (count < 6)
        {
            almostDone = false;
            if (gameover)
            {
                break;
            }
            // up;
            if (up)
            {
                if (!sound && SaveLoad.sound)
                {
                    audioS.PlayOneShot(upSound);
                    sound = true;
                }
                hand.transform.rotation = Quaternion.Slerp(hand.transform.rotation, Quaternion.Euler(0f, 0f, 50f), moveSpeed* 2f);
                if (hand.transform.rotation.z < 0.45f)
                {
                    sound = false;
                    up = false;
                    count++;
                }
            }
            // down;
            else if (!up)
            {
                if (!sound && SaveLoad.sound)
                {
                    audioS.PlayOneShot(downSound);
                    sound = true;
                }
                hand.transform.rotation = Quaternion.Slerp(hand.transform.rotation, Quaternion.Euler(0f, 0f, 130f), moveSpeed * 2f);
                if (hand.transform.rotation.z > 0.88f)
                {
                    sound = false;
                    up = true;
                    count++;
                }
            }

            // reset;
            yield return new WaitForFixedUpdate();
        }
        nextGen();
        Debug.Log("pattern2 almostdone");
        while (Mathf.Abs(hand.transform.rotation.z) - 0.7f > 0.03f)
        {
            hand.transform.rotation = Quaternion.Slerp(hand.transform.rotation, Quaternion.Euler(0f, 0f, 90f), moveSpeed * 2f);
            yield return new WaitForSeconds(0.01f);
            //yield return new WaitForFixedUpdate();
        }

        isInPattern = false;
    }
    // leap
    IEnumerator pattern3()
    {
        StartCoroutine(pattern3IND());
        isPattern3 = true;
        bool up = true;
        almostDone = false;
        bool sound = false;
        while (up && hand.transform.rotation.y < 0.5f)
        {
            if (!sound && SaveLoad.sound)
            {
                audioS.PlayOneShot(jump);
                sound = true;
            }
            hand.transform.rotation = Quaternion.Lerp(hand.transform.rotation, Quaternion.Euler(0f, 80f, 90f), moveSpeed * 2f);
            if (hand.transform.rotation.y > 0.4f)
            {
                sound = false;
                up = false;
                break;
            }
            if (gameover)
            {
                break;
            }
            almostDone = false;
            yield return new WaitForFixedUpdate();
        }

        nextGen();
        
        while (!up && hand.transform.rotation.y > 0.01f)
        {
            hand.transform.rotation = Quaternion.Lerp(hand.transform.rotation, Quaternion.Euler(0f, 0f, 90f), moveSpeed * 3f);
            if (hand.transform.rotation.y < 0.01f)
            {
                break;
            }
            if (gameover)
            {
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        if(Input.touchCount < 1)
        {
            gameover = true;
        }

        isPattern3 = false;
        yield return new WaitForFixedUpdate();
        isInPattern = false;
    }
    // square;
    IEnumerator pattern4()
    {
        almostDone = false;
        int phase = 0;
        bool sound = false;
        //left
        while (hand.transform.position.x> 3.5f&&phase == 0)
        {
            hand.transform.position = Vector3.Lerp(hand.transform.position, new Vector3(3f, 0f, 0f), moveSpeed * 1.5f);
            //Debug.Log(phase + " !! " + hand.transform.position + "++" + moveSpeed * 1.5f);
            if (hand.transform.position.x < 3.5f)
            {
                phase++;
                break;
            }
            if (gameover)
            {
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        //left up
        while (hand.transform.position.y < 3f && phase == 1)
        {
            if (!sound && SaveLoad.sound)
            {
                audioS.PlayOneShot(upSound);
                sound = true;
            }
            hand.transform.position = Vector3.Lerp(hand.transform.position, new Vector3(3.5f, 3.5f, 0f), moveSpeed * 1.5f);
            if (hand.transform.position.y >= 3f)
            {
                sound = false;
                phase++;
                break;
            }
            if (gameover)
            {
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        //right up
        while (hand.transform.position.x < 6.5f && phase == 2)
        {
            if (!sound && SaveLoad.sound)
            {
                audioS.PlayOneShot(upSound);
                sound = true;
            }
            hand.transform.position = Vector3.Lerp(hand.transform.position, new Vector3(7.5f, 3f, 0f), moveSpeed * 1.5f);
            if (hand.transform.position.x >= 6.5f)
            {
                sound = false;
                phase++;
                break;
            }
            if (gameover)
            {
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        //rightDown
        while (hand.transform.position.y > -2f && phase == 3)
        {
           
            hand.transform.position = Vector3.Lerp(hand.transform.position, new Vector3(6.5f, -3f, 0f), moveSpeed * 1.5f);
            Debug.Log(phase + " !! " + hand.transform.position);
            if (!sound && SaveLoad.sound)
            {
                audioS.PlayOneShot(downSound);
                sound = true;
            }
            if (hand.transform.position.y<-2f)
            {
                sound = false;
                phase++;
                break;
            }
            if (gameover)
            {
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        //left Down
        while (hand.transform.position.x > 3.5f && phase == 4)
        {
            hand.transform.position = Vector3.Lerp(hand.transform.position, new Vector3(3f, -2f, 0f), moveSpeed * 1.5f);
            if (!sound && SaveLoad.sound)
            {
                audioS.PlayOneShot(downSound);
                sound = true;
            }
            if (hand.transform.position.x < 3.5f)
            {
                sound = false;
                phase++;
                break;
            }
            if (gameover)
            {
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        //left
        while (hand.transform.position.y <-1.01f && phase == 5)
        {
            hand.transform.position = Vector3.Lerp(hand.transform.position, new Vector3(3.5f, 1f, 0f), moveSpeed * 1.5f);
            if (!sound && SaveLoad.sound)
            {
                audioS.PlayOneShot(upSound);
                sound = true;
            }
            if (hand.transform.position.y < 0.01f)
            {
                sound = false;
                phase++;
                break;
            }
            if (gameover)
            {
                break;
            }
            almostDone = false;

            yield return new WaitForFixedUpdate();
        }
        nextGen();
        while (hand.transform.position.x < 5.01f && phase == 6)
        {
            hand.transform.position = Vector3.Lerp(hand.transform.position, new Vector3(6f, 1f, 0f), moveSpeed * 1.5f);
            if (hand.transform.position.x > 4.8f)
            {
                phase++;
                break;
            }
            if (gameover)
            {
                break;
            }
            yield return new WaitForFixedUpdate();
        }
       
        yield return new WaitForFixedUpdate();

        isInPattern = false;
    }
    // circle;
    IEnumerator pattern5()
    {
        almostDone = false;
        isInPattern = true;
        bool sound = false;
        float originalX = hand.transform.position.x;
        timeCounter = 0f;
        timeCounter = moveSpeed * 2f;
        Vector3 originalPos = hand.transform.position;
        hand.transform.position = new Vector3(hand.transform.position.x + Mathf.Cos(timeCounter)/15, hand.transform.position.y+ Mathf.Sin(timeCounter)/15, hand.transform.position.z);
        while(hand.transform.position.x != originalX && timeCounter<7f)
        {
            if (!sound && SaveLoad.sound)
            {
                audioS.PlayOneShot(upSound);
                sound = true;
            }
            hand.transform.position = new Vector3(hand.transform.position.x + Mathf.Cos(timeCounter)/15, hand.transform.position.y + Mathf.Sin(timeCounter)/15, hand.transform.position.z);
            timeCounter +=moveSpeed * 2f;
            yield return new WaitForFixedUpdate();
        }
        sound = false;
        nextGen();
        while (Mathf.Abs(hand.transform.position.y) > 0.03f)
        {
            hand.transform.position = Vector3.Slerp(hand.transform.position, new Vector3(5f, 0f, 0f), moveSpeed * 1.5f);
            yield return new WaitForSeconds(0.01f);
        }
        isInPattern = false;
    }
    // Random
    IEnumerator pattern6()
    {
        almostDone = false;
        isInPattern = true;
        bool sound = false;
        float originalX = hand.transform.position.x;
        float originalTime = Time.time;
        Vector3 newPos = new Vector3(UnityEngine.Random.Range(3.5f, 6.5f), UnityEngine.Random.Range(-2f, 3f), 0f);

        while (Time.time < originalTime+10f)
        {
            if (Time.time %1.5f ==0)
            {
                sound = false;
                while (Mathf.Abs(hand.transform.position.x-newPos.x)<1f&& Mathf.Abs(hand.transform.position.y - newPos.y) < 2f) { 
                    newPos = new Vector3(UnityEngine.Random.Range(3.5f, 6.5f), UnityEngine.Random.Range(-2f, 3f),0f);
                    yield return new WaitForFixedUpdate();
                }
                if (newPos.y > hand.transform.position.y)
                {
                    if (!sound && SaveLoad.sound)
                    {
                        audioS.PlayOneShot(upSound);
                        sound = true;
                    }
                }
            }
            hand.transform.position = Vector3.Lerp(hand.transform.position, newPos,moveSpeed*3f);
            yield return new WaitForFixedUpdate();
        }
        nextGen();
        while (Mathf.Abs(hand.transform.position.y) > 0.03f)
        {
            hand.transform.position = Vector3.Lerp(hand.transform.position, new Vector3(5f, 0f, 0f), moveSpeed * 2f);
            yield return new WaitForSeconds(0.01f);
        }

        isInPattern = false;
    }
    // multi tapoff
    IEnumerator pattern7()
    {
        almostDone = false;
        isInPattern = true;
        bool up = true;
        bool init = false;
        float originalX = hand.transform.position.x;
        float originalTime = Time.time;
        bool sound = false;
        while (!init&&hand.transform.position.y < 3.45f)
        {
            hand.transform.position = Vector3.Lerp(hand.transform.position, new Vector3(5f, 4f, 0f), moveSpeed * 2f);
            yield return new WaitForFixedUpdate();
            if (!sound && SaveLoad.sound)
            {
                audioS.PlayOneShot(upSound);
                sound = true;
            }
        }
        init = true;
        sound = false;
        /////////////////////
        StartCoroutine(pattern3IND());
        while (hand.transform.position.y > 3f)
        {
            hand.transform.position = Vector3.Lerp(hand.transform.position, new Vector3(5f, 2.5f, 0f), moveSpeed * 2f);
            yield return new WaitForFixedUpdate();
        }
        isPattern3 = true;
        while (up && hand.transform.rotation.y < 0.5f)
        {
            if (!sound && SaveLoad.sound)
            {
                audioS.PlayOneShot(jump);
                sound = true;
            }
            hand.transform.rotation = Quaternion.Slerp(hand.transform.rotation, Quaternion.Euler(0f, 80f, 90f), moveSpeed * 2f);
            if (hand.transform.rotation.y > 0.4f)
            {
                up = false;
                break;
            }
            if (gameover)
            {
                break;
            }
            almostDone = false;
            yield return new WaitForFixedUpdate();
        }
        sound = false;
        while (!up && hand.transform.rotation.y > 0.02f)
        {
            hand.transform.rotation = Quaternion.Slerp(hand.transform.rotation, Quaternion.Euler(0f, 0f, 90f), moveSpeed * 3f);
            if (hand.transform.rotation.y < 0.02f)
            {
                up = true;
                break;
            }
            if (gameover)
            {
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        if (Input.touchCount < 1)
        {
            gameover = true;
        }
        isPattern3 = false;
        /////////////////////
        StartCoroutine(pattern3IND());

        while (hand.transform.position.y > 2f)
        {
            hand.transform.position = Vector3.Lerp(hand.transform.position, new Vector3(5f, 1.5f, 0f), moveSpeed *2f);
            yield return new WaitForFixedUpdate();
        }
        isPattern3 = true;
        while (up && hand.transform.rotation.y < 0.5f)
        {
            if (!sound && SaveLoad.sound)
            {
                audioS.PlayOneShot(jump);
                sound = true;
            }
            hand.transform.rotation = Quaternion.Slerp(hand.transform.rotation, Quaternion.Euler(0f, 80f, 90f), moveSpeed * 2f);
            if (hand.transform.rotation.y > 0.4f)
            {
                up = false;
                break;
            }
            if (gameover)
            {
                break;
            }
            almostDone = false;
            yield return new WaitForFixedUpdate();
        }
        sound = false;
        while (!up && hand.transform.rotation.y > 0.02f)
        {
            hand.transform.rotation = Quaternion.Slerp(hand.transform.rotation, Quaternion.Euler(0f, 0f, 90f), moveSpeed * 3f);
            if (hand.transform.rotation.y < 0.02f)
            {
                up = true;
                break;
            }
            if (gameover)
            {
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        if (Input.touchCount < 1)
        {
            gameover = true;
        }
        isPattern3 = false;
        /////////////////////
        StartCoroutine(pattern3IND());

        while (hand.transform.position.y > 1f)
        {
            hand.transform.position = Vector3.Lerp(hand.transform.position, new Vector3(5f, 0.5f, 0f), moveSpeed * 2f);
            yield return new WaitForFixedUpdate();
        }
        isPattern3 = true;

        while (up && hand.transform.rotation.y < 0.5f)
        {
            if (!sound && SaveLoad.sound)
            {
                audioS.PlayOneShot(jump);
                sound = true;
            }
            hand.transform.rotation = Quaternion.Slerp(hand.transform.rotation, Quaternion.Euler(0f, 80f, 90f), moveSpeed * 2f);
            if (hand.transform.rotation.y > 0.4f)
            {
                up = false;
                break;
            }
            if (gameover)
            {
                break;
            }
            almostDone = false;
            yield return new WaitForFixedUpdate();
        }
        sound = false;
        while (!up && hand.transform.rotation.y > 0.01f)
        {
            hand.transform.rotation = Quaternion.Slerp(hand.transform.rotation, Quaternion.Euler(0f, 0f, 90f), moveSpeed *3f);
            if (hand.transform.rotation.y < 0.02f)
            {
                up = true;
                break;
            }
            if (gameover)
            {
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        if (Input.touchCount < 1)
        {
            gameover = true;
        }
        
        isPattern3 = false;
        /////////////////////
        StartCoroutine(pattern3IND());

        while (hand.transform.position.y > 0f)
        {
            hand.transform.position = Vector3.Lerp(hand.transform.position, new Vector3(5f, -0.5f, 0f), moveSpeed * 2f);
            yield return new WaitForFixedUpdate();
        }
        isPattern3 = true;
        while (up && hand.transform.rotation.y < 0.5f)
        {
            if (!sound && SaveLoad.sound)
            {
                audioS.PlayOneShot(jump);
                sound = true;
            }
            hand.transform.rotation = Quaternion.Slerp(hand.transform.rotation, Quaternion.Euler(0f, 80f, 90f), moveSpeed * 2f);
            if (hand.transform.rotation.y > 0.4f)
            {
                up = false;
                break;
            }
            if (gameover)
            {
                break;
            }
            almostDone = false;
            yield return new WaitForFixedUpdate();
        }
        while (!up && hand.transform.rotation.y > 0.02f)
        {
            hand.transform.rotation = Quaternion.Slerp(hand.transform.rotation, Quaternion.Euler(0f, 0f, 90f), moveSpeed * 3f);
            if (hand.transform.rotation.y < 0.02f)
            {
                up = true;
                break;
            }
            if (gameover)
            {
                break;
            }
            yield return new WaitForFixedUpdate();
        }
        if (Input.touchCount < 1)
        {
            gameover = true;
        }
        isPattern3 = false;
        sound = false;
        /////////////////////

        nextGen();
        while (Mathf.Abs(hand.transform.position.y) > 0.03f)
        {
            hand.transform.position = Vector3.Lerp(hand.transform.position, new Vector3(5f, 0f, 0f), moveSpeed * 2f);
            yield return new WaitForSeconds(0.01f);
        }
        //Debug.Log("222!!");

        isInPattern = false;
    }
    IEnumerator pattern1IND()
    {
        if (!SaveLoad.shown.Contains(1)) { 
            ind1.enabled = true;
            yield return new WaitForSeconds(1);
            ind1.enabled = false;
            SaveLoad.shown.Add(1);
        }
    }
    IEnumerator pattern2IND()
    {
        if (!SaveLoad.shown.Contains(2))
        {

            ind2.enabled = true;
            yield return new WaitForSeconds(1);
            ind2.enabled = false;
            SaveLoad.shown.Add(2);
        }
    }
    IEnumerator pattern3IND()
    {
            ind3_1.enabled = true;
            yield return new WaitForSeconds(1f);
            ind3_1.enabled = false;
            yield return new WaitForSeconds(0.3f);
            ind3_2.enabled = true;
            yield return new WaitForSeconds(0.5f);
            ind3_2.enabled = false;
    }
    IEnumerator pattern4IND()
    {
        if (!SaveLoad.shown.Contains(4))
        {

            ind4.enabled = true;
            yield return new WaitForSeconds(1);
            ind4.enabled = false;
            SaveLoad.shown.Add(4);
        }
    }
    IEnumerator pattern5IND()
    {
        if (!SaveLoad.shown.Contains(5))
        {

            ind5.enabled = true;
            yield return new WaitForSeconds(1);
            ind5.enabled = false;
            SaveLoad.shown.Add(5);
        }
    }
    IEnumerator pattern6IND()
    {
        if (!SaveLoad.shown.Contains(6))
        {

            ind6.enabled = true;
            yield return new WaitForSeconds(1);
            ind6.enabled = false;
            SaveLoad.shown.Add(6);
        }
    }
    public void soundButton()
    {
        if (SaveLoad.sound)
        {
            SaveLoad.sound = false;
        }
        else
        {
            SaveLoad.sound = true;
        }
    }
    public void reloadButton()
    {
        SceneManager.LoadScene("mainplay");
    }
    public void shareButton()
    {
#if UNITY_ANDROID
        if (!isProcessing)
        {
            deadButtonControl(false);
            StartCoroutine(ShareAndroid());
        }
#endif
    }
    IEnumerator ShareAndroid()
    {
        isProcessing = true;
        deadButtonControl(false);
        String message = playStoreURL;
        yield return new WaitForEndOfFrame();
        //create the texture
        Texture2D screenTexture = new Texture2D(Screen.width, Screen.height, TextureFormat.RGB24, true);
        //put buffer intoTexture
        screenTexture.ReadPixels(new Rect(0f, 0f, Screen.width, Screen.height), 0, 0);
        // apply
        screenTexture.Apply();
        byte[] dataToSave = screenTexture.EncodeToPNG();
        string destination = Path.Combine(Application.persistentDataPath, System.DateTime.Now.ToString("yyyy-MM-dd-HHmmss") + ".png");
        File.WriteAllBytes(destination, dataToSave);
        if (!Application.isEditor)
        {
            //----share
            AndroidJavaClass intentClass = new AndroidJavaClass("android.content.Intent");
            AndroidJavaObject intentObject = new AndroidJavaObject("android.content.Intent");
            intentObject.Call<AndroidJavaObject>("setAction", intentClass.GetStatic<string>("ACTION_SEND"));
            AndroidJavaClass uriClass = new AndroidJavaClass("android.net.Uri");
            AndroidJavaObject uriObject = uriClass.CallStatic<AndroidJavaObject>("parse", "file://" + destination);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_TEXT"), "" + message);
            intentObject.Call<AndroidJavaObject>("putExtra", intentClass.GetStatic<string>("EXTRA_STREAM"), uriObject);

            intentObject.Call<AndroidJavaObject>("setType", "image/jpeg");
            AndroidJavaClass unity = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
            AndroidJavaObject currentActivity = unity.GetStatic<AndroidJavaObject>("currentActivity");

            currentActivity.Call("startActivity", intentObject);
        }
        isProcessing = false;
        deadButtonControl(true);
    }
    public void rateButton()
    {
#if UNITY_ANDROID
       // Application.OpenURL(playStoreURL);
#endif
    }
    public void leaderButton()
    {
        if (Social.localUser.authenticated)
        {
            Social.ShowLeaderboardUI();
        }
    }
}
