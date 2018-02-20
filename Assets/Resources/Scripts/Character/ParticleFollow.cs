using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleFollow : MonoBehaviour {

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

        playerPos.x += Mathf.Cos(heading.y);
        playerPos.z -= Mathf.Sin(heading.y);

        ParticleSystem ps = gameObject.GetComponent<ParticleSystem>();
        var shape = ps.shape;
        
        
        gameObject.transform.position = playerPos;

        gameObject.transform.LookAt(player.transform.position);

        shape.rotation = gameObject.transform.rotation.eulerAngles;

        //Quaternion newRot = Quaternion.Euler(0, player.transform.eulerAngles.y, 0);
        //gameObject.transform.rotation = Quaternion.Lerp(oldRot, newRot, Time.deltaTime * 10);
        oldRot = gameObject.transform.rotation;
    }
}
