using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScheneManager : MonoBehaviour
{
    [Serializable]
    private enum Difficulty
    {
        Easy,
        Medium,
        Hard
    }

    public static ScheneManager Instance { get; private set; }

    [SerializeField] private bool isGameScene;
    [SerializeField] private SaveManager saveManager;
    [SerializeField] private Difficulty difficulty;

    private string difficultyText;
    private float gameDuration;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        if (isGameScene)
        {
            Enemy.Instance.OnEnemyDied += Enemy_OnEnemyDied;
            Enemy.Instance.OnStalemate += Enemy_OnStalemate;
            Player.Instance.OnPlayerDied += Player_OnPlayerDied;
        }

        switch(difficulty)
        {
            case Difficulty.Easy:
                difficultyText = "easy";
                break;
            case Difficulty.Medium:
                difficultyText = "medium";
                break;
            case Difficulty.Hard:
                difficultyText = "hard";
                break;
        }

        gameDuration = 0;
    }

    private void Update()
    {
        gameDuration += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Q)) Application.Quit();
    }

    private void OnDisable()
    {
        if (isGameScene)
        {
            Enemy.Instance.OnEnemyDied -= Enemy_OnEnemyDied;
            Enemy.Instance.OnStalemate += Enemy_OnStalemate;
            Player.Instance.OnPlayerDied -= Player_OnPlayerDied;
        }
    }

    private void Enemy_OnEnemyDied(object sender, EventArgs e)
    {
        HandleGameResult(true, false);
        SceneManager.LoadScene("Results");
    }

    private void Player_OnPlayerDied(object sender, EventArgs e)
    {
        HandleGameResult(false, false);
        SceneManager.LoadScene("Results");
    }

    private void Enemy_OnStalemate(object sender, EventArgs e)
    {
        HandleGameResult(false, true);
        SceneManager.LoadScene("Results");
    }

    public void ReturnToGame()
    {
        SceneManager.LoadScene(0);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PickLevel(string level)
    {
        SceneManager.LoadScene(level);
    }

    public void LevelSelect()
    {
        SceneManager.LoadScene("LevelSelect");
    }

    private void HandleGameResult(bool isWin, bool isDraw)
    {
        int cardsPlayerPlayed = Player.Instance.CardsSummoned;
        int cardsEnemyPlayed = Enemy.Instance.CardsSummoned;
        int totalCardsPlayed = cardsEnemyPlayed + cardsPlayerPlayed;
        int enemyBlood = Enemy.Instance.GetBlood();
        int playerBlood = Player.Instance.GetBlood();

        GameAttempt attempt = new GameAttempt(isWin, isDraw, gameDuration, totalCardsPlayed, cardsPlayerPlayed, cardsEnemyPlayed, enemyBlood, playerBlood, difficultyText);

        saveManager.AddGameAttempt(attempt);
    }
}
