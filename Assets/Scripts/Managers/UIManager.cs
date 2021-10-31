using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] CanvasGroup mainMenu, instructions, credits;

    public void OpenMainMenu()
    {
        // Show main menu
        mainMenu.alpha = 1;
        mainMenu.interactable = true;
        mainMenu.blocksRaycasts = true;
        // Disable instructions
        instructions.alpha = 0;
        instructions.interactable = false;
        instructions.blocksRaycasts = false;
        // Disable credits;
        credits.alpha = 0;
        credits.interactable = false;
        credits.blocksRaycasts = false;
    }

    public void OpenInstructions()
    {
        // Disable main menu
        mainMenu.alpha = 0;
        mainMenu.interactable = false;
        mainMenu.blocksRaycasts = false;
        // Show instructions
        instructions.alpha = 1;
        instructions.interactable = true;
        instructions.blocksRaycasts = true;
        // Disable credits;
        credits.alpha = 0;
        credits.interactable = false;
        credits.blocksRaycasts = false;
    }

    public void OpenCredits()
    {
        // Disable main menu
        mainMenu.alpha = 0;
        mainMenu.interactable = false;
        mainMenu.blocksRaycasts = false;
        // Disable instructions
        instructions.alpha = 0;
        instructions.interactable = false;
        instructions.blocksRaycasts = false;
        // Disable credits;
        credits.alpha = 1;
        credits.interactable = true;
        credits.blocksRaycasts = true;
    }
}
