using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameMenu : MonoBehaviour
{
    public bool onePlayerGame;

    private Text playerText1;

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.Return))
            SceneManager.LoadScene("Level_one");
    }
}
