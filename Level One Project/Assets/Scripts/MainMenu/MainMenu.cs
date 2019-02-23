using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public LevelController lController;

    // Start is called before the first frame update
    void Start()
    {
        lController = GameObject.FindGameObjectWithTag("turn").GetComponent<LevelController>();
    }

    public void PlayGame()
    {
        lController.levels = Levels.Level1;
    }

    public void LoadGame()
    {
        lController.LoadPlayer();
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game");
        Application.Quit();
    }
}
