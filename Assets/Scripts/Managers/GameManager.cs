using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    int stacksCompleted;
    [SerializeField] int totalStacks;
    [SerializeField] Text timerUI, movesUI, pointsUI;
    float timer;
    int moves, points;
    bool isPlaying;

    public bool hasWon;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        stacksCompleted = 0;
        timer = 0;
        moves = 0;
        points = 0;
    }

    private void Start()
    {
        isPlaying = true;
        hasWon = false;
        Time.timeScale = 1;
    }

    private void Update()
    {
        if(isPlaying)
        {
            // Se il numero di stack dei semi completi è pari a 4, allora il giocatore ha vinto
            if (stacksCompleted >= totalStacks || hasWon)
            {
                UIManager.instance.OpenVictory();
                Time.timeScale = 0;
            }

            // Incrementa il timer
            if(timerUI != null && !CardsManager.instance.isSettingGame)
            {
                timer += Time.deltaTime;
                var seconds = Mathf.FloorToInt(timer % 60f);
                var minutes = Mathf.FloorToInt(timer / 60f);
                timerUI.text = minutes.ToString("00") + ":" + seconds.ToString("00");
            }
        }
    }

    // Incrementa il numero degli stack completati
    public void IncreaseStacksCompletedCounter()
    {
        stacksCompleted++;
        Debug.Log("Stacks completed: " + stacksCompleted);
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void GoToMainMenu()
    {
        SceneManager.LoadScene(0);
    }

    public void ReloadMatch()
    {
        //Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void IncreaseMoves()
    {
        movesUI.text = (++moves).ToString();
    }

    public void IncreasePoints()
    {
        points += 5;
        pointsUI.text = points.ToString();
    }

    public int GetPoints()
    {
        return points;
    }

    public int GetMoves()
    {
        return moves;
    }

    public string GetTimer()
    {
        return timerUI.text;
    }
}
