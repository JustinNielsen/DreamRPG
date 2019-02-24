using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuitButton : MonoBehaviour
{
    private TextMeshPro text;
    public Material normalColor;
    public Material hoverColor;
    public Material clickColor;
    private bool hover;

    // Start is called before the first frame update
    void Start()
    {
        text = GetComponent<TextMeshPro>();
    }

    private void OnMouseEnter()
    {
        text.color = hoverColor.color;
        hover = true;
    }

    private void OnMouseOver()
    {
        //Change camera on click and button color
        if (Input.GetMouseButtonDown(0))
        {
            text.color = clickColor.color;
            StartCoroutine(QuitGame());
        }
    }

    private void OnMouseExit()
    {
        text.color = normalColor.color;
        hover = false;
    }

    IEnumerator QuitGame()
    {
        yield return new WaitForSeconds(0.1f);

        if (hover)
        {
            text.color = hoverColor.color;
        }
        else
        {
            text.color = normalColor.color;
        }

        Application.Quit();
    }
}
