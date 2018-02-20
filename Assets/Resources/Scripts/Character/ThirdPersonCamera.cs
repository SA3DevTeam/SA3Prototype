using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour {

    // Update is called once per frame
    public bool follow = true;
    public Vector3 pos = Vector3.zero;
    private Quaternion oldRot;
    private GameObject player;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    private void FixedUpdate()
    {
        
        Vector3 playerPos = player.transform.position;
        Vector3 heading = player.transform.eulerAngles * 3.14f / 180;
        //The camera is following sonic, use his relative rotation to place the camera behind him 
        if (follow)
        {
            playerPos.y += 5;
            playerPos.x += Mathf.Cos(heading.y) * 10;
            playerPos.z += Mathf.Sin(heading.y) * -10;
        }
        else playerPos = pos;

        gameObject.transform.position = Vector3.Slerp(gameObject.transform.position, playerPos, Time.deltaTime * 5);

        gameObject.transform.LookAt(player.transform.position);

        //Quaternion newRot = Quaternion.Euler(0, player.transform.eulerAngles.y, 0);
        //gameObject.transform.rotation = Quaternion.Lerp(oldRot, newRot, Time.deltaTime * 10);
        oldRot = gameObject.transform.rotation;
    }
}
