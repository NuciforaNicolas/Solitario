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
    float timer, day;
    int moves, points;
    bool isPlaying;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        stacksCompleted = 0;
        timer = 0;
        day = 0;
        moves = 0;
        points = 0;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Start()
    {
        isPlaying = true;
    }

    private void Update()
    {
        if(isPlaying)
        {
            if (stacksCompleted >= totalStacks)
            {
                Debug.Log("You Won");
            }

            if(timerUI != null && !CardsManager.instance.isSettingGame)
            {
                timer += Time.deltaTime;
                var seconds = Mathf.FloorToInt(timer % 60f);
                var minutes = Mathf.FloorToInt(timer / 60f);
                timerUI.text = minutes.ToString("00") + ":" + seconds.ToString("00");
            }
        }
    }

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
}