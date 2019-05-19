using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStorage : MonoBehaviour
{
    private static int boardWidth = 30;
    private static int boardHeight = 33;


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

    }

    
    void Update()
    {
        
    }
}
