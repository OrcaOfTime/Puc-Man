using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameOver : MonoBehaviour
{

    public Text GameOverText;

    void Start()
    {
        GameOverText.text = PlayerPrefs.GetString("GameFinish");
        GameOverText.enabled = true;
    }



    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
            SceneManager.LoadScene("Level_one");
        else if (Input.GetKeyUp(KeyCode.Escape))
            Application.Quit();
    }
}
