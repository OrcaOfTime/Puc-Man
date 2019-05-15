using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{
    public int totalDots = 0;
    public int totalPowerDots = 0;
    public int score = 0;
    
    void Start()
    {
        totalDots = GameObject.FindGameObjectsWithTag("Dot").Length;
        totalPowerDots = GameObject.FindGameObjectsWithTag("PowerDot").Length;
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

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Dot"))
        {
            Destroy(other.gameObject);
            totalDots--;
            //AddPoints
            //PlayEating noise            
        }

        if (other.gameObject.CompareTag("PowerDot"))
        {
            Destroy(other.gameObject);
            totalPowerDots--;
            //Addpoints
            //PowerUpPacMan
            //Playeating noise
            //Play PowerUp Noise         
        }
    }
}
