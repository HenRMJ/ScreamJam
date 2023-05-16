using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadGame : MonoBehaviour
{
    [SerializeField] private Button loadButton;
    [SerializeField] private TextMeshProUGUI loadButtonText;

    private ScheneManager sceneManager;

    private void Start()
    {
        sceneManager = FindObjectOfType<ScheneManager>();

        loadButton.onClick.AddListener(() =>
        {
            FilePersister.Instance.SetFile(loadButtonText.text);
            sceneManager.LevelSelect();
        });
    }

    public void PlayHoverSound()
    {
        AkSoundEngine.PostEvent("UI_Cursor", gameObject);
    }
}
