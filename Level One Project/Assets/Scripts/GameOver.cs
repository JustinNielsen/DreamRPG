using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    public PlayerController pController;
    public GameObject gameOver;

    public void Continue()
    {
        pController.health = 3;
        pController.movement.ToggleNavMesh(false);
        pController.hud.DisplayStats();
        pController.turn.ResetArrays();
        //gameOver.SetActive(false);
        pController.lController.levels = pController.LoadPlayer();
        pController.lController.gameOverLoadPlayer = true;
        Time.timeScale = 1f;
        gameOver.SetActive(false);
    }

    public void Quit()
    {
        pController.health = 3;
        pController.movement.ToggleNavMesh(false);
        Time.timeScale = 1f;
        //gameOver.SetActive(false);
        pController.lController.levels = Levels.MainMenu;
    }
}
