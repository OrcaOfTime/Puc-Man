using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    public GameObject portalReceiver;
    public GameObject player;

    public Vector3 prevPos = Vector3.zero;
    public Vector3 curVel;

    public bool rightPortal;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        curVel = (player.transform.position - prevPos) / Time.deltaTime;
        prevPos = player.transform.position;
    }

    public void OnTriggerEnter2D(Collider2D col)
    {
        if (rightPortal)
        {
            if (col.gameObject.tag == "Player" && curVel.x > 0)
            {
                StartCoroutine(Teleoprt(1));
            }
        }
        else
        {
            if (col.gameObject.tag == "Player" && curVel.x < 0)
            {
                StartCoroutine(Teleoprt(-1));
            }
        }    
        
    }

    IEnumerator Teleoprt(float offset)
    {
        yield return new WaitForSeconds(0.3f);
        player.transform.position = new Vector2(portalReceiver.transform.position.x + offset, portalReceiver.transform.position.y);
        
    }

}
