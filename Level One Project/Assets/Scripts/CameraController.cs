using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    public GameObject[] Characters;
    public int objFocus = 0;
    //public GameObject player;
    //public GameObject enemy;

    private CinemachineVirtualCamera cam;

    void Start()
    {
        //Characters = new GameObject[2] { player, enemy };
        cam = this.GetComponent<CinemachineVirtualCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            if(objFocus < Characters.Length - 1)
            {
                objFocus++;
            }
            else
            {
                objFocus = 0;
            }

            cam.Follow = Characters[objFocus].transform;
            cam.LookAt = Characters[objFocus].transform;
        }
    }
}
