using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RangeAttack : MonoBehaviour
{
    GameObject player;
    ControlPlayer pController;
    public bool active = false;
    Camera cam;
    public GameObject mageShot;
    LineRenderer line;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        player = GameObject.FindGameObjectWithTag("player");
        pController = player.GetComponent<ControlPlayer>();
        line = player.GetComponent<LineRenderer>();
        line.material.color = Color.red;
    }

    // Update is called once per frame
    void Update()
    {
        if (active)
        {
            Ray cameraRay = cam.ScreenPointToRay(Input.mousePosition);
            int layerMask = 1 << 14;

            RaycastHit rayHit;

            if(Physics.Raycast(cameraRay, out rayHit, Mathf.Infinity, layerMask))
            {
                if(rayHit.collider.tag == "ground")
                {
                    Vector3 pointToLook = new Vector3(rayHit.point.x, transform.position.y, rayHit.point.z);
                    player.transform.LookAt(pointToLook);
                }
            }

            line.positionCount = 2;
            line.SetPosition(0, transform.position);
            //line.SetPosition(1, transform.forward * 20 + transform.position);
            RaycastHit hit;

            if(Physics.Raycast(transform.position, transform.forward, out hit, 20f))
            {
                line.SetPosition(1, hit.point);
            }
            else
            {
                line.SetPosition(1, transform.forward * 20 + transform.position);
            }

            if (Input.GetMouseButtonDown(0))
            {
                LaunchProjectile();
            }
        }
    }

    public void RangeAttackMode()
    {
        Ray cameraRay = cam.ScreenPointToRay(Input.mousePosition);
        int layerMask = 1 << 14;

        RaycastHit rayHit;

        if (Physics.Raycast(cameraRay, out rayHit, Mathf.Infinity, layerMask))
        {
            if (rayHit.collider.tag == "ground")
            {
                Vector3 pointToLook = new Vector3(rayHit.point.x, transform.position.y, rayHit.point.z);
                player.transform.LookAt(pointToLook);
            }
        }

        line.positionCount = 2;
        line.SetPosition(0, transform.position);
        //line.SetPosition(1, transform.forward * 20 + transform.position);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.forward, out hit, 20f))
        {
            line.SetPosition(1, hit.point);
        }
        else
        {
            line.SetPosition(1, transform.forward * 20 + transform.position);
        }

        if (Input.GetMouseButtonDown(0))
        {
            LaunchProjectile();
        }
    }

    private void LaunchProjectile()
    {
        Vector3 pos = transform.position + transform.forward;
        GameObject projectile = Instantiate(mageShot, pos, transform.rotation);
        active = false;
        pController.state = States.NavMesh;
        Destroy(projectile, 4f);
    }
}
