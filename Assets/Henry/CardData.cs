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
    [SerializeField] private TextMeshProUGUI healthField;
    [SerializeField] private TextMeshProUGUI costField;
    [SerializeField] private TextMeshProUGUI damageField;
    [SerializeField] private MeshRenderer cardMeshRenderer;
    

    private int health;
    private int cost;
    private int damage;

    private void Start()
    {
        health = card.GetHealth();
        cost = card.GetCost();
        damage = card.GetDamage();

        nameField.text = card.GetName();
        descriptionField.text = card.GetDescription();
        healthField.text = health.ToString();
        costField.text = cost.ToString();
        damageField.text = damage.ToString();
        cardMeshRenderer.material = card.GetCardImage();
    }
}
