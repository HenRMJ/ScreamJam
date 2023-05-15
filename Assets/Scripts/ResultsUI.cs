using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ResultsUI : MonoBehaviour
{
    [SerializeField] private SaveManager saveManager;

    [Header("TMP Fields")]
    [SerializeField] private TextMeshProUGUI gameDurationField;
    [SerializeField] private TextMeshProUGUI totalCardsField, playerCardsField, enemyCardsField, enemyBlood, playerBlood, difficulty;

    [Header("Image to change")]
    [SerializeField] private Image gameResultImage;

    [Header("Sprite States")]
    [SerializeField] private Sprite winSprite;
    [SerializeField] private Sprite loseSprite, drawSprite;

    private List<GameAttempt> attempts = new List<GameAttempt>();
    private GameAttempt lastAttempt = null;

    private void Start()
    {
        attempts = saveManager.GetGameAttempts();
        lastAttempt = attempts.Last();

        UpdateResultsText();
        UpdateWinResult();

        difficulty.text = lastAttempt.difficulty;
    }

    private void UpdateWinResult()
    {
        if (!lastAttempt.isWin)
        {
            if (lastAttempt.isDraw)
            {
                gameResultImage.sprite = drawSprite;
                return;
            }

            gameResultImage.sprite = loseSprite;
            return;
        }

        gameResultImage.sprite = winSprite;
    }

    private void UpdateResultsText()
    {
        gameDurationField.text = ConvertTimeToMinutes();
        totalCardsField.text = lastAttempt.totalCardsPlayed.ToString();
        playerCardsField.text = lastAttempt.cardsPlayerPlayed.ToString();
        enemyCardsField.text = lastAttempt.cardsEnemyPlayed.ToString();
        enemyBlood.text = lastAttempt.enemyBlood.ToString();
        playerBlood.text = lastAttempt.playerBlood.ToString();
    }

    private string ConvertTimeToMinutes()
    {
        float gameDuration = lastAttempt.gameDuration;

        double minute = Math.Floor(gameDuration / 60);
        double seconds = Math.Round(gameDuration % 60, 3);

        string secondsAsString;

        if (seconds.ToString().IndexOf(".") == 1)
        {
            secondsAsString = seconds.ToString().Insert(0, "0");
        }
        else
        {
            secondsAsString = seconds.ToString();
        }

        string runTime = minute.ToString() + ":" + secondsAsString.Replace(".", ":");

        return runTime;
    }
}
