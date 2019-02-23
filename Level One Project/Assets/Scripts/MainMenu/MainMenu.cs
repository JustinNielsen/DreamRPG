using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public LevelController lController;
    public StartButton start;

    // Start is called before the first frame update
    void Start()
    {
        lController = GameObject.FindGameObjectWithTag("turn").GetComponent<LevelController>();
    }

    public void PlayGame()
    {
        lController.levels = Levels.Level1;
        start.computerCam.Priority = 15;
    }

    public void LoadGame()
    {
        lController.LoadPlayer();
        start.computerCam.Priority = 15;
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }
}
