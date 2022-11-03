using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MultiLauncherManager : MonoBehaviour
{
    [SerializeField]
    private AudioSource _audioSource;
    private LauncherManager[] launchers = null;
    public ScoreManager sm = null;
    private int numberOfLaunchers = 0;
    public float launchProbability = 0.3f;
    public float startTime = 1.0f;
    public float repeatTime = 2.0f;
    private int numOfFruits = 0;

    // Start is called before the first frame update
    void Start()
    {
        //Physics.IgnoreLayerCollision(3, 8);
        //Physics.IgnoreLayerCollision(8, 8);

        _audioSource = GetComponent<AudioSource>();
        launchers = GetComponentsInChildren<LauncherManager>();
        numberOfLaunchers = launchers.Length;
        sm = FindObjectOfType<ScoreManager>();
    }

    public void GameStart()
    {
        UnityEngine.Debug.Log("MultiLauncherManager GameStart");
        InvokeRepeating("setMultiLauncherOn", startTime - (repeatTime / 2), repeatTime);
        InvokeRepeating("fireMultiLauncher", startTime, repeatTime);
    }

    public void GameOver()
    {
        CancelInvoke("setMultiLauncherOn");
        CancelInvoke("fireMultiLauncher");
    }


    void setMultiLauncherOn()
    {
        numOfFruits = 0;

        // Debug.Log("numberOfLaunchers = " + numberOfLaunchers);
        for (int i = 0; i < numberOfLaunchers; i++)
        {
            float rand = Random.Range(0.0f, 1.0f);
            if (rand <= launchProbability)
            {
                numOfFruits++;
                launchers[i].SetLauncherOn(true);
            } else
            {
                launchers[i].SetLauncherOn(false);
            }
        }



    }

    void fireMultiLauncher()
    {
        
        for (int i = 0; i < numberOfLaunchers; i++)
        {
           
            launchers[i].Fire();
        }
        if (numOfFruits > 0 && _audioSource != null)
        {
            _audioSource.Play();
        }

        if (sm != null)
        {
            sm.SetNumOfFruits(numOfFruits);
        }

    }

}
