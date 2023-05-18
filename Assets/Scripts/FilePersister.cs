using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FilePersister : MonoBehaviour
{
    public static FilePersister Instance;

    private bool saveFileExists = false;
    private string fileName = null;

    private void Awake()
    {
        if (Instance != null) 
        {
            Debug.Log("This file persister is not necessary and has been deleted");
            Destroy(this);
            return;
        }

        Instance = this;

        DontDestroyOnLoad(this);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void Start()
    {
        ES3Settings ES3Settings = new ES3Settings(ES3.EncryptionType.AES, "239fhj91llas10hdn01;aeql");

        if (ES3.DirectoryExists("saves/", ES3Settings))
        {
            if (ES3.GetFiles("saves/", ES3Settings).Length <= 0)
            {
                saveFileExists = false;
            } else
            {
                saveFileExists = true;
            }
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (fileName == null)
        {
            SaveManager.Instance.NameSaveFile("DefaultSave");
        } else
        {
            SaveManager.Instance.NameSaveFile(fileName);
        }

        AkSoundEngine.StopAll();
    }

    public string[] GetSaveFileNames()
    {
        ES3Settings ES3Settings = new ES3Settings(ES3.EncryptionType.AES, "239fhj91llas10hdn01;aeql");

        return ES3.GetFiles("saves/", ES3Settings);
    }
    public void SetFile(TMP_InputField newFileName)
    {
        fileName = newFileName.text;
        SaveManager.Instance.NameSaveFile(fileName);
    }

    public void SetFile(string newFileName)
    {
        fileName = newFileName;
        SaveManager.Instance.NameSaveFile(fileName);
    }

    public string GetFileName() => fileName;
    public bool SaveFileExists() => saveFileExists;
}
