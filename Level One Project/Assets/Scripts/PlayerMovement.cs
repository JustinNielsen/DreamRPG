using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System;

public class PlayerMovement : MonoBehaviour
{
    Camera cam;
    public NavMeshAgent agent;
    GameObject player;
    GameObject waypoint;
    GameObject waypointPrefab;
    public float movementSpeed = 8f;
    public bool isMoving = false;
    Vector3 movingTarget;
    Vector3 clickedTarget;
    public NavMeshPath path;
    float pathLength;
    public LineRenderer line;
    PlayerController pControl;
    public float maxDistance;
    GameObject moveDirection;
    Vector3 moveInput;
    Vector3 moveVelocity;
    public Rigidbody rb;
    bool checkingIfStuck;
    Ray ray;
    public Animator anim;
    bool activeFlag = true;
    GameObject footprints;

    //Find speed
    Vector3 previousPosition;
    float curSpeed;
    float previousDistance;
    float distanceMoved;

    Coroutine rockStuck;

    // Start is called before the first frame update
    void Start()
    {
        //Initialize cam to main camera
        cam = Camera.main;
        //Get the player object
        player = GameObject.FindGameObjectWithTag("player");
        //Get the navMeshAgent from the player object
        agent = player.GetComponent<NavMeshAgent>();
        //Get lineRenderer off player
        line = player.GetComponent<LineRenderer>();
        //Initialize the path to null
        path = new NavMeshPath();
        //Get PlayerController off of player
        pControl = GetComponent<PlayerController>();
        //Initialize maxDistance from the public variable on the playerController
        maxDistance = pControl.maxDistance;
        //Initialize object to base movement from
        moveDirection = GameObject.FindGameObjectWithTag("turn");
        //Get the rigidbody from the player object
        rb = player.GetComponent<Rigidbody>();
        //Initialize checking if stuck to false
        checkingIfStuck = false;
        //Initialize footprints
        footprints = GameObject.FindGameObjectWithTag("footprints");
    }

    private void FixedUpdate()
    {
        if (pControl.state == States.NavMesh && isMoving && agent.path != null)
        {

            previousDistance = agent.remainingDistance;

            if (!checkingIfStuck)
            {
                checkingIfStuck = true;
                StartCoroutine(StuckCheck());
                Debug.Log("Remaining Distance 1: " + agent.remainingDistance);
            }
        }
    }

    void Update()
    {

        //If the current state is WASD
        if (pControl.state == States.WASD)
        {
            //Gets input from keys
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            //Sets velocity from input
            moveInput = new Vector3(horizontal, 0f, vertical);

            //Get camera rotation and forward values
            Vector3 cameraForward = cam.transform.forward;
            cameraForward.y = 0f;
            Quaternion cameraRelativeRotation = Quaternion.FromToRotation(Vector3.forward, cameraForward);
            Vector3 lookToward = cameraRelativeRotation * moveInput;

            //Use camera values to turn the player when input is being pressed
            if (moveInput.sqrMagnitude > 0)
            {
                Ray lookRay = new Ray(transform.position, lookToward);
                transform.LookAt(lookRay.GetPoint(1));
            }

            //If the magnitudes is greater than 1 divide move input by its magnitude
            if (moveInput.magnitude > 1)
            {
                moveInput /= moveInput.magnitude;
            }

            //Sets the move velocity variable to be used later on the rigidBody
            moveVelocity = transform.forward * movementSpeed * moveInput.sqrMagnitude;
        }
        else
        {
            //If the states is not WASD the rigidbodys velocity is set to zero
            rb.velocity = Vector3.zero;
        }

        //Look at the mouse if in navmesh state, the players is not moving, and the player is not dead
        if (!isMoving && pControl.state == States.NavMesh && pControl.health != 0 && pControl.active)
        {
            pControl.attack.LookAtMouse();
        }

        //Input for moving the player with mouse clicks
        if (pControl.state == States.NavMesh)
        {
            //Triggers when the player clicks with the left mouse button
            if (Input.GetMouseButtonDown(0))
            {
                //Convertn mousePosition from a screen point to a ray
                ray = cam.ScreenPointToRay(Input.mousePosition);

                //Defines which layers to ignore with the raycast
                int layerMask = 1 << 11;

                //Makes sure aniation isn't stuck
                anim.SetTrigger("ExitCombat");

                //sets clicked target to the location the ray hits
                ShootRayClicked(ray, layerMask);
            }
        }

        //Stops your movement 1.5 seconds after your turn is over.
        if (!pControl.active && activeFlag)
        {
            StartCoroutine(EndMovement());
            activeFlag = false;
        }

        //Resets the active flag
        if (pControl.active && !activeFlag)
        {
            activeFlag = true;
        }
    }

