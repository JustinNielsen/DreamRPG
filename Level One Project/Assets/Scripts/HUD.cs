using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject settingMenu;
    public PlayerMovement pMovement;
    public Image manaBar;
    public LevelController lController;

    private float remainingMana;

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void QuitGame()
    {
        Debug.Log("Quitting Game...");
        lController.levels = Levels.MainMenu;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingMenu.activeSelf == false)
            {
                if (GameIsPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
            else
            {
                settingMenu.SetActive(false);
                Resume();
            }
        }

        if (pMovement.maxDistance != remainingMana)
        {
            manaBar.fillAmount = (pMovement.maxDistance * 0.025f) + 0.25f;
            remainingMana = pMovement.maxDistance;
        }
    }
}
