using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementNode : MonoBehaviour
{

    public MovementNode[] neighbours;
    public Vector2[] validMovement;

    void Start()
    {
        validMovement = new Vector2[neighbours.Length];

        for(int i = 0; i < neighbours.Length; ++i)
        {
            MovementNode neighbour = neighbours[i];
            Vector2 tempVec = neighbour.transform.localPosition - transform.localPosition;
            validMovement[i] = tempVec.normalized;
        }
    }

    
    
}
