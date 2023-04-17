using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardData : MonoBehaviour
{
    [SerializeField] private CardSO card;

    [Header("Card Fields")]
    [SerializeField] private TextMeshProUGUI nameField;
    [SerializeField] private TextMeshProUGUI descriptionField;
    [SerializeField] private TextMeshProUGUI defenseField;
    [SerializeField] private TextMeshProUGUI bloodCostField;
    [SerializeField] private TextMeshProUGUI attackField;
    [SerializeField] private MeshRenderer cardMeshRenderer;

    private CardType cardType;
    private int bloodCost;
    private int defense;
    private int attack;
    

    private void Start()
    {
        cardType = card.GetCardType();
        bloodCost = card.GetBloodCost();


        cardMeshRenderer.material = card.GetCardImage();
        nameField.text = card.GetName();

        descriptionField.text = card.GetDescription();
        bloodCostField.text = bloodCost.ToString();
        

        if (cardType == CardType.Monster)
        {
            attack = card.GetAttack();
            defense = card.GetDefense();

            attackField.text = attack.ToString();
            defenseField.text = defense.ToString();
        } else
        {
            attackField.gameObject.SetActive(false);
            defenseField.gameObject.SetActive(false);
        }
    }
}
