using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class LevelUp : MonoBehaviour
{
    public HUD hud;

    public GameObject levelUpMenu;
    public AudioSource levelUpSource;
    public AudioClip[] levelUpSounds;
    float spellReductionCost;

    private void Start()
    {
        spellReductionCost = hud.pController.attack.spellCost * 0.1f;
    }

    public void MagicClick()
    {
        //Decrese spell cost by 10% and return to the game
        hud.pController.attack.spellCost -= spellReductionCost;

        //Resume game
        Time.timeScale = 1f;
        levelUpMenu.SetActive(false);
    }

    public void MeleeClick()
    {
        //Increase melee damage by 1
        hud.pController.attack.damage++;

        //Resume game
        Time.timeScale = 1f;
        levelUpMenu.SetActive(false);
    }
}
