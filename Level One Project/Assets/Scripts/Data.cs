using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Data
{
    public Levels level;
    public int health;
    public int playerLevel;
    public float playerXP;
    public int damage;
    public float spellCost;

    public Data(PlayerController player)
    {
        level = player.lController.levels;
        playerLevel = player.playerLevel;
        playerXP = player.playerXP;
        health = player.health;
        damage = player.attack.damage;
        spellCost = player.attack.spellCost;
        //position[0] = player.transform.position.x;
        //position[1] = player.transform.position.y;
        //position[2] = player.transform.position.z;
    }
}
