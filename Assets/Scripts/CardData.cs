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
    private int bloodCost, defense, attack;

    private Quaternion rotationalValue;
    private Vector3 targetPoint;

    private const float SPEED = 5f;

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

        targetPoint = transform.position;
        rotationalValue = transform.rotation;
    }

    private void Update()
    {
        MoveCard();
    }

    // Might want to move these methods into a different class Move Card and Move To Point
    private void MoveCard()
    {
        if (transform.position == targetPoint && transform.rotation == rotationalValue) return;

        transform.position = Vector3.Lerp(transform.position, targetPoint, Time.deltaTime * SPEED);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotationalValue, Time.deltaTime * SPEED);
    }

    public void MoveToPoint(Vector3 pointToMoveTo, Quaternion rotation)
    {
        targetPoint = pointToMoveTo;
        rotationalValue = rotation;
    }
}
