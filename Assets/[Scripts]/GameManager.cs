using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SceneManagement;

public enum GameState
{
    MainMenu,
    HowToPlay,
    Credits,
    Loss,
    Game,

    NoState
}

// Alot of buttons in this game are used elsewhere and do the same tasks
public enum Btn
{
    Start    = 0,
    HowToPlay= 1,
    Credits  = 2,
    Quit     = 3,
    MainMenu = 4,


}


public class GameManager : MonoBehaviour
{
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text livesText;
    private int score = 0;
    private int lives = 3;

    public static GameManager Instance;

    [SerializeField] private Canvas[] gameCanvases;

    public delegate void OnFruitCollectedDelegate();
    public OnFruitCollectedDelegate onFruitCollected;

    private GameState gameState;

    // Start is called before the first frame update
    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        Time.timeScale = 0f;
        ChangeGameState(GameState.MainMenu);
    }
    public void ChangeGameState(GameState newState)
    {
        TurnOffAllCanvases();
        gameState = newState;
        Canvas currentCanvas = newState switch
        {
            GameState.MainMenu => gameCanvases[(int)GameState.MainMenu],
            GameState.HowToPlay => gameCanvases[(int)GameState.HowToPlay],
            GameState.Credits => gameCanvases[(int)GameState.Credits],
            GameState.Loss => gameCanvases[(int)GameState.Loss],
            GameState.Game => gameCanvases[(int)GameState.Game],


            _ => throw new Exception("Unknown game state!"),
        };
        currentCanvas.gameObject.SetActive(true);

    }
    public void OnButtonPressed(int btnIndex)
    {
        Btn btn = (Btn)btnIndex;

        GameState changeTo = btn switch
        {
            Btn.Start      => GameState.Game,
            Btn.HowToPlay  => GameState.HowToPlay,
            Btn.Credits    => GameState.Credits,
            Btn.MainMenu   => GameState.MainMenu,
            Btn.Quit       => GameState.NoState,
            _              => throw new Exception("GameManager.cs: Unknown game state!"),
        };
        
        if (changeTo != GameState.NoState)
            ChangeGameState(changeTo);

        if (btn == Btn.Start)
        {
            FindObjectOfType<SnakeController>().ResetSnake();
            AudioManager.Instance.PlayAudio(Audio.MainTheme, true);
            Time.timeScale = 1f;
        }
        else if (btn == Btn.Quit)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

    }

    public void TurnOffAllCanvases()
    {
        foreach(Canvas c in gameCanvases)
        {
            c.gameObject.SetActive(false);
        }
    }

    public void OnFruitCollected(int incrementBy = 10)
    {
        StartCoroutine(SmoothStepScore(incrementBy, 1f));
    }
    private IEnumerator SmoothStepScore(int incrementBy, float smoothStepTime)
    {

        float timer = 0f;

        int originalScore = score;
        int targetScore = score + incrementBy;

        while (timer <= smoothStepTime)
        {
            timer += Time.deltaTime;

            int tempScore = (int)Mathf.SmoothStep(0f, (float)incrementBy, timer / smoothStepTime);

            score = tempScore + originalScore;
            scoreText.text = "Score: " + score.ToString();

            yield return null;
        }
        score = targetScore;
        scoreText.text = "Score: " + score.ToString();

        onFruitCollected.Invoke();
    }
    public void OnGameOver()
    {
        // Stop all music themes.
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetScore()
    {
        score = 0;
        scoreText.text = "Score: " + score.ToString();

    }
    public void ResetLives()
    {
        lives = 3;
        livesText.text = "Lives: " + lives.ToString();
    }
    public void LoseLife()
    {
        lives--;
        livesText.text = "Lives: " + lives.ToString();
    }
}
