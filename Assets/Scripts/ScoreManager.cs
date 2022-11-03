using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreManager : MonoBehaviour
{
    static private int score = 0;
    static private int life = 10;
    private int numOfFruits = 0;
    private int additionalScore = 0;
    public TextMeshPro scoreText;
    public TextMeshPro lifeText;
    public TextMeshPro additionalScoreText;
    public TextMeshPro GameOverText;

    private GameManager gm;

    // Start is called before the first frame update
    public void SetNumOfFruits(int num)
    {
        numOfFruits = num;
        additionalScore = num;
    }

    public int GetLife()
    {
        return life;
    }

    public void PlusScore()
    {
        score++;
        numOfFruits--;
        Debug.Log(numOfFruits);
        if(numOfFruits == 0 && additionalScore != 1)
        {
            score += additionalScore;
            additionalScoreText.text = "Additional Scored!";
        } 
        if(additionalScore == 1)
        {
            additionalScoreText.text = "";
        }
    }

    public void MinusLife()
    {
        life--;
        if(life<=0)
        {
            gm.GameOver();
        }
        if(numOfFruits > 0)
        {
            additionalScoreText.text = "";
        }
    }
    void Start()
    {
        GameOverText.fontSize = 20;
        GameOverText.text = "Press Button\n\t To Start";
        gm = FindObjectOfType<GameManager>();
        // initialize();
    }

    public void Initialize()
    {
        score = 0;
        life = 10;
        additionalScoreText.text = "";
        GameOverText.text = "";
    }

    public void GameOver()
    {
        additionalScoreText.text = "";
        scoreText.text = "";
        lifeText.text = "";
        GameOverText.text = "Game\n\tOver\nScore = " + score;
    }

    // Update is called once per frame
    void Update()
    {
        if (gm.now_gaming)
        {
            scoreText.text = "Score : " + score.ToString();
            lifeText.text = "Life : " + life.ToString();
        }
        else
        {
            additionalScoreText.text = "";
        }
    }

}
