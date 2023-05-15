using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class SaveManager : MonoBehaviour
{
    private List<GameAttempt> attempts = new List<GameAttempt>();
    private bool saveFileExists;

    private void Awake()
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
        var ES3Settings = new ES3Settings(ES3.EncryptionType.AES, "239fhj91llas10hdn01;aeql");
        ES3.Save("attempts", attempts, ES3Settings);
    }

    private void LoadGame()
    {
        saveFileExists = ES3.FileExists();

        if (saveFileExists)
        {
            var ES3Settings = new ES3Settings(ES3.EncryptionType.AES, "239fhj91llas10hdn01;aeql");
            attempts =  ES3.Load("attempts", ES3Settings) as List<GameAttempt>;
            return;
        }
    }

    public List<GameAttempt> GetGameAttempts() => attempts;
}
