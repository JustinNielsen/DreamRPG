using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

//Creates an enum for each level scene
public enum Levels { MainMenu, Level1, Level2, Level3}
public class LevelController : MonoBehaviour
{
    private GameObject player;
    //This enum will be used to track the level it should be
    public Levels levels;
    private Levels currentLevel;
    // Start is called before the first frame update
    void Start()
    {
        //Gets a reference to the player and sets the levels enum
        player = GameObject.FindGameObjectWithTag("player");
        levels = Levels.MainMenu;
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
                        break;
                    }
                case Levels.Level1:
                    {
                        SceneManager.LoadScene("Level1", LoadSceneMode.Additive);
                        currentLevel = levels;
                        //This will reactivate the player, which was deactivated for the main menu
                        player.SetActive(true);
                        break;
                    }
                case Levels.Level2:
                    {
                        SceneManager.LoadScene("Level2", LoadSceneMode.Additive);
                        currentLevel = levels;
                        break;
                    }
                case Levels.Level3:
                    {
                        SceneManager.LoadScene("Level3", LoadSceneMode.Additive);
                        currentLevel = levels;
                        break;
                    }
            }
        }
       
    }
}
