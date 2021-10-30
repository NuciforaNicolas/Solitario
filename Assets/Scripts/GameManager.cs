using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    int stacksCompleted;
    [SerializeField] int totalStacks;


    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        stacksCompleted = 0;
        DontDestroyOnLoad(this.gameObject);
    }

    private void Update()
    {
        if(stacksCompleted >= totalStacks)
        {
            Debug.Log("You Won");
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
}
