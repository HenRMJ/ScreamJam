using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;
using System;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    private List<GameAttempt> attempts = new List<GameAttempt>();
    private bool saveFileExists;
    private string fileName = null;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(this);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        LoadGame();
    }

    private void OnDestroy()
    {
        SaveGame();
    }

    public void AddGameAttempt(GameAttempt attempt)
    {
        attempts.Add(attempt);
    }

    private void SaveGame()
    {
        if (fileName == null)
        {
            fileName = "defaultSave";
        }

        ES3Settings ES3Settings = new ES3Settings(ES3.EncryptionType.AES, "239fhj91llas10hdn01;aeql");

        ES3.Save("attempts", attempts, $"saves/{fileName}.es3", ES3Settings);
    }

    private void LoadGame()
    {
        Debug.Log(fileName);
        ES3Settings ES3Settings = new ES3Settings(ES3.EncryptionType.AES, "239fhj91llas10hdn01;aeql");

        saveFileExists = ES3.FileExists($"saves/{fileName}.es3", ES3Settings);

        if (saveFileExists)
        {
            attempts =  ES3.Load("attempts", $"saves/{fileName}.es3", ES3Settings) as List<GameAttempt>;
            return;
        }
    }

    public void NameSaveFile(string nameOfFile)
    {
        fileName = nameOfFile;
        Debug.Log(fileName);
    }
    public List<GameAttempt> GetGameAttempts() => attempts;
}
