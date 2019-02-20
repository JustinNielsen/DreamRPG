using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Creates an enum for each level scene
public enum Levels { MainMenu, Level1, Level2, Level3 };

public class LevelController : MonoBehaviour
{
    private GameObject player;
    private MeshRenderer pRenderer;

    //This enum will be used to track the level it should be
    public Levels levels;
    private Levels currentLevel;
    private TurnBasedSystem turn;
    private PlayerController pController;

    // Start is called before the first frame update
    void Start()
    {
        //Gets a reference to the player and sets the levels enum
        player = GameObject.FindGameObjectWithTag("player");
        levels = Levels.MainMenu;
        //Gets a reference to the playerController
        pController = player.GetComponent<PlayerController>();
        //Gets a reference to the turnBasedSystemScript in the playerController
        turn = pController.turn;
        //Gets a refernece to the meshRenderer on the player
        pRenderer = player.GetComponent<MeshRenderer>();
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
                        pRenderer.enabled = false;
                        break;
                    }
                case Levels.Level1:
                    {
                        player.transform.position = pController.checkpointLocations[0];
                        SceneManager.LoadScene("Level1", LoadSceneMode.Additive);
                        currentLevel = levels;
                        //This will reactivate the player, which was deactivated for the main menu, set the players position, and initilize turn arrays
                        pRenderer.enabled = true;
                        turn.ResetArrays();
                        break;
                    }
                case Levels.Level2:
                    {
                        SceneManager.LoadScene("Level2", LoadSceneMode.Additive);
                        currentLevel = levels;
                        player.transform.position = pController.checkpointLocations[1];
                        turn.ResetArrays();
                        break;
                    }
                case Levels.Level3:
                    {
                        SceneManager.LoadScene("Level3", LoadSceneMode.Additive);
                        currentLevel = levels;
                        player.transform.position = pController.checkpointLocations[2];
                        turn.ResetArrays();
                        break;
                    }
            }
        }

        //Switches scene - Testing purposes only
        if (Input.GetKeyDown(KeyCode.L))
        {
            levels = Levels.Level1;
        }
       
    }
}
