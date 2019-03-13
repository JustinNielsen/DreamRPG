using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TurnBasedSystem : MonoBehaviour
{

    public GameObject[] turnArr;
    public int turn = 0;
    public List<Character> charList;
    public PlayerController pController;

    //Get textures for the turn order
    public Texture[] turnTextures;
    public Texture player;
    public Texture enemy1;
    public Texture enemy2;
    public Texture enemy3;
    public Texture enemy4;

    public GameObject turnOrderPrefab;
    public GameObject hud;
    int oldCount;
    public List<GameObject> turnOrder;
    public RectTransform turnArrow;

    // Start is called before the first frame update
    private void Start()
    {
        turnTextures = new Texture[5] { player, enemy1, enemy2, enemy4, enemy3 };
        //turnOrder.Add(GameObject.FindGameObjectWithTag("playerTurn"));
    }

    public void ResetArrays()
    {
        StartCoroutine(Reset(0.1f));

        //InitializeTurnArray();
        //Create an empty list for charList
        //charList = new List<Character>();
        //InitializeCharacterList();
    }

    private void InitializeCharacterList()
    {
        //Initilizes charList with objects from the turnArr
        foreach(GameObject obj in turnArr)
        {
            charList.Add(new Character(obj));
        }
    }

    private void InitializeTurnArray()
    {
        //Creates an array of enemy objects
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("enemy");
        //Creates the turnArr
        turnArr = new GameObject[enemies.Length + 1];
        //Sets the first element in the turnArr to the player
        turnArr[0] = GameObject.FindGameObjectWithTag("player");
        //Adds the enemies to the turnArr
        for(int i = 1; i < turnArr.Length; i++)
        {
            turnArr[i] = enemies[i - 1];
        }

        //If all the enemies are gone change state to WASD
        if(turnArr.Length == 1)
        {
            pController.ResetPlayerCombat();
            pController.lController.fightSongActive = false;
            pController.movement.ToggleNavMesh(false);
            pController.remainingMana = pController.maxMana;
            pController.maxDistance = 10f * this.gameObject.transform.localScale.y;
            pController.health = 3;
            //Updates the change in health
            pController.hud.HUDHealth();
            //Updates the change in mana
            pController.hud.DecreaseManaBar(0);
            //Switch animation back to walking
            pController.movement.anim.SetTrigger("ExitCombat");

            //This goes through each level option
            switch (pController.lController.levels)
            {
                case Levels.Level1:
                    pController.MattVoiceOver(2);
                    break;
                case Levels.Level2:
                    pController.MattVoiceOver(5);
                    break;
                case Levels.Level3:
                    pController.MattVoiceOver(8);
                    //Open the exit door
                    pController.exitDoor.SetActive(false);
                    break;
            }
        }
    }
    
    //Switches the turn to a differnt character
    public void SwitchTurn()
    {
        //If turn is on turnArr's final value then set turn to 0
        if(turn == turnArr.Length - 1)
        {
            turn = 0;
        }
        else //Otherwise advance turn by 1
        {
            turn++;
        }

        if(pController.state == States.WASD)
        {
            turn = 0;
        }

        //Change turn arrow indicator location
        turnArrow.anchoredPosition = new Vector3(-40.1f, (-20f + (-32f * turn)), 0f);

        //If turn = 0 activate the player object and deactivate the enemy objects
        if (turn == 0)
        {
            foreach(Character classObj in charList)
            {
                if (classObj.Obj.CompareTag("player"))
                {
                    classObj.PController.TogglePlayer(true);
                }
                else
                {
                    classObj.EController.ToggleEnemy(false);
                }
            }
        }
        else //Otherwise if turn != 0 deactivate everything and activate the object whose turn it is
        {
            //Deactivate everything
            foreach (Character classObj in charList)
            {
                if (classObj.Obj.CompareTag("player"))
                {
                    classObj.PController.TogglePlayer(false);
                }
                else
                {
                    classObj.EController.ToggleEnemy(false);
                }
            }

            //Activate the object based on the turn using the charList
            charList[turn].EController.ToggleEnemy(true);
        }
    }

    private void InitilizeTurnOrderHud()
    {
        //Destroy previous enemy turn indicators
        GameObject[] enemyTurns = GameObject.FindGameObjectsWithTag("enemyTurn");
        foreach (GameObject obj in enemyTurns)
        {
            turnOrder.Remove(obj);
            Destroy(obj);
        }

        if(pController.state == States.WASD)
        {
            return;
        }

        Vector3 initialPos = new Vector3(-20, -20, 0);
        int enemyType;

        for (int i = 1; i < charList.Count; i++)
        {
            enemyType = charList[i].EController.enemyType;
            Vector3 enemyPos = new Vector3(initialPos.x, initialPos.y - (32 * i), initialPos.z);

            GameObject turnIndicator = Instantiate(turnOrderPrefab, enemyPos, Quaternion.identity);
            turnIndicator.transform.SetParent(hud.transform, false);
            turnIndicator.GetComponent<RawImage>().texture = turnTextures[enemyType];

            //Add object to turnOrder list
            turnOrder.Add(turnIndicator);
        }
    }

    IEnumerator Reset(float delay)
    {
        yield return new WaitForSeconds(delay);

        InitializeTurnArray();
        //Create an empty list for charList
        charList = new List<Character>();
        InitializeCharacterList();
        InitilizeTurnOrderHud();
    }
}
