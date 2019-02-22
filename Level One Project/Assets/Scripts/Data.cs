using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Data
{
    public Levels level;
    //public int health;
    //public float[] position;

    public Data(LevelController player)
    {
        level = player.levels;
        //health = player.health;
        //position[0] = player.transform.position.x;
        //position[1] = player.transform.position.y;
        //position[2] = player.transform.position.z;
    }
}
