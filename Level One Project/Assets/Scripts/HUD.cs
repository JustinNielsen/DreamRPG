using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Video;

public class HUD : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject settingMenu;
    public GameObject tutorial;
    public PlayerMovement pMovement;
    public Image manaBar;
    public Image xpBar;
    public Text playerStats;
    public LevelController lController;
    public PlayerController pController;
    public GameObject hud;

    public Image leftButton;
    //Heart images
    public Sprite heart1;
    public Sprite heart2;
    public Sprite heart3;
    public Sprite shield;
    public GameObject heartPrefab;

    public Image[] tutorialImages;

    Sprite[] heartSprites;
    //Health list
    List<GameObject> healthList;
    bool healthInitialized = false;
    public bool tutorialDone = false;
    public bool isGameWon;

    //Text that shows the players stats
    //public TextMeshProUGUI playerStats;

    //Text that shows what state the player is in
    public TextMeshProUGUI stateIndicator;

    //Text that shows messages for the player
    public TextMeshProUGUI errorMessage;

    public TextMeshProUGUI instructions;

    //public TextMeshProUGUI enemyStats;

    //The number is used to differentiate between instruction pieces.
    private int instructionNumber;
    //This flag is used to see if the instructions are accessed through the pause menu.
    private bool throughPause = false;

    private void Start()
    {
        instructionNumber = 1;
        isGameWon = false;
    }

    //Resumes the game from the pause menu
    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
        //Reactivates the player
        ReenablePlayer();
    }

    //Used on the pause menu to pause the game
    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
        //Deactivates player
        DisablePlayer();
    }

    //Used on the pause menu to get back to the main menu
    public void QuitGame()
    {
        Resume();

        //This resets the tutorial
        throughPause = false;
        TutorialReset();

        pController.movement.ToggleNavMesh(false);
        lController.levels = Levels.MainMenu;
    }

    //Decrease the mana bar by the inputed spell cost
    public void DecreaseManaBar(float spellCost)
    {
        //The mana bar fill amount shown on screen is only .25 of the whole thing
        //The increment amount uses the maxMana to figure out how much to increment
        //the mana bar for each point of mana.
        float incrementAmount = 0.25f / pController.maxMana;

        //Subtracts the spell cost from the remaining mana
        pController.remainingMana -= spellCost;

        //Fills the mana bar to the correct point. 
        //An empty mana bar is at .25 and a full mana bar is at .5
        manaBar.fillAmount = (pController.remainingMana * incrementAmount) + 0.25f;
    }

    //Initilizes the heart sprites array and the health list
    private void InitializeHealth()
    {
        heartSprites = new Sprite[4] { heart3, heart2, heart1, shield };
        healthList = new List<GameObject>();
        healthInitialized = true;
    }

    public void HUDHealth()
    {
        //If the heart sprites and list arn't initilized then do it here.
        //Had to do this because the start function wasn't running at the right time.
        if (!healthInitialized)
        {
            InitializeHealth();
        }

        //Destroys all the hearts from a privoius run of this array
        GameObject[] hearts = GameObject.FindGameObjectsWithTag("hearts");
        foreach (GameObject obj in hearts)
        {
            healthList.Remove(obj);
            Destroy(obj);
        }

        //Places all the hearts on the hud according to the player controllers health variable
        for (int i = 0; i < pController.health; i++)
        {
            Vector3 pos = new Vector3(100 + (i * 25), 14, 0);

            GameObject health = Instantiate(heartPrefab, pos, Quaternion.identity);
            health.transform.SetParent(hud.transform, false);
            health.GetComponent<Image>().sprite = heartSprites[i];

            //Adds the object to healthList
            healthList.Add(health);
        }

        //If the shield is active put it at the right most side of the hearts
        if (pController.shieldActive)
        {
            Vector3 pos = new Vector3(100 + (pController.health * 25), 14, 0);

            GameObject health = Instantiate(heartPrefab, pos, Quaternion.identity);
            health.transform.SetParent(hud.transform, false);
            health.GetComponent<Image>().sprite = heartSprites[3];
        }
    }

    //Updates all hud information. ie: health, stats, mana, and turn indicators
    public void UpdateHUD()
    {
        HUDHealth();
        DisplayStats();
        DecreaseManaBar(0);
        pController.turn.ResetArrays();        
    }

    //Displays the players stats according to the state that they are in
    //Obsolete. Delete?
    public void DisplayStats()
    {
        switch (pController.state)
        {
            case States.WASD:
            case States.NavMesh://Display xp bar and level
                xpBar.fillAmount = (float)pController.playerXP / 100.0f;
                playerStats.text = $"Level: {pController.playerLevel}";
                break;
            case States.MeleeAttack: //Display xp bar, level, and melee damage
                xpBar.fillAmount = (float)pController.playerXP / 100.0f;
                playerStats.text = $"Level: {pController.playerLevel}   Damage: {pController.attack.damage}";
                break;
            case States.RangeAttack: // Display xp bar, level, and range cost
                xpBar.fillAmount = (float)pController.playerXP / 100.0f;
                playerStats.text = $"Level: {pController.playerLevel}   Cost: {pController.attack.spellCost}";
                break;
            case States.Shielding: // Display xp bar, level, and shield cost
                xpBar.fillAmount = (float)pController.playerXP / 100.0f;
                playerStats.text = $"Level: {pController.playerLevel}   Cost: {40}";
                break;
        }
    }

    //Displays error messages to the player when they are out of mana and stuff like that
    //Takes in a string that is flashed in red at the top of the screen
    public IEnumerator DisplayError(string error)
    {
        errorMessage.text = error;
        errorMessage.CrossFadeAlpha(1, 0.5f, false);
        yield return new WaitForSeconds(1f);
        errorMessage.CrossFadeAlpha(0, 0.5f, false);
    }

    void Update()
    {
        //Makes sure that you can't open the pause menu when in the settings menu, gameover screen, or level up screen
        if (Input.GetKeyDown(KeyCode.Escape)  )//&& tutorialDone)
        {
            if (settingMenu.activeSelf == false && pController.levelUpMenu.activeSelf == false && pController.gameOver.activeSelf == false)
            {
                if (GameIsPaused)
                {
                    //This checks to see if the pause menu is turned on. It is used for the instructions/tutorial
                    throughPause = false;
                    TutorialReset();
                    Resume();
                }
                else
                {
                    throughPause = true;
                    Pause();
                }
            }

        }
    }

    public void Tutorial()
    {
        //When you finish the tutorial again.
        if (instructionNumber > 6)
        {
            //This is only if it is paused. This will make the pause menu come up again.
            TutorialReset();
        }
        else if (instructionNumber == 1)
        {
            tutorial.SetActive(true);
            pauseMenuUI.SetActive(false);
        }
           
        //Switch statement to walk through each piece of instruction
        switch (instructionNumber)
            {
                case 1:

                instructions.text = "Use WASD to move in free movement mode";
                tutorialImages[0].enabled = true;
                tutorialImages[1].enabled = false;
                tutorialImages[2].enabled = false;
                tutorialImages[3].enabled = false;
                leftButton.enabled = false;

                break;
                case 2:

                instructions.text = "Once you enter the combat zone, you can move by moving the mouse and clicking. The line shows your path, and is green when you can move to that spot. You only have a limited amount of movement per turn, so use it strategically!";
                //Changes the images
                tutorialImages[0].enabled = false;
                tutorialImages[1].enabled = true;
                tutorialImages[2].enabled = true;
                tutorialImages[3].enabled = true;
                tutorialImages[4].enabled = false;
                tutorialImages[5].enabled = false;

                if (!throughPause)
                {
                    //If the tutorial is accessed not through the pause menu, it will use this section of code
                    leftButton.enabled = false;
                }
                else
                {
                    leftButton.enabled = true;
                }
                
                break;
                case 3:

                instructions.text = "You can also use the mouse to use melee attacks. The attack will only hit when it is over the enemy. Be warned: You can only use one melee attack per turn.";
                //Changes the images... again
                tutorialImages[1].enabled = false;
                tutorialImages[2].enabled = false;
                tutorialImages[3].enabled = false;
                tutorialImages[4].enabled = true;
                tutorialImages[5].enabled = true;
                tutorialImages[6].enabled = false;
                tutorialImages[7].enabled = false;
                //This will be enabled regardless of the mode used, so don't worry about it.
                leftButton.enabled = true;

                break;
                case 4:

                instructions.text = "Another option for an attack is a ranged attack. You can use as many of these as you want per turn, assuming you have enough mana.";
                tutorialImages[4].enabled = false;
                tutorialImages[5].enabled = false;
                tutorialImages[6].enabled = true;
                tutorialImages[7].enabled = true;
                tutorialImages[8].enabled = false;

                break;
                case 5:

                instructions.text = "Your final option is to use a shield. A shield gives you one extra hit point, but costs mana to use. You can only have one shield active at a time.";
                tutorialImages[6].enabled = false;
                tutorialImages[7].enabled = false;
                tutorialImages[8].enabled = true;

                break;
                case 6:

                instructions.text = "Use the scroll wheel to switch between all of these options. Hit \"Esc\" to open up the pause menu, which will let you review these instructions, and hit \"Enter\" to end your turn.";
                tutorialImages[8].enabled = false;

                break;
            }
    }
    public void TutorialLeft()
    {
        if(instructionNumber > 1)
        {
            //Moves left
            instructionNumber--;
        }

        Tutorial();
    }

    public void TutorialRight()
    {
        if(instructionNumber < 7)
        {
            //Moves Right
            instructionNumber++;
        }

        Tutorial();
    }

    private void TutorialReset()
    {
        //This will deactivate the tutorial stuff
        tutorial.SetActive(false);

        if (throughPause)
        {
            //Resets the instruction flag if this is not accessed through the pause menu.
            pauseMenuUI.SetActive(true);
        }

        //Resets the instruction number.
        instructionNumber = 1;
    }

    public void WinGame()
    {
        //Resets the players level and stats
        pController.playerLevel = 1;
        pController.playerXP = 0;
        pController.attack.damage = 2;
        pController.attack.spellCost = 20;

        //Sets the level to the main menu, freezes time, and deactivates the hud.
        lController.backAudio.Stop();
        //pController.lController.hud.SetActive(false);


        isGameWon = true;

    }

    //Reenables the player
    private void ReenablePlayer()
    {
        pMovement.enabled = true;
        pController.enabled = true;
        pController.attack.enabled = true;
    }

    //Disables the player
    private void DisablePlayer()
    {
        pMovement.enabled = false;
        pController.enabled = false;
        pController.attack.enabled = false;
    }


}
