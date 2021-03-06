using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] CanvasGroup mainMenu, instructions, credits, victory, options;
    [SerializeField] Button mainmenu, undo, opt;
    [SerializeField] Text points, moves, timer;
    public static UIManager instance;

    private void Awake()
    {
        instance = this;
    }

    public void OpenMainMenu()
    {
        if(mainMenu != null && instructions != null && credits != null)
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
    }

    public void OpenInstructions()
    {
        if (mainMenu != null && instructions != null && credits != null)
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
    }

    public void OpenCredits()
    {
        if (mainMenu != null && instructions != null && credits != null)
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

    public void OpenVictory()
    {
        if(victory != null && moves != null && points != null && timer != null)
        {
            victory.alpha = 1;
            moves.text = GameManager.instance.GetMoves().ToString();
            points.text = GameManager.instance.GetPoints().ToString();
            timer.text = GameManager.instance.GetTimer();
        }
    }

    public void OpenOptions()
    {
        if (options != null)
        {
            options.alpha = 1;
            options.interactable = true;
            options.blocksRaycasts = true;
            mainmenu.interactable = false;
            opt.interactable = false;
            undo.interactable = false;
            Time.timeScale = 0;
        }
    }

    public void CloseOptions()
    {
        if (options != null)
        {
            options.alpha = 0;
            options.interactable = false;
            options.blocksRaycasts = false;
            mainmenu.interactable = true;
            opt.interactable = true;
            undo.interactable = true;
            Time.timeScale = 1;
        }
    }
}
