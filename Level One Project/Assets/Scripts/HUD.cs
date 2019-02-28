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
    public GameObject hud;

    //Heart images
    public Sprite heart1;
    public Sprite heart2;
    public Sprite heart3;
    public Sprite shield;
    public GameObject heartPrefab;

    Sprite[] heartSprites;
    //Health list
    List<GameObject> healthList;
    bool healthInitialized = false;

    //Text that shows the players stats
    public TextMeshProUGUI playerStats;

    //Text that shows what state the player is in
    public TextMeshProUGUI stateIndicator;

    //Text that shows messages for the player
    public TextMeshProUGUI errorMessage;

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

    private void InitializeHealth()
    {
        heartSprites = new Sprite[4] { heart3, heart2, heart1, shield };
        healthList = new List<GameObject>();
        healthInitialized = true;
    }

    public void HUDHealth()
    {
        if (!healthInitialized)
        {
            InitializeHealth();
        }

        GameObject[] hearts = GameObject.FindGameObjectsWithTag("hearts");
        foreach (GameObject obj in hearts)
        {
            healthList.Remove(obj);
            Destroy(obj);
        }

        for (int i = 0; i < pController.health; i++)
        {
            Vector3 pos = new Vector3(100 + (i * 25), 14, 0);

            GameObject health = Instantiate(heartPrefab, pos, Quaternion.identity);
            health.transform.SetParent(hud.transform, false);
            health.GetComponent<Image>().sprite = heartSprites[i];

            //Adds the object to healthList
            healthList.Add(health);
        }

        if (pController.shieldActive)
        {
            Vector3 pos = new Vector3(100 + (pController.health * 25), 14, 0);

            GameObject health = Instantiate(heartPrefab, pos, Quaternion.identity);
            health.transform.SetParent(hud.transform, false);
            health.GetComponent<Image>().sprite = heartSprites[3];
        }
    }

    public void DisplayStats()
    {
        switch (pController.state)
        {
            case States.WASD:
            case States.NavMesh:
                playerStats.text = $"Player Level: {pController.playerLevel}\nXP: {pController.playerXP}";
                break;
            case States.MeleeAttack:
                //TODO - Implement change in damage on stats
                playerStats.text = $"Player Level: {pController.playerLevel}\nXP: {pController.playerXP}\nDamage: 1";
                break;
            case States.RangeAttack:
                playerStats.text = $"Player Level: {pController.playerLevel}\nXP: {pController.playerXP}\nRemaining Mana: {pController.remainingMana}\nSpell Cost: {pController.attack.spellCost}";
                break;
        }
    }

    public IEnumerator DisplayError(string error)
    {
        errorMessage.text = error;
        errorMessage.CrossFadeAlpha(1, 0.5f, false);
        yield return new WaitForSeconds(1f);
        errorMessage.CrossFadeAlpha(0, 0.5f, false);
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
                            instructions.text = "Welcome to tutorial mode. Click \"I\" for more info. Click \"Q\" to quit tutorial mode.";
                            break;
                        }
                    case 2:
                        {
                            instructions.text = "Use WASD to move in free movment mode. Once you enter the combat zone, you will be presented with more options.";
                            break;
                        }
                    case 3:
                        {
                            instructions.text = "Combat Movement is based on clicks, presented by a line from the player to the mouse. Green means you can move, red means you can't move, and yellow means you are moving.";
                            break;
                        }
                    case 4:
                        {
                            instructions.text = "You also have 2 modes of attack, melee and range. These are based on clicks as well";
                            break;
                        }
                    case 5:
                        {
                            instructions.text = "Once in the combat zone you can use the scroll wheel Combat Movement, Melee Attack, and Range Attack.";
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
    }
}
