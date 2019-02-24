﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.UI;

//Creates an enum for each level scene
public enum Levels { MainMenu, Level1, Level2, Level3 };

public class LevelController : MonoBehaviour
{
    public GameObject player;
    public GameObject hud;
    public Camera cam;
    private CinemachineBrain camBrain;

    //This enum will be used to track the level it should be
    public Levels levels;
    private Levels currentLevel;
    int sceneIndex;

    private TurnBasedSystem turn;
    private PlayerController pController;

    //Audio things
    AudioSource backAudio;
    public AudioClip mainMenu;
    public AudioClip level1;
    public AudioClip level2;
    public AudioClip level3;
    public AudioClip fightSong;

    public bool fightSongActive = false;

    public GameObject fadePanel;
    Image fade;
    bool fading = true;

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
        camBrain = cam.GetComponent<CinemachineBrain>();
        //Gets the image component from fade panel
        fade = fadePanel.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        //This will make sure that the scene is loaded only once.
        if(levels != currentLevel)
        {
            StartCoroutine(SwitchLevels());
            currentLevel = levels;
            camBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
            /*
            fade.CrossFadeAlpha(1, 1, true);
            //Checks which level it needs to be
            switch (levels)
            {
                case Levels.MainMenu:
                    {
                        camBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
                        //Loads scene and sets the current level so it is correct.
                        SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
                        //Unloads previous scene
                        SceneManager.UnloadSceneAsync(sceneIndex);
                        StartCoroutine(FadeOut());
                        sceneIndex = 1;
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
                        camBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
                        //Unloads previous scene
                        SceneManager.LoadScene("Level1", LoadSceneMode.Additive);
                        SceneManager.UnloadSceneAsync(sceneIndex);
                        StartCoroutine(FadeOut());
                        sceneIndex = 2;
                        currentLevel = levels;
                        //This will reactivate the player, which was deactivated for the main menu, set the players position, and initilize turn arrays
                        InitilizeLevel(1);
                        backAudio.clip = level1;
                        backAudio.Play();
                        fightSongActive = false;
                        SavePlayer();
                        break;
                    }
                case Levels.Level2:
                    {
                        camBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
                        //Unloads previous scene
                        SceneManager.LoadScene("Level2", LoadSceneMode.Additive);
                        SceneManager.UnloadSceneAsync(sceneIndex);
                        sceneIndex = 3;
                        currentLevel = levels;
                        InitilizeLevel(2);
                        backAudio.clip = level2;
                        backAudio.Play();
                        fightSongActive = false;
                        SavePlayer();
                        break;
                    }
                case Levels.Level3:
                    {
                        camBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
                        //Unloads previous scene
                        SceneManager.LoadScene("Level3", LoadSceneMode.Additive);
                        SceneManager.UnloadSceneAsync(sceneIndex);
                        sceneIndex = 4;
                        currentLevel = levels;
                        InitilizeLevel(3);
                        backAudio.clip = level3;
                        backAudio.Play();
                        fightSongActive = false;
                        SavePlayer();
                        break;
                    }
            }*/
        }

        //Switches scene - Testing purposes only
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            levels = Levels.Level1;
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            levels = Levels.Level2;
        }

        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            levels = Levels.Level3;
        }

        if (pController.state != States.WASD && !fightSongActive)
        {
            fightSongActive = true;
            backAudio.clip = fightSong;
            backAudio.Play();
        }
        /*
        if (fading)
        {
            fade.CrossFadeAlpha(1, 0.25f, false);
        }
        else
        {
            fade.CrossFadeAlpha(0, 0.25f, false);
        }*/
    }

    //Moves the player to checkpoint location, enables the players renderer, activiates the hud, and resets the turn arrays
    private void InitilizeLevel(int level)
    {
        player.SetActive(true);
        player.transform.position = pController.checkpointLocations[level - 1];
        hud.SetActive(true);
    }

    public void SavePlayer()
    {
        Checkpoint.SavePlayer(this);
    }

    public Levels LoadPlayer()
    {
        //Get Player data
        Data data = Checkpoint.LoadPlayer();
        //if there is a save file load the level
        if (data != null)
        {
            //levels = data.level;
            return data.level;
        }
        else
        {
            return Levels.MainMenu;
        }

    }

    IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(0.25f);
        fade.CrossFadeAlpha(1, 1, true);
    }

    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(0.25f);
        fade.CrossFadeAlpha(0, 1, true);
    }

    IEnumerator SwitchLevels()
    {
        fade.CrossFadeAlpha(1, 1f, true);
        yield return new WaitForSeconds(1.1f);

        switch (levels)
        {
            case Levels.MainMenu:
                {
                    camBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
                    //Loads scene and sets the current level so it is correct.
                    SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
                    //Unloads previous scene
                    SceneManager.UnloadSceneAsync(sceneIndex);
                    StartCoroutine(FadeOut());
                    sceneIndex = 1;
                    //currentLevel = levels;
                    player.SetActive(false);
                    hud.SetActive(false);
                    backAudio.clip = mainMenu;
                    backAudio.Play();
                    fightSongActive = false;
                    break;
                }
            case Levels.Level1:
                {
                    camBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
                    //Unloads previous scene
                    SceneManager.LoadScene("Level1", LoadSceneMode.Additive);
                    SceneManager.UnloadSceneAsync(sceneIndex);
                    sceneIndex = 2;
                    //currentLevel = levels;
                    //This will reactivate the player, which was deactivated for the main menu, set the players position, and initilize turn arrays
                    InitilizeLevel(1);
                    backAudio.clip = level1;
                    backAudio.Play();
                    fightSongActive = false;
                    SavePlayer();
                    break;
                }
            case Levels.Level2:
                {
                    camBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
                    //Unloads previous scene
                    SceneManager.LoadScene("Level2", LoadSceneMode.Additive);
                    SceneManager.UnloadSceneAsync(sceneIndex);
                    sceneIndex = 3;
                    //currentLevel = levels;
                    InitilizeLevel(2);
                    backAudio.clip = level2;
                    backAudio.Play();
                    fightSongActive = false;
                    SavePlayer();
                    break;
                }
            case Levels.Level3:
                {
                    camBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
                    //Unloads previous scene
                    SceneManager.LoadScene("Level3", LoadSceneMode.Additive);
                    SceneManager.UnloadSceneAsync(sceneIndex);
                    sceneIndex = 4;
                    //currentLevel = levels;
                    InitilizeLevel(3);
                    backAudio.clip = level3;
                    backAudio.Play();
                    fightSongActive = false;
                    SavePlayer();
                    break;
                }
        }

        yield return new WaitForSeconds(1.5f);
        fade.CrossFadeAlpha(0, 1f, true);
    }
}
