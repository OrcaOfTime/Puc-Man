using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameStorage : MonoBehaviour
{
    [HideInInspector] public int totalDots, totalPowerDots;
        
    private int score = 0;
    public  int Score 
    {
        get 
        {
            return score;
        }
        set 
        {
            score = value;
            Debug.Log("Score: " + score);
            displayedScore.text = "Score: " + score;
        }
    }



    public Text displayedScore;
    public Text OptamisticMessage;
    public Text PreperrationText;
  
    public Image playerLives2;
    public Image playerLives3;

    private static int boardWidth = 31;
    private static int boardHeight = 33;

    private bool deathStarted = false;
    private bool startedConsumed = false;

    private int playerLives = 3;

    public AudioClip backgroundNormal;
    public AudioClip BackgroundFrightened;
    public AudioClip backgroundDeath;
    public AudioClip ConsumedGhost;
    
    [HideInInspector] public static float xOffset = 13.5f;
    [HideInInspector] public static float yOffset = 11.5f;

    public MovementNode[,] board = new MovementNode[boardWidth, boardHeight];
    public GameObject[,] dotBoard = new GameObject[boardWidth, boardHeight];


    void Start()
    {
        Object[] nodeStorage = GameObject.FindObjectsOfType(typeof(MovementNode));

        foreach (MovementNode n in nodeStorage)
        {
            Vector2 nodePos = n.transform.position;  
            if(n.name != "GhostHome")

            board[(int)(nodePos.x + xOffset), (int)(nodePos.y + yOffset)] = n;
              
        }

        Object[] dotStorage = GameObject.FindGameObjectsWithTag("Dot");
        Object[] powerDotStorage = GameObject.FindGameObjectsWithTag("PowerDot");

        foreach(GameObject d in dotStorage)
        {
            Vector2 dotPos = d.transform.position;
            dotBoard[(int)(dotPos.x + xOffset), (int)(dotPos.y + yOffset)] = d;
        }
        foreach(GameObject pd in powerDotStorage)
        {
            Vector2 powPos = pd.transform.position;
            dotBoard[(int)(powPos.x + xOffset), (int)(powPos.y + yOffset)] = pd;
        }

        totalDots = GameObject.FindGameObjectsWithTag("Dot").Length;
        totalDots += GameObject.FindGameObjectsWithTag("PowerDot").Length;

        startGame();

    }

    private void Update()
    {
        Debug.Log(totalDots);
        if (totalDots == 0)
            checkLevelComplete();
    }

    public void startGame()
    {
        displayedScore.GetComponent<Text>().text = "Score: " + Score;
        displayedScore.GetComponent<Text>().enabled = true;

        GameObject[] g = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in g)
        {
            ghost.transform.GetComponent<Ghost>().allowMovement = false;
            ghost.transform.GetComponent<SpriteRenderer>().enabled = false;
        }

        GameObject pucMan = GameObject.Find("Pucman");
        pucMan.transform.GetComponent<PucMan>().allowMovement = false;
        pucMan.transform.GetComponent<SpriteRenderer>().enabled = false;

        StartCoroutine(showObjectsAfterDelay(2.25f));
    }

    public void startConsumedGhost(Ghost consumeGhost)
    {
        if (!startedConsumed)
        {
            startedConsumed = true;
            GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");

            foreach(GameObject ghost in o)
                ghost.GetComponent<Ghost>().allowMovement = false;
            
            GameObject pucMan = GameObject.Find("Pucman");
            pucMan.transform.GetComponent<PucMan>().allowMovement = false;
            pucMan.transform.GetComponent<SpriteRenderer>().enabled = false;

            consumeGhost.transform.GetComponent<SpriteRenderer>().enabled = false;
            transform.GetComponent<AudioSource>().Stop();

            transform.GetComponent<AudioSource>().PlayOneShot(ConsumedGhost);

            StartCoroutine(processConsumedGhost(0.75f, consumeGhost));
        }
    }

    IEnumerator processConsumedGhost(float delay, Ghost consumeGhost)
    {
        yield return new WaitForSeconds(delay);

        GameObject pucMan = GameObject.Find("Pucman");
        pucMan.transform.GetComponent<SpriteRenderer>().enabled = true;
        pucMan.transform.GetComponent<PucMan>().allowMovement = true;

        consumeGhost.transform.GetComponent<SpriteRenderer>().enabled = true;

        GameObject[] o = GameObject.FindGameObjectsWithTag("Ghost");
        foreach (GameObject ghost in o)
            ghost.GetComponent<Ghost>().allowMovement = true;

        transform.GetComponent<AudioSource>().Play();
        startedConsumed = false;
    }

    IEnumerator showObjectsAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject[] g = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in g)
            ghost.transform.GetComponent<SpriteRenderer>().enabled = true; 

        GameObject pucMan = GameObject.Find("Pucman");
        pucMan.transform.GetComponent<SpriteRenderer>().enabled = true;

        OptamisticMessage.transform.GetComponent<Text>().enabled = false;
        PreperrationText.transform.GetComponent<Text>().enabled = false;

        StartCoroutine(startGameAfterDelay(2f));

    }

    IEnumerator startGameAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameObject[] g = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in g)
            ghost.transform.GetComponent<Ghost>().allowMovement = true;


        GameObject pucMan = GameObject.Find("Pucman");
        pucMan.transform.GetComponent<PucMan>().allowMovement = true;

        transform.GetComponent<AudioSource>().clip = backgroundNormal;
        transform.GetComponent<AudioSource>().Play();
        transform.GetComponent<AudioSource>().loop = true;

    }

    private void checkLevelComplete()
    {
        if (totalDots == 0)
            PlayerPrefs.SetString("GameFinish", "YOU WIN!");
        else if (playerLives == 0)
            PlayerPrefs.SetString("GameFinish", "Looks like you lost");

        SceneManager.LoadScene("Game Over");
    }


    public void startDeath()
    {
        if (!deathStarted)
        {

            StopAllCoroutines();

            deathStarted = true;
            GameObject[] g = GameObject.FindGameObjectsWithTag("Ghost");

            foreach (GameObject ghost in g)
                ghost.transform.GetComponent<Ghost>().allowMovement = false;
            

            GameObject pucMan = GameObject.Find("Pucman");
            pucMan.transform.GetComponent<PucMan>().allowMovement = false;
            pucMan.transform.GetComponent<Animator>().enabled = false;

            transform.GetComponent<AudioSource>().Stop();

            StartCoroutine(ProcessDeath(2));
        }
            
    }

    IEnumerator ProcessDeath(float delay)
    {
        yield return new WaitForSeconds(2);

        GameObject[] g = GameObject.FindGameObjectsWithTag("Ghost");

        foreach (GameObject ghost in g)
            ghost.transform.GetComponent<SpriteRenderer>().enabled = false;

        StartCoroutine(ProcessDeathAnimation(1.9f));


    }
    IEnumerator ProcessDeathAnimation(float delay)
    {
        GameObject pucMan = GameObject.Find("Pucman");

        pucMan.transform.localScale = new Vector3(1, 1, 1);
        pucMan.transform.localRotation = Quaternion.Euler(0, 0, 0);

        pucMan.transform.GetComponent<Animator>().runtimeAnimatorController = pucMan.transform.GetComponent<PucMan>().deathAnimation;
        pucMan.transform.GetComponent<Animator>().enabled = true;

        transform.GetComponent<AudioSource>().clip = backgroundDeath;
        transform.GetComponent<AudioSource>().Play();

        yield return new WaitForSeconds(delay);

        StartCoroutine(ProcessRestart(2));
    }
    IEnumerator ProcessRestart(float delay)
    {
        GameObject pucMan = GameObject.Find("Pucman");
        pucMan.transform.GetComponent<SpriteRenderer>().enabled = false;

        transform.GetComponent<AudioSource>().Stop();

        yield return new WaitForSeconds(delay);

        restart();
    }

    public void restart()
    {
       
        playerLives--;

        switch (playerLives)
        {
            case 2:
                playerLives3.enabled = false;
                break;
            case 1:
                playerLives2.enabled = false;
                break;
            case 0:
                checkLevelComplete();
                break;
        }
           
        GameObject pucMan = GameObject.Find("Pucman");
        pucMan.GetComponent<PucMan>().restart();

        GameObject[] g = GameObject.FindGameObjectsWithTag("Ghost");

        foreach(GameObject ghost in g)
            ghost.transform.GetComponent<Ghost>().restart();

        transform.GetComponent<AudioSource>().clip = backgroundNormal;
        transform.GetComponent<AudioSource>().Play();
        transform.GetComponent<AudioSource>().loop = true;

        deathStarted = false;
    }
}
