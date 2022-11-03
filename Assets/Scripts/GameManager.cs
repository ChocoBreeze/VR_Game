using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField]
    private MultiLauncherManager MLM;
    [SerializeField]
    private ScoreManager SM;
    public bool now_gaming;
    public float time;
    // Start is called before the first frame update
    void Start()
    {
        MLM = FindObjectOfType<MultiLauncherManager>();
        SM = FindObjectOfType<ScoreManager>();
        now_gaming = false;
        time = 0;
    }

    public void GameStart()
    {
        if (now_gaming == false)
        {
            UnityEngine.Debug.Log("GameManager GameStart");
            now_gaming = true;
            SM.Initialize();
            MLM.GameStart();
            InvokeRepeating("IncreaseDifficulty", 10.0f, 10.0f); //10초 마다
        }
    }

    void IncreaseDifficulty()
    {
        UnityEngine.Debug.Log("난이도 상승!");
        if(Time.timeScale < 3.0f)
        {
            Time.timeScale += 0.1f;
        }
        
    }

    public void GameOver()
    {
        now_gaming = false;
        MLM.GameOver();
        SM.GameOver();
        Time.timeScale = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
    }
}
