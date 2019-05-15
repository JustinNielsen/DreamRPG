using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Tutorial : MonoBehaviour
{
    public GameObject autoTutorialHUD;
    public TextMeshProUGUI tutorialText;

    PlayerController pController;

    public bool autoTutorialActive = false;
    string[] tutorialDescription;
    int tutorialPortion;

    //Triggers for when the player does certain things
    public bool rangeShot = false;

    // Start is called before the first frame update
    void Start()
    {
        //Get a reference to the player controller on the player
        pController = this.GetComponent<PlayerController>();

        //initilizing an array of all the tutorial description portions
        tutorialDescription = new string[10] 
        { "While in WASD movement mode you can move the character using the keys W,A,S, and D. ",
          "Each level has a predetermined combat zone that will activte once you enter. Once in the combat zone your state will automatically change to combat movement. Please use W,A,S, and D to navigate to the combat zone.",
          "Once you enter the combat zone, you can move by moving the mouse and clicking. The line shows your path, and is green when you can move to that spot. Please move towards an enemy.",
          "You can switch your action by using the scroll wheel or arrow keys. Scroll or arrow up one please.",
          "A melee attack has a limited amount of range. In order to hit the enemy, they must be in the red box. Hit the enemy with a melee attack. You can only do this once a turn.",
          "Scroll up again to reach the ranged attack. This move has constant damage, and costs mana to use. That means you can use it more than once a turn. If you run out of mana, you can't use it anymore. Mana regenerates each turn.",
          "Scroll up again to reach the shield. This uses mana to give you one extra hit point. However, you can only have one shield active at a time.",
          "If you feel like you have done all you can for your turn, hit enter to end you turn.",
          "Once all of the enemies are defeated, you will switch back to W,A,S, and D movement.",
          "To exit the level, you must walk to the doors at the end. You can also use Esc to open the pause menu."};

        //Sets the starting point of the tutorial
        UpdateTutorial(0);
    }

    //Update is called once per frame
    void Update()
    {
        if (autoTutorialActive)
        {
            switch (tutorialPortion)
            {
                case 0:
                    //Advance the tutorial if the player pushes either W, A, S, or D
                    if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
                    {
                        UpdateTutorial(1);
                    }
                    break;
                case 1:
                    //Advance the tutorial once the player enters the combat zone.
                    if (pController.state == States.NavMesh)
                    {
                        UpdateTutorial(2);
                    }
                    break;
                case 2:
                    //Advance the tutorial once the player clicks on a valid location and starts moving
                    if (pController.movement.isMoving)
                    {
                        UpdateTutorial(3);
                    }
                    break;
                case 3:
                    //Advance the tutorial once the player switches to the melee attack state
                    if(pController.state == States.MeleeAttack)
                    {
                        UpdateTutorial(4);
                    }
                    break;
                case 4:
                    //Adnvace the tutorial once the player uses the melee attack
                    if (pController.canAttack == false)
                    {
                        UpdateTutorial(5);
                    }
                    break;
                case 5:
                    //Advance the tutorial once the player switches to range attack mode and shoots a bullet
                    if(rangeShot == true)
                    {
                        UpdateTutorial(6);
                    }
                    break;
                case 6:
                    //Advance the tutorial once the player activates the shield
                    if (pController.shieldActive == true)
                    {
                        UpdateTutorial(7);
                    }
                    break;
                case 7:
                    //Advance the tutorial once the player presses enter to end their turn
                    if (Input.GetKeyDown(KeyCode.Return))
                    {
                        UpdateTutorial(8);
                    }
                    break;
                case 8:
                    //Advance the tutorial once the player kills all the enemies
                    if(pController.state == States.WASD)
                    {
                        UpdateTutorial(9);
                    }
                    break;
                case 9:
                    //Advance the tutorial once the player starts moving
                    if(Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.D))
                    {
                        //At this point we can either keep the text on the screen, add more text, or take it away                        
                    }
                    break;
            }

            //Disable the tutorial text if the player is not on level 1
            if(pController.lController.levels != Levels.Level1)
            {
                autoTutorialHUD.SetActive(false);
                autoTutorialActive = false;
            }
        }
    }

    void UpdateTutorial(int i)
    {
        tutorialPortion = i;

        //change the tutorial text to the prewritten tutorial thingymajigs according to the number inputed
        tutorialText.text = tutorialDescription[i];
    }
}
