using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class HUD : MonoBehaviour
{
    public static bool GameIsPaused = false;
    public GameObject pauseMenuUI;
    public GameObject settingMenu;
    public PlayerMovement pMovement;
    public Image manaBar;
    public LevelController lController;
    public PlayerController pController;
    public GameObject hud;

    //Heart images
    public Sprite heart1;
    public Sprite heart2;
    public Sprite heart3;
    public Sprite shield;
    public GameObject heartPrefab;

    Sprite[] heartSprites;
    //Health list
    List<GameObject> healthList;
    bool healthInitialized = false;

    //Text that shows what state the player is in
    public TextMeshProUGUI stateIndicator;

    //Text that shows messages for the player
    public TextMeshProUGUI errorMessage;

    private void Start()
    {
        //heartSprites = new Sprite[4] { heart1, heart2, heart3, shield };
        //healthList = new List<GameObject>();
        //HUDHealth();
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        GameIsPaused = false;
    }

    public void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        GameIsPaused = true;
    }

    public void QuitGame()
    {
        Resume();
        pController.movement.ToggleNavMesh(false);
        lController.levels = Levels.MainMenu;
    }

    public void DecreaseManaBar(float spellCost)
    {
        float incrementAmount = 0.25f / pController.maxMana;
        pController.remainingMana -= spellCost;

        manaBar.fillAmount = (pController.remainingMana * incrementAmount) + 0.25f;
    }

    private void InitilizeHealth()
    {
        heartSprites = new Sprite[4] { heart3, heart2, heart1, shield };
        healthList = new List<GameObject>();
        healthInitialized = true;
    }

    public void HUDHealth()
    {
        if (!healthInitialized)
        {
            InitilizeHealth();
        }

        GameObject[] hearts = GameObject.FindGameObjectsWithTag("hearts");
        foreach(GameObject obj in hearts)
        {
            healthList.Remove(obj);
            Destroy(obj);
        }

        Vector3 initialPos = new Vector3(100, 14, 0);

        for(int i = 0; i < pController.health; i++)
        {
            Vector3 pos = new Vector3(100 + (i * 25), 14, 0);

            GameObject health = Instantiate(heartPrefab, pos, Quaternion.identity);
            health.transform.SetParent(hud.transform, false);
            health.GetComponent<Image>().sprite = heartSprites[i];

            //Adds the object to healthList
            healthList.Add(health);
        }

        if (pController.shieldActive)
        {
            Vector3 pos = new Vector3(100 + (pController.health * 25), 14, 0);

            GameObject health = Instantiate(heartPrefab, pos, Quaternion.identity);
            health.transform.SetParent(hud.transform, false);
            health.GetComponent<Image>().sprite = heartSprites[3];
        }
    }

    public IEnumerator DisplayError(string error)
    {
        errorMessage.text = error;
        errorMessage.CrossFadeAlpha(1, 0.5f, false);
        yield return new WaitForSeconds(1f);
        errorMessage.CrossFadeAlpha(0, 0.5f, false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (settingMenu.activeSelf == false)
            {
                if (GameIsPaused)
                {
                    Resume();
                }
                else
                {
                    Pause();
                }
            }
            else
            {
                settingMenu.SetActive(false);
                Resume();
            }
        }

        /*if (pMovement.maxDistance != remainingMana)
        {
            manaBar.fillAmount = (pMovement.maxDistance * 0.025f) + 0.25f;
            remainingMana = pMovement.maxDistance;
        }*/
    }
}
