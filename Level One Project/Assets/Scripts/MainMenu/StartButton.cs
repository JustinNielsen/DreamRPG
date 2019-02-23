using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Cinemachine;
using UnityEngine.UI;

public class StartButton : MonoBehaviour
{
    private TextMeshPro text;
    public Material normalColor;
    public Material hoverColor;
    public Material clickColor;
    private bool hover;
    public GameObject menu;
    public CinemachineVirtualCamera computerCam;
    Camera cam;
    CinemachineBrain camBrain;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshPro>();
        cam = Camera.main;
        camBrain = cam.GetComponent<CinemachineBrain>();
    }

    private void OnMouseEnter()
    {
        text.color = hoverColor.color;
        hover = true;
        Debug.Log("in");
    }

    private void OnMouseOver()
    {
        Debug.Log("over");
        //Change camera on click and button color
        if (Input.GetMouseButtonDown(0))
        {
            camBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.EaseInOut;
            computerCam.Priority = 25;
            text.color = clickColor.color;
            StartCoroutine(NormalColor());
        }
    }

    private void OnMouseExit()
    {
        text.color = normalColor.color;
        hover = false;
        Debug.Log("out");
    }

    IEnumerator NormalColor()
    {
        yield return new WaitForSeconds(0.1f);

        if (hover)
        {
            text.color = hoverColor.color;
        }
        else
        {
            text.color = normalColor.color;
        }

        yield return new WaitForSeconds(1.85f);

        menu.SetActive(true);
    }

    //Changes the camera to the room camera
    public void BackToRoom()
    {
        menu.SetActive(false);
        computerCam.Priority = 15;
    }
}
