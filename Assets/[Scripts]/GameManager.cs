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
    [SerializeField] private TMP_Text gameOverScoreText;
    [SerializeField] private TMP_Text gameTimerText;
    [SerializeField] private TMP_Text timeUntilShrinkText;
    [SerializeField] private GameObject arena;
    [SerializeField] private float shrinkRate = 10f;
    [SerializeField] private float shrinkBy = 0.2f;
    [SerializeField] private float shrinkSpeed = 3f;
    
    private int score = 0;
    [NonSerialized] public float arenaScaleXZ = 2f;
    [NonSerialized] public float gameTimer = 0f;
    [NonSerialized] public float timeUntilShrink = 10f;

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

        timeUntilShrink = shrinkRate;

        InvokeRepeating(nameof(OnShrinkArena), shrinkRate, shrinkRate);
    }
    

    public void OnShrinkArena()
    {
        timeUntilShrink = shrinkRate;
        if (arenaScaleXZ > 0.5f)
        {
            StartCoroutine(ShrinkArena());
        }
        else
        {
            CancelInvoke(nameof(OnShrinkArena));
        }
    }

    public IEnumerator ShrinkArena()
    {
        float startArenaScaleXZ = arenaScaleXZ;
        float targetArenaScaleXZ = arenaScaleXZ - shrinkBy;
        float timer = 0f;
        while (timer < shrinkSpeed)
        {
            timer += Time.deltaTime;
            arenaScaleXZ = Mathf.Lerp(startArenaScaleXZ, targetArenaScaleXZ, timer / shrinkSpeed);
            arena.transform.localScale = new Vector3(arenaScaleXZ, 1f, arenaScaleXZ);
            
            yield return null;
        }
        FindObjectOfType<Fruit>().RandomLocation();
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
        AudioManager.Instance.PlayAudio(Audio.BtnClick, false);
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
            arenaScaleXZ = 2f;
            arena.transform.localScale = new Vector3(2f, 1f, 2f);
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
    public void OnGameOver()
    {
        StopAllCoroutines();

        Time.timeScale = 0f;
        ChangeGameState(GameState.Loss);
        gameOverScoreText.text = "Score: " + score.ToString();
    }
    public void OnFruitCollected(int incrementBy = 10)
    {
        StartCoroutine(SmoothStepScore(incrementBy, 0.25f));
    }
    private IEnumerator SmoothStepScore(int incrementBy, float smoothStepTime)
    {

        float timer = 0f;

        int originalScore = score;
        int targetScore = score + incrementBy;

        while (timer < smoothStepTime)
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

    public void ResetScore()
    {
        score = 0;
        scoreText.text = "Score: " + score.ToString();

    }
    // Update is called once per frame
    void Update()
    {
        timeUntilShrink -= Time.deltaTime;
        timeUntilShrinkText.text = "Time until arena shrinks:\n0:" + ((int)timeUntilShrink).ToString("00");

        int realSeconds = (int)gameTimer % 60;
        int realMinutes = Mathf.FloorToInt((int)gameTimer / 60);

        gameTimerText.text = realMinutes + ":" + realSeconds.ToString("00");
    }
}
