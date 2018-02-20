using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    // Use this for initialization
    private Rigidbody rb;
    public int speed; // How fast sonic runs
    public int rotationSpeed; // How fast sonic can turn
    public int jumpSpeed; // How High sonic can jump
    private bool grounded; // is sonic touching the ground?
    private bool looping; // is sonic touching a loop? probably depricated
    private Time start; // ???
    private bool HomingAttackOK = false; // If in the air and space bar released
    private bool HAStage1 = false; // if in the air
    private Vector3 prevFloorNormal = new Vector3(0,1,0); // what floor was I standing on?
    private RaycastHit hitD; // what is bellow sonic?
    private Quaternion newRot = Quaternion.Euler(1,1,0); // default orientation
    public ParticleSystem ps;

    void Start () {
        rb = gameObject.GetComponent<Rigidbody>();
        grounded = true; // can't jump when game starts
	}
	
	// Update is called once per frame
	void Update () {

        float valueH = Input.GetAxis("Horizontal");
        float valueV = Input.GetAxis("Vertical");
        float jumpV = Input.GetAxis("Jump");
        float reduce = 1;

        float velocity = Vector3.Distance(Vector3.zero, rb.velocity);
        var mm = ps.main;
        mm.startSize3D = true;
        mm.startSizeY = velocity;
        mm.startRotation3D = true;
        mm.startRotationZ = transform.eulerAngles.y;
        Debug.Log(mm.startRotationZ.curveMax + ", " + transform.eulerAngles.y);
        var em = ps.emission;
        em.rateOverTime = Mathf.Pow(velocity, 2) / 5;
       

        //if (!grounded && !looping) reduce = 10; // reduces sonics movement when he's in the air

        //This statment gets the normal vector of the surface sonic is standing on and set his upward orientation relevnt to it while
        // maintaining his heading
        if (Physics.Raycast(transform.position, -transform.up, out hitD, 3)){
            if (hitD.collider.tag == "Loop")
            {
                newRot = Quaternion.FromToRotation(transform.up, hitD.normal) * transform.rotation;
                transform.rotation = newRot;//Quaternion.Slerp(transform.rotation, newRot, Time.deltaTime * 30);
            }
        }

        transform.Rotate(new Vector3(0, rotationSpeed * valueH,0));
        //Moves sonic forward and backwards
        rb.AddRelativeForce(new Vector3(((speed * valueV * -1) / reduce), 0, 0));

        //Detects if sonic is on the ground or on a loop and the jump button is pressed
        //This lets sonic jump
        if ((grounded || looping) && jumpV == 1 && !HAStage1)
        {
            rb.AddRelativeForce(new Vector3(0, jumpSpeed * jumpV, 0), ForceMode.Impulse);
            HAStage1 = true;
        }
        //Readies the homing attack if the space bar is released while sonic is in the air
        if(jumpV == 0 && HAStage1)
        {
            Debug.Log("Homing Ready");
            HomingAttackOK = true;
            HAStage1 = false;
        }
        //if sonic is in free fall we reorientate him to face upwards
        if (!grounded && !looping)
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(0, transform.eulerAngles.y, 0), Time.deltaTime * 10);
            rb.AddForce(Physics.gravity);

            // executes a homing attack if the space bar is pressed, released and pressed again while sonic is in midair
            if (HomingAttackOK && jumpV == 1)
            {
                Debug.Log("Homing!");
                rb.AddRelativeForce(new Vector3(jumpSpeed * -jumpV * 2, 0, 0), ForceMode.Impulse);
                HomingAttackOK = false;
                HAStage1 = false;
            }
        }

    }
    
    void OnCollisionEnter(Collision hit)
    {
        //Sonic has hit the floor! reset some bools for the game state and orientate him to up.
        if (hit.gameObject.tag == "Floor")
        {
            grounded = true;
            HomingAttackOK = false;
            transform.eulerAngles = new Vector3(0, transform.eulerAngles.y, 0);
        }
        //Sonic has hit a loop! reset some bools for the game state.
        if (hit.gameObject.tag == "Loop")
        {
            grounded = true;
            HomingAttackOK = false;
        }
        
    }

    void OnCollisionExit(Collision collision)
    {
        //If sonic is no longer touching the floor or the ground sonic will no longer be grounded
        if(collision.gameObject.tag == "Floor" || collision.gameObject.tag == "Loop")
        {
            looping = false;
            grounded = false;
        }
        
    }

    void OnTriggerEnter(Collider hit)
    {
        //Sonic hit a camera box, tell the camera to stop following the player and move the camera to the new position
        if (hit.gameObject.tag == "camFix")
        {
            Debug.Log("Enter cam region");
            ThirdPersonCamera cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ThirdPersonCamera>();
            cam.follow = false;
            cam.pos = hit.gameObject.transform.GetChild(0).gameObject.transform.position;
        }
    }
    private void OnTriggerExit(Collider hit)
    {
        //Sonic left the camera box, tell the camera to follow sonic.
        if (hit.gameObject.name == "camFix")
        {
            ThirdPersonCamera cam = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<ThirdPersonCamera>();
            cam.follow = true;
        }
    }
}
