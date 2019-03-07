using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDELETELATER : MonoBehaviour
{
    private void Tutorial()
    {
        int instructionNumber = 6;
        //Just replace the entire switch statement with this VVVVV
        switch (instructionNumber)
        {
            case 1:
                {
                    instructions.text = "Welcome to tutorial mode. Click \"I\" for more info. Click \"Q\" to quit tutorial mode.";
                    break;
                }
            case 2:
                {
                    instructions.text = "Use WASD to move in free movment mode. Once you enter the combat zone, you will can use the scroll wheel to switch between more options.";
                    break;
                }
            case 3:
                {
                    instructions.text = "Combat Movement is based on clicks, presented by a line from the player to the mouse. You can only move when the line is green.";
                    break;
                }
            case 4:
                {
                    instructions.text = "You also have 2 modes of attack, melee and range. These are based on clicks as well";
                    break;
                }
            case 5:
                {
                    instructions.text = "You can only use the melee attack once per turn, but it is more powerful.";
                    break;
                }
            case 6:
                {
                    instructions.text = "Your range attack can be used multiply times a turn, but costs some mana. Your mana replenishes a little each turn.";
                    break;
                }
            case 7:
                {
                    instructions.text = "Your final choice in combat mode is to shield. This option costs mana, but lets you have one extra hit point.";
                        break;
                }
            case 8:
                {
                    instructions.text = "To end your turn, click 'Enter'.";
                    break;
                }

        }
    }
}