    //Method for the combat movement
    public void NavMeshMovement()
    {
        //Declare a Ray
        //Ray ray;
        //Shows line
        line.enabled = true;

        //Check if the player is done moving
        if (maxDistance < 0.5f && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0 && Vector3.Distance(transform.position, agent.destination) <= 1f)
        {
            //Hides Line
            line.positionCount = 0;

        }

        if (Vector3.Distance(transform.position, agent.destination) <= 0.1f)
        {
            //Sets isMoving to false
            isMoving = false;

            if (rockStuck != null)
            {
                StopCoroutine(rockStuck);
            }
        }

        if (clickedTarget != null)
        {
            line.material.color = Color.yellow;
            //Updates the path
            NavMesh.CalculatePath(transform.position, clickedTarget, NavMesh.AllAreas, path);
            //Draws updated path
            DrawPath(path, 3);
        }


        //Triggers if the player isn't moving
        if (!isMoving)
        {
            //Convertn mousePosition from a screen point to a ray
            ray = cam.ScreenPointToRay(Input.mousePosition);

            //Defines which layers to ignore with the raycast
            int layerMask = 1 << 11;

            //sets clicked target to the location the ray hits
            movingTarget = ShootRay(ray, layerMask);

        }

        //Set the speed for the animations
        anim.SetFloat("Speed", (agent.velocity.magnitude / pControl.gameObject.transform.localScale.y));
    }

    //Only draws the path doesn't set the destination for the NavMeshAgent
    private Vector3 ShootRay(Ray ray, int layerMask)
    {
        //Declare a hit variable 
        RaycastHit hit;


        //Check if the point the player clicked is possible to move to
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            //Set clicked target to the hit point
            clickedTarget = hit.point;

            //Creates a NavMeshPath
            NavMesh.CalculatePath(transform.position, hit.point, NavMesh.AllAreas, path);

            //Calculate the length of the path
            pathLength = CalculatePathLength(path);

            //If the point clicked is within the max move distance then place a waypoint and move towards it
            if (pathLength < maxDistance)
            {
                //Draws the path
                DrawPath(path, 1);

            }
            else
            {
                //Draws the path
                DrawPath(path, 2);
            }
        }
        else
        {
            line.positionCount = 0;
        }

