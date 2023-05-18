using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelSelectMenu : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI welcomeText;

    [Header("Cabin Fields")]
    [SerializeField] private TextMeshProUGUI cabinField;
    [SerializeField] private TextMeshProUGUI cabinLore;
    [SerializeField] private TextMeshProUGUI cabinButtonText;
    [SerializeField] private Button cabinButton;
    
    [Header("Cabin Fields")]
    [SerializeField] private TextMeshProUGUI studyTitle;
    [SerializeField] private TextMeshProUGUI studyLore;
    [SerializeField] private TextMeshProUGUI studyButtonText;
    [SerializeField] private Button studyButton;

    private bool beatDungeon;
    private bool beatCabin;

    private void Start()
    {
        welcomeText.text = $"Welcome, {FilePersister.Instance.GetFileName()}";

        CheckProgression();
        SetLevelText();
        AddSelectSoundToButtons(transform);
    }

    private void CheckProgression()
    {
        List<GameAttempt> attempts = SaveManager.Instance.GetGameAttempts();

        foreach (GameAttempt attempt in attempts)
        {
            if (attempt.difficulty == "easy" &&
                attempt.isWin)
            {
                beatDungeon = true;
            }

            if (attempt.difficulty == "medium" &&
                attempt.isWin)
            {
                beatCabin = true;
            }
        }
    }

    private void SetLevelText()
    {
        if (!beatDungeon)
        {
            cabinField.text = "Locked";
            cabinButtonText.text = "Locked";
            cabinLore.text = "Escape from the dungeon to unlock this level";
            cabinButton.interactable = false;

            studyTitle.text = "Locked";
            studyButtonText.text = "Locked";
            studyLore.text = "Escape from the dungeon to learn how to unlock this level";
            studyButton.interactable = false;
        }

        if (beatDungeon &&
            !beatCabin)
        {
            studyTitle.text = "Locked";
            studyButtonText.text = "Locked";
            studyLore.text = "Escape from the cabin to unlock this level";
            studyButton.interactable = false;
        }
    }

    private void AddSelectSoundToButtons(Transform root)
    {
        foreach (Transform child in root)
        {
            if (child.TryGetComponent(out Button button))
            {
                button.onClick.AddListener(() =>
                {
                    AkSoundEngine.PostEvent("UI_Select", child.gameObject);
                });
            }

            AddSelectSoundToButtons(child);
        }
    }

    public void PlayHoverSound(Button button)
    {
        if (!button.interactable) return;
        AkSoundEngine.PostEvent("UI_Cursor", gameObject);
    }
}
