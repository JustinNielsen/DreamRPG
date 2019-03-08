using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOver : MonoBehaviour
{
    public ControlPlayer pController;
    public GameObject gameOver;

    public void Continue()
    {
        pController.health = 3;
        pController.remainingMana = 100;
        pController.movement.ToggleNavMesh(false);
        pController.hud.DisplayStats();
        pController.turn.ResetArrays();
        //gameOver.SetActive(false);
        pController.hud.DecreaseManaBar(0);
        pController.lController.levels = pController.LoadPlayer();
        pController.lController.gameOverLoadPlayer = true;
        Time.timeScale = 1f;
        gameOver.SetActive(false);
    }

    public void Quit()
    {
        pController.health = 3;
        pController.movement.ToggleNavMesh(false);
        pController.hud.DisplayStats();
        pController.turn.ResetArrays();
        Time.timeScale = 1f;
        //gameOver.SetActive(false);
        pController.lController.levels = Levels.MainMenu;
    }
}
