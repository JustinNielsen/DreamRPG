using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;
using UnityEngine.UI;

//Creates an enum for each level scene
public enum Levels { MainMenu = 99999, Level1 = 0, Level2 = 3, Level3 = 6, Space = 9 };

public class LevelController : MonoBehaviour
{
    public GameObject player;
    //MeshRenderer pRenderer;
    Rigidbody playerRB;

    public GameObject hud;
    public HUD hudScript;
    public Camera cam;
    private CinemachineBrain camBrain;

    //This enum will be used to track the level it should be
    public Levels levels;
    private Levels currentLevel;
    int sceneIndex;

    private TurnBasedSystem turn;
    public PlayerController pController;

    //Audio things
    AudioSource backAudio;
    public AudioClip mainMenu;
    public AudioClip level1;
    public AudioClip level2;
    public AudioClip level3;
    public AudioClip fightSong;

    public AudioClip[] songs;

    public bool fightSongActive = false;

    public GameObject fadePanel;
    Image fade;
    bool fading = true;

    //Check if the player is trying to restart a level
    public bool gameOverLoadPlayer = false;

    public CinemachineTransposer transposer;

    // Start is called before the first frame update
    void Start()
    {
        //sets the levels enum
        levels = Levels.MainMenu;
        //Gets a reference to the playerController
        pController = player.GetComponent<PlayerController>();
        //Gets the players mesh renderer and rigidbody
        //pRenderer = player.GetComponent<MeshRenderer>();
        playerRB = player.GetComponent<Rigidbody>();
        //Gets a reference to the turnBasedSystemScript in the playerController
        turn = GameObject.FindGameObjectWithTag("turn").GetComponent<TurnBasedSystem>();
        currentLevel = Levels.Level1;
        backAudio = cam.GetComponent<AudioSource>();
        camBrain = cam.GetComponent<CinemachineBrain>();
        //Gets the image component from fade panel
        fade = fadePanel.GetComponent<Image>();
        //Initilize songs array
        songs = new AudioClip[5] { mainMenu, level1, level2, level3, fightSong };
        //Initilize transposer
        transposer = pController.NormalCamera.GetCinemachineComponent<CinemachineTransposer>();
    }

    // Update is called once per frame
    void Update()
    {
        //This will make sure that the scene is loaded only once.
        if(levels != currentLevel || gameOverLoadPlayer)
        {
            gameOverLoadPlayer = false;
            pController.gameOver.SetActive(false);
            StartCoroutine(SwitchLevels());
            currentLevel = levels;
            camBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
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

        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            levels = Levels.Space;
        }

        //Activates the fight song when in a combat zone and the fight song is not already active
        if (pController.state != States.WASD && !fightSongActive)
        {
            fightSongActive = true;
            backAudio.clip = songs[4];
            backAudio.Play();
        }

        //Turns off the fight song if fightSongActive is false and the song that is playing is the fightsong
        if (!fightSongActive && backAudio.clip == songs[4])
        {
            fightSongActive = false;
            backAudio.clip = songs[sceneIndex - 1];
            backAudio.Play();
        }
    }

    //Moves the player to checkpoint location, enables the players renderer, activiates the hud, and resets the turn arrays
    private void InitilizeLevel(int level)
    {
        //player.SetActive(true);
        //pRenderer.enabled = true;
        playerRB.isKinematic = false;
        player.transform.position = pController.checkpointLocations[level - 1];
        //Initilize health on the hud
        pController.hud.HUDHealth();
        hud.SetActive(true);
        hud.GetComponent<HUD>().errorMessage.CrossFadeAlpha(0, 0.01f, false);
        pController.state = States.WASD;
        pController.hud.DisplayStats();
        pController.remainingMana = pController.maxMana;
        pController.hud.DecreaseManaBar(0);
        //pController.turn.ResetArrays();
    }

