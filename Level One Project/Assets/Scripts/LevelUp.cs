using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LevelUp : MonoBehaviour
{
    public HUD hud;

    public GameObject levelUpMenu;

    //Audio for leveling up
    public AudioSource levelUpSource;
    public AudioClip[] levelUpSounds;

    float spellReductionCost;

    private void Start()
    {
        spellReductionCost = hud.pController.attack.spellCost * 0.1f;
    }

    public void PlayLevelSound(int i)
    {
        levelUpSource.clip = levelUpSounds[i];
        levelUpSource.Play();
    }

    public void MagicClick()
    {
        //Decrese spell cost by 10% and return to the game
        hud.pController.attack.spellCost -= (int)spellReductionCost;

        //Save the player
        hud.pController.SavePlayer();

        //Resume game
        Time.timeScale = 1f;

        if(hud.pController.state == States.RangeAttack || hud.pController.state == States.MeleeAttack)
        {
            hud.pController.canAttack = true;
        }

        levelUpMenu.SetActive(false);
    }

    public void MeleeClick()
    {
        //Increase melee damage by 1
        hud.pController.attack.damage += 1;

        //Save the player
        hud.pController.SavePlayer();

        //Resume game
        Time.timeScale = 1f;

        if (hud.pController.state == States.RangeAttack || hud.pController.state == States.MeleeAttack)
        {
            hud.pController.canAttack = true;
        }

        levelUpMenu.SetActive(false);
    }
}
