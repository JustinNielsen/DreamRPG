using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject settingMenu;
    public PlayerMovement pMovement;
    public Image manaBar;
    public LevelController lController;
    public PlayerController pController;
    public TextMeshProUGUI instructions;
    //The number is used to differentiate between instruction pieces.
    private int instructionNumber;
    //The flag is used to determine if we actually need to continue the instructions.
    private bool instructionFlag;
    private void Start()
    {
        instructionNumber = 1;
        instructionFlag = true;
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
        pController.movement.ToggleNavMesh(false);
        lController.levels = Levels.MainMenu;
    }

    public void DecreaseManaBar(float spellCost)
    {
        float incrementAmount = 0.25f / pController.maxMana;
        pController.remainingMana -= spellCost;

        manaBar.fillAmount = (pController.remainingMana * incrementAmount) + 0.25f;
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
        //Continues the instructions
        if (Input.GetKeyDown(KeyCode.I))
        {
            //Checks to see if we should continue instructions
            if (instructionFlag)
            {
                //First we go to the next instruction tidbit
                instructionNumber++;
                //Switch statement to walk through each piece of instruction
                switch (instructionNumber)
                {
                    case 1:
                        {
                            instructions.text = "Welcome to the tutorial mode. Click I for more info. Click Q to quit tutorial mode.";
                            break;
                        }
                    case 2:
                        {
                            instructions.text = "WASD to move. Once you enter the combat zone, you will be presented with more options.";
                            break;
                        }
                    case 3:
                        {
                            instructions.text = "Movement will be based on clicks, presented by a green line.";
                            break;
                        }
                    case 4:
                        {
                            instructions.text = "You will also have 2 modes of attack, melee and range. These are based on clicks as well";
                            break;
                        }
                    case 5:
                        {
                            instructions.text = "Use the scroll wheel to switch between all of these options.";
                            break;
                        }

                }

            }
        }

        //This disables the instructions
        if (Input.GetKeyDown(KeyCode.Q))
        {
            instructionFlag = false;
        }

        /*if (pMovement.maxDistance != remainingMana)
        {
            manaBar.fillAmount = (pMovement.maxDistance * 0.025f) + 0.25f;
            remainingMana = pMovement.maxDistance;
        }*/
    }
}
