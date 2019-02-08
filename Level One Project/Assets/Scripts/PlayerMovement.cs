using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerMovement : MonoBehaviour
{
    private Camera cam;
    private GameObject player;
    private GameObject waypoint;
    public GameObject waypointPrefab;
    public NavMeshAgent agent;
    private float movementSpeed = 8f;
    private bool isMoving = false;
    private Vector3 movingTarget;
    private Vector3 clickedTarget;
    private NavMeshPath path;
    private float pathLength;
    private LineRenderer line;
    private PlayerController pControl;
    private float maxDistance;

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
        //Initiaize prefab
        waypointPrefab = GameObject.Find("waypoint");
    }

    public void NavMeshMovement()
    {
        //Declare a Ray
        Ray ray;

        //Check if the player is done moving
        if (maxDistance < 0.5f && agent.pathStatus == NavMeshPathStatus.PathComplete && agent.remainingDistance == 0 && Vector3.Distance(transform.position, agent.destination) <= 1f)
        {
            //Sets the maxDistance back to 10 but doesn't update the scele of the range indicator yet.
            maxDistance = 10.0f;
            //Switch Turns
            pControl.turn.SwitchTurn();
        }

        if (Vector3.Distance(transform.position, agent.destination) <= 1f)
        {
            //Sets isMoving to false
            isMoving = false;
        }

        if (clickedTarget != null)
        {
            //Updates the path
            NavMesh.CalculatePath(transform.position, clickedTarget, NavMesh.AllAreas, path);
            //Draws updated path
            DrawPath(path, 3);
        }

        //Triggers when the player clicks with the left mouse button
        if (Input.GetMouseButtonDown(0))
        {
            //Convertn mousePosition from a screen point to a ray
            ray = cam.ScreenPointToRay(Input.mousePosition);

            //Defines which layers to ignore with the raycast
            int layerMask = 1 << 11;

            //sets clicked target to the location the ray hits
            ShootRayClicked(ray, layerMask);
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

            //Debug.Log("Line");
        }
    }

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

                Debug.Log("In Range");

            }
            else
            {
                Debug.Log("Out of Range");

                //Draws the path
                DrawPath(path, 2);
            }

            //Debug.Log("Path Length: " + pathLength.ToString("n2"));
            //Debug.Log("Max Distance: " + maxDistance);
        }

        return hit.point;
    }

    private void ShootRayClicked(Ray ray, int layerMask)
    {
        //Declare a hit variable 
        RaycastHit hit;

        //Check if the point the player clicked is possible to move to
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
        {
            //Creates a NavMeshPath
            NavMesh.CalculatePath(transform.position, hit.point, NavMesh.AllAreas, path);

            pathLength = CalculatePathLength(path);

            //If the point clicked is within the max move distance then place a waypoint and move towards it
            if (pathLength < maxDistance)
            {
                //Draws the path
                DrawPath(path, 1);

                //Check if a waypoint is already placed. If so Destroy it.
                if (waypoint != null)
                {
                    Destroy(waypoint);
                }

                //Place a waypoint
                waypoint = Instantiate(waypointPrefab, clickedTarget, Quaternion.identity);

                //Subtract the distance moved from the maxDistance
                maxDistance -= pathLength;

                //Move to clicked point
                agent.SetDestination(clickedTarget);

                //Set isMoving to true
                isMoving = true;

            }
            else
            {
                Debug.Log("Out of Range");

                //Draws the path
                DrawPath(path, 2);
            }

            //Debug.Log("Path Length: " + pathLength.ToString("n2"));
            //Debug.Log("Max Distance: " + maxDistance);
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
        line.positionCount = 0;

        if (isGood == 1)
        {
            line.material.color = Color.green;
            Debug.Log("Green");
        }
        else if (isGood == 2)
        {
            line.material.color = Color.red;
            Debug.Log("Red");
        }

        line.positionCount = meshPath.corners.Length;

        for (int i = 0; i < meshPath.corners.Length; i++)
        {
            line.SetPosition(i, path.corners[i]);
        }
    }

    public void KeyboardMovement()
    {
        //moveInput = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
        //moveVelocity = moveInput * movementSpeed;

        //Move Up and down
        player.transform.position += new Vector3(cam.transform.forward.x, 0, cam.transform.forward.z) * Time.deltaTime * movementSpeed * Input.GetAxis("Vertical");
        //Move Left and Right
        player.transform.position += new Vector3(cam.transform.right.x, 0, cam.transform.right.z) * Time.deltaTime * movementSpeed * Input.GetAxis("Horizontal");
    }

    //Turns on or off the navMesh according to the bool parameter
    private void ToggleNavMesh(bool isOn)
    {
        //if isOn = true then turn on the range circle indicator, the players NavMeshAgent, and sets bool navMesh to true.
        if (isOn)
        {
            //NavMeshAgent and 
            agent.enabled = true;
            pControl.state = States.NavMesh;
        }
        else //Sets everything stated above to false if isOn = false
        {
            //rangeRen.enabled = false;
            agent.enabled = false;
            pControl.state = States.Neutral;
        }
    }
}
