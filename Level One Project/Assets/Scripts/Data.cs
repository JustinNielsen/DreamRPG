using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Data
{
    public Levels level;
    public int health;
    public int playerLevel;
    public int playerXP;

    public Data(ControlPlayer player)
    {
        level = player.lController.levels;
        playerLevel = player.playerLevel;
        playerXP = player.playerXP;
        health = player.health;
        //position[0] = player.transform.position.x;
        //position[1] = player.transform.position.y;
        //position[2] = player.transform.position.z;
    }
}
