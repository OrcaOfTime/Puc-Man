using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{

    private const float xOffset = 13.5f;
    private const float yOffset = 11.5f;
    private GameObject pucMan;

    public bool allowMovement = true;

    public float moveSpeed = 3.5f;
    public float normalMoveSpeed = 3.5f;
    public float frightenedMoveSpeed = 2.6f;
    public float consumedMoveSpeed = 10f;
    public float previousMoveSpeed;


    public MovementNode startPos;
    public MovementNode homeNode;
    public MovementNode ghostHouse;

    public float ghostReleaseTimer = 0;
    public int purplesReleaseTimer = 5;
    public int blueReleaseTimer = 10;
    public int pinkReleaseTimer = 15;

    public bool isInGhostHouse = false;

    public int frightenedModeDuration = 10;
    public int startBlinkingAt = 7;

    public RuntimeAnimatorController ghostUp;
    public RuntimeAnimatorController ghostRight;
    public RuntimeAnimatorController ghostDown;
    public RuntimeAnimatorController ghostLeft;
    public RuntimeAnimatorController ghostFrightBlue;
    public RuntimeAnimatorController ghostFrightWhite;

    public Sprite eyesUp;
    public Sprite eyesRight;
    public Sprite eyesDown;
    public Sprite eyesLeft;


    public float frightenedModeTimer = 0;
    public float blinkTimer = 0;
    bool frightenedModeIsWHite = false;

    private int modeChangeIteration = 1; //how many times are chase and scatter mode going to switch 
    private float modeChangeTimer = 0; //keep track of when we need to change mode


    private int scatterTime1 = 7;
    private int chaseTime1 = 20;
    private int scatterTime2 = 7;
    private int chaseTime2 = 20;
    private int scatterTime3 = 5;
    private int chaseTime3 = 20;
    private int scatterTime4 = 5;

    private enum ghostMode
    {
        chase,
        scatter,
        frightened,
        consumed
    }

    public enum GhostType
    {
        Green,
        Purple,
        Pink,
        Blue
    }

    public GhostType ghostType = GhostType.Green;

    ghostMode currentmode = ghostMode.scatter;
    ghostMode previousMode;
    private MovementNode currentNode, targetNode, prevNode;
    private Vector2 curDirection, nextDirection;

    void Start()
    {
        pucMan = GameObject.FindGameObjectWithTag("Player");

        MovementNode node = getNode(transform.localPosition);

        if(node != null)
          currentNode = node;

        if (node.GetComponent<SpecialObject>().isGhostHouse)
        {
            curDirection = Vector2.up;
            targetNode = currentNode.neighbours[0];
        }
        else
        {
            curDirection = Vector2.left;
            targetNode = chooseNextNode();
        }

        prevNode = currentNode;

        updateAnimation();
    }

    public void restart()
    {
        allowMovement = true;

        transform.GetComponent<SpriteRenderer>().enabled = true;
        currentmode = ghostMode.scatter;
        moveSpeed = normalMoveSpeed;
        previousMoveSpeed = 0;

        transform.position = startPos.transform.position;

        ghostReleaseTimer = 0;
        modeChangeIteration = 1;
        modeChangeTimer = 0;

        if(transform.name != "Green Ghost")
            isInGhostHouse = true;

        currentNode = startPos;

        if (isInGhostHouse)
        {
            curDirection = Vector2.up;
            targetNode = currentNode.neighbours[0];
        }
        else
        {
            curDirection = Vector2.left;
            targetNode = chooseNextNode();
        }

        prevNode = currentNode;
        updateAnimation();
    }
   
    void Update()
    {
        if (allowMovement)
        {
            modeUpdate();
            moveGhost();
            releaseGhosts();
            checkCollision();
            checkGhostInHouse();
        }
    }

    void checkGhostInHouse()
    {
        if(currentmode == ghostMode.consumed)
        {
            MovementNode node = getNode(transform.position);

            if(node != null)
            {
                if (node.transform.GetComponent<SpecialObject>().isGhostHouse)
                {
                    moveSpeed = normalMoveSpeed;
                    currentNode = node;

                    curDirection = Vector2.up;
                    targetNode = currentNode.neighbours[0];

                    prevNode = currentNode;

                    currentmode = ghostMode.chase;

                    updateAnimation();
                }
            }
        }
    }

    void checkCollision()
    {
        Rect ghostRect = new Rect(transform.position, transform.GetComponent<SpriteRenderer>().sprite.bounds.size / 4);
        Rect pucManRect = new Rect(pucMan.transform.position, pucMan.transform.GetComponent<SpriteRenderer>().sprite.bounds.size / 4);

        if (ghostRect.Overlaps(pucManRect))
        {
            if(currentmode == ghostMode.frightened)
            Consumed();
            else if(currentmode == ghostMode.chase || currentmode == ghostMode.scatter)
                GameObject.Find("Game Storage").transform.GetComponent<GameStorage>().startDeath();
            
;       }
    }



    void moveGhost()
    {
        if(targetNode != currentNode && targetNode != null && !isInGhostHouse)
        {
            if (overShot())
            {
                currentNode = targetNode;
                transform.localPosition = currentNode.transform.position;

                GameObject exitportal = getPortal(currentNode.transform.position);
              
                if(exitportal != null)
                {
                    transform.localPosition = exitportal.transform.position;

                    currentNode = exitportal.GetComponent<MovementNode>();
                }
                targetNode = chooseNextNode();
                prevNode = currentNode;
                currentNode = null;

                updateAnimation();
            }
            else
            {
                transform.localPosition += (Vector3)curDirection * moveSpeed * Time.deltaTime;
            }
        }
    }

    void modeUpdate() //determines whether ghost modes need to change
    {
        if (currentmode != ghostMode.frightened)
        {
            modeChangeTimer += Time.deltaTime;

            if (modeChangeIteration == 1)
            {
                if (currentmode == ghostMode.scatter && modeChangeTimer > scatterTime1)
                {
                    changeGhostMode(ghostMode.chase);
                    modeChangeTimer = 0;
                }
                if (currentmode == ghostMode.chase && modeChangeTimer > chaseTime1)
                {
                    changeGhostMode(ghostMode.scatter);
                    modeChangeTimer = 0;
                    modeChangeIteration = 2;
                }
            }
            else if (modeChangeIteration == 2)
            {
                if (currentmode == ghostMode.scatter && modeChangeTimer > scatterTime2)
                {
                    changeGhostMode(ghostMode.chase);
                    modeChangeTimer = 0;
                }
                if (currentmode == ghostMode.chase && modeChangeTimer > chaseTime2)
                {
                    changeGhostMode(ghostMode.scatter);
                    modeChangeTimer = 0;
                    modeChangeIteration = 3;
                }
            }
            else if (modeChangeIteration == 3)
            {
                if (currentmode == ghostMode.scatter && modeChangeTimer > scatterTime3)
                {
                    changeGhostMode(ghostMode.chase);
                    modeChangeTimer = 0;
                }
                if (currentmode == ghostMode.chase && modeChangeTimer > chaseTime3)
                {
                    changeGhostMode(ghostMode.scatter);
                    modeChangeTimer = 0;
                    modeChangeIteration = 4;
                }
            }
            else if (modeChangeIteration == 4)
            {
                if (currentmode == ghostMode.scatter && modeChangeTimer > scatterTime4)
                {
                    changeGhostMode(ghostMode.chase);
                    modeChangeTimer = 0;
                }

            }
            
        }
        else if (currentmode == ghostMode.frightened)
        {
            frightenedModeTimer += Time.deltaTime;

            if (frightenedModeTimer >= frightenedModeDuration)
            {
                frightenedModeTimer = 0;
                changeGhostMode(previousMode);
            }
            if (frightenedModeTimer >= startBlinkingAt)
            {
                blinkTimer += Time.deltaTime;
                if (blinkTimer >= 0.1f)
                {
                    blinkTimer = 0f;

                    if (frightenedModeIsWHite) //Causes flashing blue and white animation
                    {
                        transform.GetComponent<Animator>().runtimeAnimatorController = ghostFrightBlue;
                        frightenedModeIsWHite = false;
                    }
                    else
                    {
                        transform.GetComponent<Animator>().runtimeAnimatorController = ghostFrightWhite;
                        frightenedModeIsWHite = true;
                    }
                }

            }

        }
    }

    void changeGhostMode(ghostMode gm) //changes behaviour of ghost AI
    {

        if (currentmode == ghostMode.frightened)
            moveSpeed = previousMoveSpeed;
        

        if(gm == ghostMode.frightened)
        {
            previousMoveSpeed = moveSpeed;
            moveSpeed = frightenedMoveSpeed;
        }

        if(currentmode != gm)
        {
            previousMode = currentmode;
            currentmode = gm;
        }

        updateAnimation();      
    }

    public void FrightenedMode()
    {
        if(currentmode != ghostMode.consumed)
        {
            frightenedModeTimer = 0;
            changeGhostMode(ghostMode.frightened);
        }
    }

    MovementNode getNode(Vector2 pos)
    {
        MovementNode node = GameObject.Find("Game Storage").GetComponent<GameStorage>().board[(int)(pos.x + xOffset), (int)(pos.y + yOffset)];

        if(node != null)
        {
            return node;
        }

        return null;

    }

    GameObject getPortal(Vector2 pos)
    {
        MovementNode node = GameObject.Find("Game Storage").GetComponent<GameStorage>().board[(int)(pos.x + xOffset), (int)(pos.y + yOffset)];

        if(node != null)
        {
            if (node.GetComponent<SpecialObject>().isPortal)
            {
                GameObject exitPortal = node.GetComponent<SpecialObject>().portalReceiver;
                return exitPortal;
            }
        }
        return null;
    }

    float lengthFromNode(Vector2 targetPos)
    {
        Vector2 vec = targetPos - (Vector2)prevNode.transform.position;
        return vec.sqrMagnitude;
    }

    bool overShot()
    {
        float nodeToTarget = lengthFromNode(targetNode.transform.position);
        float nodeToSelf = lengthFromNode(transform.position);

        return nodeToSelf > nodeToTarget;
    }

    float getDistance(Vector2 posA, Vector2 posB) //distance between
    {
        float dx = posA.x - posB.x;
        float dy = posA.y - posB.y;

        float distance = Mathf.Sqrt((dx * dx) + (dy * dy));

        return distance;
    }

    Vector2 getGreenGhostTargetNode()
    {
        Vector2 pucManPos = pucMan.transform.localPosition;
        Vector2 targetTile = new Vector2((int)pucManPos.x, (int)pucManPos.y);

        return targetTile;
    } //mimics Blinky's AI

    Vector2 getPurpleGhostTargetNode()
    {
        //Purple ghost tries to get four tiles in front of pucMan's position

        Vector2 pucManPos = pucMan.transform.localPosition;
        Vector2 pucManOrientation = pucMan.GetComponent<PucMan>().currentDirection;

        int pucManPosX = (int)pucManPos.x;
        int pucManPosY = (int)pucManPos.y;

        Vector2 pucManTile = new Vector2(pucManPosX, pucManPosY);
        Vector2 targetTile = pucManTile + (4 * pucManOrientation);

        return targetTile;
    } //mimics Pinky's AI

    Vector2 getBlueGhostTargetNode() //Mimics Inky's AI
    {
        Vector2 pucManPos = pucMan.transform.localPosition;
        Vector2 pucManOrientation = pucMan.GetComponent<PucMan>().orientation;

        int pucManPosX = (int)pucManPos.x;
        int pucManPosY = (int)pucManPos.y;

        Vector2 pucManNode = new Vector2(pucManPosX, pucManPosY);

        Vector2 doubleOrientation = 2 * pucManOrientation;

        Vector2 targetNode = pucManNode + (2 * pucManOrientation);

        //Temp blinky position
        Vector2 tempGreenGhosrPos = GameObject.Find("Green Ghost").transform.localPosition;

        int GreenPosX = (int)tempGreenGhosrPos.x;
        int GreenPosY = (int)tempGreenGhosrPos.y;

        Vector2 GreenPos = new Vector2(GreenPosX, GreenPosY);

        float distance = getDistance(tempGreenGhosrPos, targetNode);
        distance *= 2;

        targetNode = new Vector2(tempGreenGhosrPos.x + distance, tempGreenGhosrPos.y + distance);

        return targetNode;
    }

    Vector2 getPinkGhostTargetNode()
    {
        //calculates distance of Pink Ghost from PucMan
        //if distance is greater that 8 tiles, ghost targets pacman
        //is distance is less than 8 tile, ghost scatters

        Vector2 pucManPos = pucMan.transform.localPosition;

        float distance = getDistance(transform.localPosition, pucManPos);
        Vector2 targetNode = Vector2.zero;

        if(distance >= 8)
        {
            targetNode = new Vector2((int)pucManPos.x, (int)pucManPos.y);
        }
        else if(distance < 8)
        {
            targetNode = homeNode.transform.position;
        }
        return targetNode;

    } //mimics Clyde's AI

    void releaseGhosts()
    {
        ghostReleaseTimer += Time.deltaTime;

        if (ghostReleaseTimer > purplesReleaseTimer)
            releasePurpleGhost();
        if (ghostReleaseTimer > pinkReleaseTimer)
            releasePinkGhost();
        if (ghostReleaseTimer > blueReleaseTimer)
            releaseBlueGhost();

    }

    void releasePurpleGhost()
    {
        if (ghostType == GhostType.Purple && isInGhostHouse)
            isInGhostHouse = false;
    }

    void releasePinkGhost()
    {
        if (ghostType == GhostType.Pink && isInGhostHouse)
            isInGhostHouse = false;
    }

    void releaseBlueGhost()
    {
        if (ghostType == GhostType.Blue && isInGhostHouse)
            isInGhostHouse = false;
    }

    Vector2 getTargetNode()
    {
        Vector2 targetNode = Vector2.zero;

        if (ghostType == GhostType.Green)
            targetNode = getGreenGhostTargetNode();
        else if (ghostType == GhostType.Purple)
            targetNode = getPurpleGhostTargetNode();
        else if (ghostType == GhostType.Blue)
            targetNode = getBlueGhostTargetNode();
        else if (ghostType == GhostType.Pink)
            targetNode = getPinkGhostTargetNode();



        return targetNode;
    }

    Vector2 getRandomTile()
    {
        float posX = Random.Range(0, 29);
        float posY = Random.Range(0, 32);

        return new Vector2(posX, posY);

    }

    MovementNode chooseNextNode()
    {
       
        Vector2 targetNode = Vector2.zero;

        if (currentmode == ghostMode.chase)
            targetNode = getTargetNode();
        else if (currentmode == ghostMode.scatter)
            targetNode = homeNode.transform.position;
        else if (currentmode == ghostMode.frightened)
            targetNode = getRandomTile();
        else if (currentmode == ghostMode.consumed)
            targetNode = ghostHouse.transform.position;

        MovementNode movetoNode = null;

        MovementNode[] foundNodes = new MovementNode[4];
        Vector2[] foundNodesDir = new Vector2[4];

        int nodeCount = 0;

        for (int i = 0; i < currentNode.neighbours.Length; i++)
        {
            if(currentNode.validMovement[i] != curDirection * -1) //Don't want ghost going in reverse of current direction
            {
                if(currentmode != ghostMode.consumed)
                {
                    MovementNode node = getNode(currentNode.transform.position);

                    if (node.GetComponent<SpecialObject>().isGhostHouseEntrance)
                    {
                        //found ghost house, don't want to allow movement
                        if(currentNode.validMovement[i] != Vector2.down)
                        {
                            foundNodes[nodeCount] = currentNode.neighbours[i];
                            foundNodesDir[nodeCount] = currentNode.validMovement[i];
                            nodeCount++;
                        }
                    }
                    else
                    {
                        foundNodes[nodeCount] = currentNode.neighbours[i];
                        foundNodesDir[nodeCount] = currentNode.validMovement[i];
                        nodeCount++;
                    }
                }
                else
                {
                    foundNodes[nodeCount] = currentNode.neighbours[i];
                    foundNodesDir[nodeCount] = currentNode.validMovement[i];
                    nodeCount++;
                }

                
            }
        }

        if(foundNodes.Length == 1)
        {
            movetoNode = foundNodes[0];
            curDirection = foundNodesDir[0];
        }
        if(foundNodes.Length > 1)
        {
            float shortestdDis = 10000f;
            for (int i = 0; i < foundNodes.Length; i++)
            {
                if(foundNodesDir[i] != Vector2.zero)
                {
                    float distance = getDistance(foundNodes[i].transform.position, targetNode);
                    if(distance < shortestdDis)
                    {
                        shortestdDis = distance;
                        movetoNode = foundNodes[i];
                        curDirection = foundNodesDir[i];
                    }
                }
            }
        }

        return movetoNode;
    }

    void updateAnimation()
    {
        if (currentmode != ghostMode.frightened && currentmode != ghostMode.consumed)
        {

            if (curDirection == Vector2.right)
                transform.GetComponent<Animator>().runtimeAnimatorController = ghostRight;
            else if (curDirection == Vector2.down)
                transform.GetComponent<Animator>().runtimeAnimatorController = ghostDown;
            else if (curDirection == Vector2.left)
                transform.GetComponent<Animator>().runtimeAnimatorController = ghostLeft;
            else if (curDirection == Vector2.up)
                transform.GetComponent<Animator>().runtimeAnimatorController = ghostUp;
        }
        else if(currentmode == ghostMode.frightened)
            transform.GetComponent<Animator>().runtimeAnimatorController = ghostFrightBlue;
        else if(currentmode == ghostMode.consumed)
        {
            transform.GetComponent<Animator>().runtimeAnimatorController = null;
            if (curDirection == Vector2.up)
                transform.GetComponent<SpriteRenderer>().sprite = eyesUp;
            if (curDirection == Vector2.right)
                transform.GetComponent<SpriteRenderer>().sprite = eyesRight;
            if (curDirection == Vector2.down)
                transform.GetComponent<SpriteRenderer>().sprite = eyesDown;
            if (curDirection == Vector2.left)
                transform.GetComponent<SpriteRenderer>().sprite = eyesLeft;
        }
            
    }

    void Consumed()
    {
        GameObject.Find("Game Storage").GetComponent<GameStorage>().score += 200;

        currentmode = ghostMode.consumed;
        previousMoveSpeed = moveSpeed;
        moveSpeed = consumedMoveSpeed;
        updateAnimation();

        GameObject.Find("Game Storage").transform.GetComponent<GameStorage>().startConsumedGhost(this.GetComponent<Ghost>());
    }

    
}
