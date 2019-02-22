using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Creates an enum for each level scene
public enum Levels { MainMenu, Level1, Level2, Level3 };

public class LevelController : MonoBehaviour
{
    public GameObject player;
    public GameObject hud;
    public Camera cam;

    //This enum will be used to track the level it should be
    public Levels levels;
    private Levels currentLevel;
    private TurnBasedSystem turn;
    private PlayerController pController;

    //Audio things
    AudioSource backAudio;
    public AudioClip mainMenu;
    public AudioClip level1;
    public AudioClip level2;
    public AudioClip level3;
    public AudioClip fightSong;

    bool fightSongActive = false;

    // Start is called before the first frame update
    void Start()
    {
        //sets the levels enum
        levels = Levels.MainMenu;
        //Gets a reference to the playerController
        pController = player.GetComponent<PlayerController>();
        //Gets a reference to the turnBasedSystemScript in the playerController
        turn = GameObject.FindGameObjectWithTag("turn").GetComponent<TurnBasedSystem>();
        currentLevel = Levels.Level1;
        backAudio = cam.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        //This will make sure that the scene is loaded only once.
        if(levels != currentLevel)
        {
            //Checks which level it needs to be
            switch (levels)
            {
                case Levels.MainMenu:
                    {
                        //Loads scene and sets the current level so it is correct.
                        SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
                        currentLevel = levels;
                        player.SetActive(false);
                        hud.SetActive(false);
                        backAudio.clip = mainMenu;
                        backAudio.Play();
                        fightSongActive = false;
                        break;
                    }
                case Levels.Level1:
                    {
                        SceneManager.LoadScene("Level1", LoadSceneMode.Additive);
                        currentLevel = levels;
                        //This will reactivate the player, which was deactivated for the main menu, set the players position, and initilize turn arrays
                        InitilizeLevel(1);
                        backAudio.clip = level1;
                        backAudio.Play();
                        fightSongActive = false;
                        break;
                    }
                case Levels.Level2:
                    {
                        SceneManager.LoadScene("Level2", LoadSceneMode.Additive);
                        currentLevel = levels;
                        InitilizeLevel(2);
                        backAudio.clip = level2;
                        backAudio.Play();
                        fightSongActive = false;
                        break;
                    }
                case Levels.Level3:
                    {
                        SceneManager.LoadScene("Level3", LoadSceneMode.Additive);
                        currentLevel = levels;
                        InitilizeLevel(3);
                        backAudio.clip = level3;
                        backAudio.Play();
                        fightSongActive = false;
                        break;
                    }
            }
        }

        //Switches scene - Testing purposes only
        if (Input.GetKeyDown(KeyCode.L))
        {
            levels = Levels.Level1;
        }
       
        if(pController.state != States.WASD && !fightSongActive)
        {
            fightSongActive = true;
            backAudio.clip = fightSong;
            backAudio.Play();
        }
    }

    //Moves the player to checkpoint location, enables the players renderer, activiates the hud, and resets the turn arrays
    private void InitilizeLevel(int level)
    {
        player.SetActive(true);
        player.transform.position = pController.checkpointLocations[level - 1];
        hud.SetActive(true);
    }
}
