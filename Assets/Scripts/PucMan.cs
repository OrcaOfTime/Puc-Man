using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PucMan : MonoBehaviour
{
    private const float xOffset = 13.5f;
    private const float yOffset = 11.5f;
    private Vector2 nextDirection;

    private MovementNode currentNode, previousNode, targetNode;

    public Vector2 orientation;
    public float moveSpeed = 3.6f;
    public Sprite idleSprite;
    
    public Vector2 currentDirection = Vector2.zero;
    

    //private PlayerDeath 
    
    void Start()
    {
        MovementNode node = getNode(transform.localPosition);
        
        if (node != null)
        {
            currentNode = node;
        }

        // currentDirection = Vector2.left;
        orientation = Vector2.left;
    }

    
    void Update()
    {
        checkDirection();
        Move();
        Rotate();
        isIdle();
        consumeDot();
    }

    void isIdle() //Stops movement animation when Pucman stops moving
    {
        if(currentDirection == Vector2.zero)
        {
            GetComponent<Animator>().enabled = false;
            GetComponent<SpriteRenderer>().sprite = idleSprite;
        }
        else
        {
            GetComponent<Animator>().enabled = true;
        }
    }

    private void Rotate()
    {
        if (currentDirection == Vector2.left)
        {
            orientation = Vector2.left;
            transform.localScale = new Vector3(-1, 1, 1);
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
           
        else if (currentDirection == Vector2.right)
        {
            orientation = Vector2.right;
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            transform.localScale = new Vector3(1, 1, 1);
        }
            
        else if (currentDirection == Vector2.down)
        {
            orientation = Vector2.down;
            transform.localRotation = Quaternion.Euler(0, 0, -90f);
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if(currentDirection == Vector2.up)
        {
            orientation = Vector2.up;
            transform.localRotation = Quaternion.Euler(0, 0, 90f);
            transform.localScale = new Vector3(1, 1, 1);
        }
           
    }

    private void Move()
    {
        if (targetNode != currentNode && targetNode != null)
        {

            if(nextDirection == currentDirection * -1) //Allows Pucman to reverse direction whilst travelling between nodes
            {
                currentDirection *= -1;

                MovementNode tempNode = targetNode;

                targetNode = previousNode;
                previousNode = tempNode;
            }

            if (overShotDestination())
            {
                currentNode = targetNode;
                transform.localPosition = currentNode.transform.position;

                GameObject exitportal = getPortal(currentNode.transform.position);

                if(exitportal != null)
                {
                    transform.localPosition = exitportal.transform.position;
                    currentNode = exitportal.GetComponent<MovementNode>();
                }

                MovementNode destNode = canMove(nextDirection);

                if (destNode != null)
                    currentDirection = nextDirection;

                if (destNode == null)
                    destNode = canMove(currentDirection);

                if (destNode != null)
                {
                    targetNode = destNode;
                    previousNode = currentNode;
                    currentNode = null;
                }
                else
                {
                    currentDirection = Vector2.zero;
                }

            }
            else
            {
                transform.localPosition += (Vector3)(currentDirection * moveSpeed) * Time.deltaTime;
            }
        }

        transform.localPosition += (Vector3)currentDirection * moveSpeed * Time.deltaTime;
    }

    private void checkDirection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            changePosition(Vector2.down);
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            changePosition(Vector2.up);
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            changePosition(Vector2.left);
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            changePosition(Vector2.right);

    }

    MovementNode getNode(Vector2 pos)
    {

        
        MovementNode curNode = GameObject.Find("Game Storage").GetComponent<GameStorage>().board[(int)(pos.x + xOffset), (int)(pos.y + yOffset)];

        if (curNode != null)
            return curNode;

        return null;
    }

    MovementNode canMove(Vector2 dir)
    {
        MovementNode destNode = null;

        for(int i = 0; i < currentNode.neighbours.Length; ++i)
        {
            if(currentNode.validMovement[i] == dir)
            {
                destNode = currentNode.neighbours[i];
                break;
            }          
                 
        }
        return destNode;
    }

    void moveToNode(Vector2 dir)
    {
        MovementNode destNode = canMove(dir);

        if(destNode != null)
        {
            transform.localPosition = destNode.transform.localPosition;
            currentNode = destNode;
        }

    }

    void changePosition(Vector2 dir)
    {
        if (dir != currentDirection)
            nextDirection = dir;

        if (currentNode != null)
        {
            MovementNode destNode = canMove(dir);

            if(destNode != null)
            {
                currentDirection = dir;
                targetNode = destNode;
                previousNode = currentNode;
                currentNode = null;
            }
        }
            
    }

    float lengthFromNode(Vector2 destPos)
    {
        Vector2 vec = destPos - (Vector2)previousNode.transform.position;
        return vec.sqrMagnitude;
    }

    bool overShotDestination()
    {
        float nodeToDest = lengthFromNode(targetNode.transform.position);
        float nodeToPuc = lengthFromNode(transform.localPosition);

        return nodeToPuc > nodeToDest;
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

    GameObject getDot(Vector2 pos)
    {

        float PosX = pos.x;
        float PosY = pos.y;

        GameObject dot = GameObject.Find("Game Storage").GetComponent<GameStorage>().dotBoard[(int)(PosX + xOffset), (int)(PosY + yOffset)];

        if(dot != null)
            return dot;

        return null;
    }

    void consumeDot()
    {
        GameObject o = getDot(transform.position);
     
        if(o != null)
        {
            SpecialObject s = o.GetComponent<SpecialObject>();

            if(s != null)
            {
                if(!s.isConsumed && (s.isDot || s.isPowerDot))
                {
                    Debug.Log("Dot Eaten at pos: " + s.transform.position);
                    s.GetComponent<SpriteRenderer>().enabled = false;
                    s.isConsumed = true;

                    if (s.isPowerDot)
                    {
                        GameObject[] ghosts = GameObject.FindGameObjectsWithTag("Ghost");

                        foreach (GameObject g in ghosts)
                            g.GetComponent<Ghost>().FrightenedMode();
                    }
                }
            }
        }
    }

}