        return hit.point;
    }

    //Draws the path and sets the destination for the NavMeshAgent
    private void ShootRayClicked(Ray ray, int layerMask)
    {
        //Declare a hit variable 
        RaycastHit hit;

        //Check if the point the player clicked is possible to move to
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            //Creates a NavMeshPath
            NavMesh.CalculatePath(transform.position, hit.point, NavMesh.AllAreas, path);

            //Find the length of the path to the clicked point
            pathLength = CalculatePathLength(path);

            //Test if path length is messed up
            Debug.Log(pathLength);

            //Secondary check for the distance that is less refined
            float distance = Vector3.Distance(player.transform.position, hit.point);

            //If the point clicked is within the max move distance then place a waypoint and move towards it
            if (pathLength < maxDistance && distance < maxDistance)
            {
                //Draws the path
                DrawPath(path, 1);

                //Subtract the distance moved from the maxDistance
                maxDistance -= pathLength;

                //Move to clicked point
                agent.SetDestination(clickedTarget);

                //Set isMoving to true
                isMoving = true;
                rockStuck = StartCoroutine(UnstuckOnRocks());
            }
            else
            {
                //Notifies the player that they can't move to that location
                StartCoroutine(pControl.hud.DisplayError("Too Far Away"));

                //Draws the path
                DrawPath(path, 2);
            }
        }
    }

    //Calculates the length of the navMesh Path
    private float CalculatePathLength(NavMeshPath meshPath)
    {
        //If the path has less than 2 corners return 0
        if (path.corners.Length < 2)
        {
            return 0;
        }

        Vector3 previousCorner = meshPath.corners[0];
        float totalLength = 0.0f;

        //Calculate the length between all the corners and add them to the totalLength
        for (int i = 1; i < meshPath.corners.Length; i++)
        {
            Vector3 currentCorner = meshPath.corners[i];
            totalLength += Vector3.Distance(previousCorner, currentCorner);
            previousCorner = currentCorner;
        }

        return totalLength;
    }

    private void DrawPath(NavMeshPath meshPath, int isGood)
    {
        //Deletes the line
        line.positionCount = 0;

        //Changes the color of the line based on the inputed number
        if (isGood == 1)
        {
            line.material.color = Color.green;
        }
        else if (isGood == 2)
        {
            line.material.color = Color.red;
        }

        //Sets the amount of corners for the line
        line.positionCount = meshPath.corners.Length;

        //Sets the position of each corner
        for (int i = 0; i < meshPath.corners.Length; i++)
        {
            line.SetPosition(i, path.corners[i]);
        }
       
        angle = Quaternion.Euler(90, line.transform.rotation.y + 90, line.transform.rotation.z);
        obj = (GameObject)Instantiate(footprints, line.transform.position, angle);
        obj.transform.parent = line.transform;
    }

    GameObject obj;
    Quaternion angle;

    public void KeyboardMovement()
    {
        //Applies velocity to rigidbody
        rb.velocity = moveVelocity;
        anim.SetFloat("Speed", (rb.velocity.magnitude / pControl.gameObject.transform.localScale.y));
    }

    //Turns on or off the navMesh according to the bool parameter
    public void ToggleNavMesh(bool isOn)
    {
        //if isOn = true then turn on the players NavMeshAgent, and sets bool navMesh to true.
        if (isOn)
        {
            agent.enabled = true;
            pControl.state = States.NavMesh;
        }
        else //Sets everything stated above to false if isOn = false
        {
            agent.enabled = false;
            pControl.state = States.WASD;
            line.positionCount = 0;
            Destroy(pControl.attack.hitbox);
            pControl.lController.fightSongActive = false;
        }
    }

    //Disables the players path if they are stuck
    IEnumerator StuckCheck()
    {
        float minMoveDistance;

        //If the current speed is still less than one after 1 second the players current path will be reset
        yield return new WaitForSeconds(1f);

        distanceMoved = previousDistance - agent.remainingDistance;

        if (pControl.lController.levels == Levels.Space)
        {
            minMoveDistance = 0.001f;
        }
        else
        {
            minMoveDistance = 0.01f;
        }

        if (pControl.state == States.NavMesh && agent.path != null && isMoving && distanceMoved < minMoveDistance || agent.remainingDistance == Mathf.Infinity)
        {
            //Adds the remaining distance back to the maxDistance variable
            Debug.Log("Remaining Distance 2: " + agent.remainingDistance);

            if (agent.remainingDistance != Mathf.Infinity)
            {
                maxDistance += agent.remainingDistance;
            }
            else
            {
                maxDistance += 5f;
            }

            agent.ResetPath();
        }

        checkingIfStuck = false;
    }

    private IEnumerator EndMovement()
    {
        yield return new WaitForSeconds(1.5f);
        agent.ResetPath();
    }

    //This might get us unstuck on rocks. and other stuff.
    private IEnumerator UnstuckOnRocks()
    {
        yield return new WaitForSeconds(7);
        if (agent.path != null)
        {
            agent.ResetPath();
        }
    }
}
