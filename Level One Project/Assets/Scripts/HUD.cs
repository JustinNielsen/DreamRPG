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
    public PlayerController pController;

    private void Start()
    {

    }

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
        Resume();
        lController.levels = Levels.MainMenu;
    }

    public void DecreaseManaBar(float spellCost)
    {
        float incrementAmount = 0.25f / pController.maxMana;
        pController.remainingMana -= spellCost;

        manaBar.fillAmount -= incrementAmount * spellCost;
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

        /*if (pMovement.maxDistance != remainingMana)
        {
            manaBar.fillAmount = (pMovement.maxDistance * 0.025f) + 0.25f;
            remainingMana = pMovement.maxDistance;
        }*/
    }
}
