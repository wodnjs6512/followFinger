using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class lightControl : MonoBehaviour
{
    private bool rotating = true;
    private float skipTime;

    private void Start()
    {
        skipTime = Time.time + 3f;
    }
    private void Update()
    {
        if (rotating)
        {
            Vector3 to = new Vector3(0, 30, 0);
            if (Vector3.Distance(transform.eulerAngles, to) > 0.01f)
            {
                transform.eulerAngles = Vector3.Lerp(transform.rotation.eulerAngles, to, Time.deltaTime);
            }
            else
            {
                transform.eulerAngles = to;
                rotating = false;
            }
        }
        if (Time.time > skipTime)
        {
            SceneManager.LoadScene("mainplay");
        }
    }
}
