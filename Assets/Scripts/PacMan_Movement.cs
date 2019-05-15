using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacMan_Movement : MonoBehaviour
{   
    
    public float moveSpeed = 7f;
    
    public Vector2 direction = Vector2.zero;
    
    //private PlayerDeath 
    
    void Awake()
    {

    }

    
    void Update()
    {
        checkDirection();
        MovePacMan();
        RotatePacMan();
        
    }

    private void RotatePacMan()
    {
        if (direction == Vector2.left)
        {
            transform.localScale = new Vector3(-1, 1, 1);
            transform.localRotation = Quaternion.Euler(0, 0, 0);
        }
           
        else if (direction == Vector2.right)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 0);
            transform.localScale = new Vector3(1, 1, 1);
        }
            
        else if (direction == Vector2.down)
        {
            transform.localRotation = Quaternion.Euler(0, 0, -90f);
            transform.localScale = new Vector3(1, 1, 1);
        }
        else if(direction == Vector2.up)
        {
            transform.localRotation = Quaternion.Euler(0, 0, 90f);
            transform.localScale = new Vector3(1, 1, 1);
        }
           
    }

    private void MovePacMan()
    {
        transform.localPosition += (Vector3)direction * moveSpeed * Time.deltaTime;
    }

    private void checkDirection()
    {
        if (Input.GetKeyDown(KeyCode.DownArrow))
            direction = Vector2.down;
        else if (Input.GetKeyDown(KeyCode.UpArrow))
            direction = Vector2.up;
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
            direction = Vector2.left;
        else if (Input.GetKeyDown(KeyCode.RightArrow))
            direction = Vector2.right;
    }

    


}
