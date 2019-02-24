using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public LevelController lController;
    public PlayerController pController;
    public StartButton start;

    // Start is called before the first frame update
    void Start()
    {
        lController = GameObject.FindGameObjectWithTag("turn").GetComponent<LevelController>();
        pController = lController.pController;
    }

    public void PlayGame()
    {
        pController.lController.levels = Levels.Level1;
        //StartCoroutine(CamPriority());
    }

    public void LoadGame()
    {
        Levels l = pController.LoadPlayer();

        if(l != Levels.MainMenu)
        {
            pController.gameObject.SetActive(true);
            pController.lController.levels = l;
        }
        else
        {
            Debug.Log("No Save File");
        }
        //StartCoroutine(CamPriority());
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }

    IEnumerator CamPriority()
    {
        yield return new WaitForSeconds(1f);
        start.computerCam.Priority = 15;
    }
}