    //Slowly fades in a white screen
    IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(0.25f);
        fade.CrossFadeAlpha(1, 1, true);
    }

    //Slowly fades out a white screen
    IEnumerator FadeOut()
    {
        yield return new WaitForSeconds(0.25f);
        fade.CrossFadeAlpha(0, 1, true);
    }

    //Switches the levles according to the levels variable
    IEnumerator SwitchLevels()
    {
        fade.CrossFadeAlpha(1, 1f, true);
        yield return new WaitForSeconds(1.1f);

        //Checks to see which level is trying to be activated
        switch (levels)
        {
            case Levels.MainMenu:
                {
                    camBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
                    //Unloads previous scene
                    SceneManager.UnloadSceneAsync(sceneIndex);
                    //Loads scene and sets the current level so it is correct.
                    SceneManager.LoadScene("MainMenu", LoadSceneMode.Additive);
                    StartCoroutine(FadeOut());
                    sceneIndex = 1;
                    //currentLevel = levels;
                    //player.SetActive(false);
                    //pRenderer.enabled = false;
                    playerRB.isKinematic = true;
                    hud.SetActive(false);
                    pController.gameOver.SetActive(false);
                    backAudio.clip = songs[0];
                    backAudio.Play();
                    fightSongActive = false;
                    //Set the scale of the player
                    player.transform.localScale = new Vector3(0, 0, 0);
                    break;
                }
            case Levels.Level1:
                {
                    camBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
                    //Unloads previous scene
                    SceneManager.UnloadSceneAsync(sceneIndex);
                    SceneManager.LoadScene("Level1", LoadSceneMode.Additive);
                    sceneIndex = 2;
                    //currentLevel = levels;
                    //This will reactivate the player, which was deactivated for the main menu, set the players position, and initilize turn arrays
                    InitilizeLevel(1);
                    backAudio.clip = songs[1];
                    backAudio.Play();
                    fightSongActive = false;
                    pController.SavePlayer();
                    player.transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
                    transposer.m_FollowOffset = new Vector3(-6f, 12, -3f);
                    pController.NormalCamera.transform.rotation = Quaternion.Euler(new Vector3(60, 245, 0));
                    pController.movement.movementSpeed = 4 * pController.gameObject.transform.localScale.y;
                    pController.movement.agent.speed = 1.4f * pController.gameObject.transform.localScale.y;
                    pController.movement.line.startWidth = 0.1f;
                    pController.movement.line.endWidth = 0.1f;
                    pController.movement.maxDistance = 4f * pController.gameObject.transform.localScale.y;
                    break;
                }
            case Levels.Level2:
                {
                    camBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
                    //Unloads previous scene
                    SceneManager.UnloadSceneAsync(sceneIndex);
                    SceneManager.LoadScene("Level2", LoadSceneMode.Additive);
                    sceneIndex = 3;
                    //currentLevel = levels;
                    InitilizeLevel(2);
                    backAudio.clip = songs[2];
                    backAudio.Play();
                    fightSongActive = false;
                    pController.SavePlayer();
                    player.transform.localScale = new Vector3(1.5f, 1.5f, 1.5f);
                    transposer.m_FollowOffset = new Vector3(-4f, 8f, -2f);
                    pController.NormalCamera.transform.rotation = Quaternion.Euler(new Vector3(60, 235, 0));
                    pController.movement.movementSpeed = 4 * pController.gameObject.transform.localScale.y;
                    pController.movement.agent.speed = 1.4f * pController.gameObject.transform.localScale.y;
                    pController.movement.line.startWidth = 0.1f;
                    pController.movement.line.endWidth = 0.1f;
                    pController.movement.maxDistance = 4f * pController.gameObject.transform.localScale.y;
                    break;
                }
            case Levels.Level3:
                {
                    camBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
                    //Unloads previous scene
                    SceneManager.UnloadSceneAsync(sceneIndex);
                    SceneManager.LoadScene("Level3", LoadSceneMode.Additive);
                    sceneIndex = 4;
                    //currentLevel = levels;
                    InitilizeLevel(3);
                    backAudio.clip = songs[3];
                    backAudio.Play();
                    fightSongActive = false;
                    pController.SavePlayer();
                    player.transform.localScale = new Vector3(3f, 3f, 3f);
                    transposer.m_FollowOffset = new Vector3(-6f, 12, -3f);
                    pController.NormalCamera.transform.rotation = Quaternion.Euler(new Vector3(60, 245, 0));
                    pController.movement.movementSpeed = 4 * pController.gameObject.transform.localScale.y;
                    pController.movement.agent.speed = 1.4f * pController.gameObject.transform.localScale.y;
                    pController.movement.line.startWidth = 0.1f;
                    pController.movement.line.endWidth = 0.1f;
                    pController.movement.maxDistance = 4f * pController.gameObject.transform.localScale.y;
                    break;
                }
            case Levels.Space:
                {
                    camBrain.m_DefaultBlend.m_Style = CinemachineBlendDefinition.Style.Cut;
                    //Unloads previous scene
                    SceneManager.UnloadSceneAsync(sceneIndex);
                    SceneManager.LoadScene("Space", LoadSceneMode.Additive);
                    sceneIndex = 5;
                    //currentLevel = levels;
                    InitilizeLevel(4);
                    backAudio.clip = songs[3];
                    backAudio.Play();
                    fightSongActive = false;
                    pController.SavePlayer();
                    player.transform.localScale = new Vector3(.25f, .25f, .25f);
                    transposer.m_FollowOffset = new Vector3(-0.3f, 1, -0.15f);
                    pController.NormalCamera.transform.rotation = Quaternion.Euler(new Vector3(60, 245, 0));
                    pController.movement.movementSpeed = 4 * pController.gameObject.transform.localScale.y;
                    pController.movement.agent.speed = 1.4f * pController.gameObject.transform.localScale.y;
                    pController.movement.line.startWidth = 0.01f;
                    pController.movement.line.endWidth = 0.01f;
                    pController.movement.maxDistance = 4f * pController.gameObject.transform.localScale.y;
                    break;
                }
        }

        yield return new WaitForSeconds(1.5f);
        fade.CrossFadeAlpha(0, 1f, true);
        Debug.Log((int)levels);
        pController.MattVoiceOver((int)levels);
    }
}
