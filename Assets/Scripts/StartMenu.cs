using System.Collections;
using System.Collections.Generic;
using System.IO.Enumeration;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    [SerializeField] private Button continueButton;
    [SerializeField] private Button loadButton;

    [SerializeField] private Button gameLoadPrefab;
    [SerializeField] private RectTransform gameLoadParent;
    [SerializeField] private EventSystem eventSystem;

    private bool filesExist;

    private void Start()
    {
        filesExist = FilePersister.Instance.SaveFileExists();

        continueButton.interactable = filesExist;
        loadButton.interactable = filesExist;

        CreateLoadFiles();
        AddSelectButtonSound(transform);
    }

    public void PlayHoverSound()
    {
        AkSoundEngine.PostEvent("UI_CUrsor", gameObject);
    }

    private void CreateLoadFiles()
    {
        if (!filesExist) return;

        foreach (string file in FilePersister.Instance.GetSaveFileNames())
        {
            string fileName = RemoveFileExtension(file);

            Button instantiatedButton = Instantiate(gameLoadPrefab, gameLoadParent);

            instantiatedButton.gameObject.GetComponentInChildren<TMP_Text>().text = fileName;
        }

        float prefabSize = gameLoadPrefab.GetComponent<RectTransform>().rect.size.y + 16f;

        gameLoadParent.sizeDelta = new Vector2(700f, prefabSize * gameLoadParent.childCount);
    }

    private string RemoveFileExtension(string fileName)
    {
        return fileName.Substring(0, fileName.Length - 4);
    }

    public void ContinueGame()
    {
        string firstFile = RemoveFileExtension(FilePersister.Instance.GetSaveFileNames()[0]);

        FilePersister.Instance.SetFile(firstFile);
        ScheneManager.Instance.LevelSelect();
    }

    private void AddSelectButtonSound(Transform root)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent(out Button button))
            {
                button.onClick.AddListener(() =>
                {
                    AkSoundEngine.PostEvent("UI_Select", button.gameObject);
                });
            }

            AddSelectButtonSound(child);
        }
    }
}
