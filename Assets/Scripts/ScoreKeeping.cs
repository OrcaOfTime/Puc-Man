using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreKeeping : MonoBehaviour
{
    [HideInInspector] public int totalDots, totalPowerDots, score = 0;
    
    public Text displayedScore;
    
    void Start()
    {
        totalDots = GameObject.FindGameObjectsWithTag("Dot").Length;
        totalPowerDots = GameObject.FindGameObjectsWithTag("PowerDot").Length;

        displayedScore.text = "Score: " + score;
    }

   
    void Update()
    {
        checkLevelComplete();
    }

    private void checkLevelComplete()
    {
        if (totalDots == 0 && totalPowerDots == 0)
            completeLevel();
    }

    private void completeLevel()
    {
        throw new NotImplementedException();
    }

    void addPoints(Collider2D other)
    {
        if (other.gameObject.CompareTag("Dot"))
            score += 10;
        else if (other.gameObject.CompareTag("PowerDot"))
            score += 20;

        displayedScore.text = "Score: " + score;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Dot"))
        {
            Destroy(other.gameObject);
            totalDots--;
            addPoints(other);
            //PlayEating noise            
        }

        if (other.gameObject.CompareTag("PowerDot"))
        {
            Destroy(other.gameObject);
            totalPowerDots--;
            addPoints(other);
            //PowerUpPacMan
            //Playeating noise
            //Play PowerUp Noise         
        }
    }
}
