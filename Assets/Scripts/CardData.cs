using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

public class CardData : MonoBehaviour
{
    public static event EventHandler OnAnyCardHover;

    public bool InHand { get; set; }
    public bool InDeck { get; set; }
    public bool InPlay { get; set; }
    public bool CanMove { get; set; }
    public bool MarkedAsSacrifice { get; set; }
    public int PositionInHand { get; set; }
    public CardType Type { get; private set;}
    public CardGroup Group { get; private set; }
    public Vector2Int currentPosition { get; set; }

    [SerializeField] private CardSO card;

    [Header("Card Fields")]
    [SerializeField] private TextMeshProUGUI nameField;
    [SerializeField] private TextMeshProUGUI loreField;
    [SerializeField] private TextMeshProUGUI defenseField;
    [SerializeField] private TextMeshProUGUI bloodCostField;
    [SerializeField] private TextMeshProUGUI attackField;
    [SerializeField] private MeshRenderer cardMeshRenderer;

    [Header("Card Movement Values")]
    [SerializeField] private float speed;
    [SerializeField] private Vector3 hoverOffset;

    private int bloodCost, defense, attack;
    private string cardName, UIDescription, loreBlurb;

    private Quaternion rotationalValue;
    private Vector3 targetPoint;
    private Hand hand;
    bool belongsToPlayer;

    private void Start()
    {
        foreach (Hand hand in FindObjectsOfType<Hand>())
        {
            if (hand.BelongsToPlayer && belongsToPlayer)
            {
                this.hand = hand;
            }
            else if (!hand.BelongsToPlayer && !belongsToPlayer)
            {
                this.hand = hand;
            }
        }

        // Assinging CardData
        Type = card.GetCardType();
        bloodCost = card.GetBloodCost();
        Group = card.GetCardGroup();
        cardName = card.GetName();
        UIDescription = card.GetUIDescription();
        loreBlurb = card.GetLoreBlurb();

        // Assigning Visuals
        cardMeshRenderer.material = card.GetCardImage();
        nameField.text = cardName;
        loreField.text = loreBlurb;
        bloodCostField.text = bloodCost.ToString();

        // Checking if Card is a monster to assign attack and defense
        if (Type == CardType.Monster)
        {
            attack = card.GetAttack();
            defense = card.GetDefense();

            attackField.text = attack.ToString();
            defenseField.text = defense.ToString();
        }
        else
        {
            attackField.gameObject.SetActive(false);
            defenseField.gameObject.SetActive(false);
        }

        targetPoint = transform.position;
        rotationalValue = transform.rotation;

        InHand = false;
        InDeck = true;
        InPlay = false;
        CanMove = false;
        MarkedAsSacrifice = false;

        StartRoundState.OnStartRound += StartRoundState_OnStartRound;
    }    

    private void Update()
    {
        HoverInfo();
        HoverCard();
        MoveCard();
    }

    private void OnDestroy()
    {
        if (MarkedAsSacrifice) return;
        if (Type == CardType.Spell) return;

        int rowDiedOn = currentPosition.y;

        Debug.Log("Card Died on row " + rowDiedOn);

        if (belongsToPlayer)
        {
            if (rowDiedOn == 0 || rowDiedOn == 1)
            {
                Player.Instance.Heal(bloodCost);
            } else
            {
                Player.Instance.Heal(Mathf.RoundToInt(bloodCost / 2));
            }
        } else
        {
            int enemyHomeRow = GridManager.Instance.GetGridDimensions().y - 1;

            if (rowDiedOn == enemyHomeRow || rowDiedOn == enemyHomeRow - 1)
            {
                Enemy.Instance.SetBlood(bloodCost);
            } else
            {
                Enemy.Instance.SetBlood(Mathf.RoundToInt(bloodCost / 2));
            }
        }

        StartRoundState.OnStartRound -= StartRoundState_OnStartRound;
    }

    // Might want to move these methods into a different class Move Card and Move To Point
    private void MoveCard()
    {
        if (transform.position == targetPoint && transform.rotation == rotationalValue) return;
        transform.position = Vector3.Lerp(transform.position, targetPoint, Time.deltaTime * speed);
        transform.rotation = Quaternion.Lerp(transform.rotation, rotationalValue, Time.deltaTime * speed);
    }

    public void MoveToPoint(Vector3 pointToMoveTo, Quaternion rotation)
    {
        targetPoint = pointToMoveTo;
        rotationalValue = rotation;
    }

    private void HoverCard()
    {
        if (!InHand) return;
        if (!belongsToPlayer) return;

        // Resets card position if no card is being hovered
        if (Utils.GetCardObjectUnderCursor() == null)
        {
            MoveToPoint(hand.GetPositionInHand(PositionInHand), hand.GetRotationInHand());

            return;
        }

        Transform cardTransform = Utils.GetCardObjectUnderCursor().transform;

        // if the card is being hovered it raises the position, else, it resets the position
        if (cardTransform == transform)
        {
            Vector3 hoverPoint = hand.GetPositionInHand(PositionInHand) + hoverOffset;
            Quaternion hoverRotation = Camera.main.transform.rotation;

            if (targetPoint != hoverPoint)
            {
                AkSoundEngine.PostEvent("BringUp_Card", gameObject);
            }
            
            MoveToPoint(hoverPoint, hoverRotation);
        } else
        {
            MoveToPoint(hand.GetPositionInHand(PositionInHand), hand.GetRotationInHand());
        }
    }

    private void HoverInfo()
    {
        if (InDeck) return;
        if (!InPlay && !belongsToPlayer) return;

        GameObject cursorCard = Utils.GetCardObjectUnderCursor();

        if (cursorCard == null) return;
        if (gameObject != cursorCard) return;

        OnAnyCardHover?.Invoke(this, EventArgs.Empty);
    }

    private void StartRoundState_OnStartRound(object sender, EventArgs e)
    {
        CanMove = true;
    }

    public void SetBloodCost(int newBloodCost)
    {
        bloodCost = newBloodCost;
    }

    public void SetBelongsToPlayer(bool belongsToPlayer)
    {
        this.belongsToPlayer = belongsToPlayer;
    }


    public Hand GetHand() => hand;
    public int GetAttackDamage() => attack;
    public int GetDefenseValue() => defense;
    public bool BelongsToPlayer() => belongsToPlayer;
    public string GetCardUIDescription() => UIDescription;
    public string GetCardName() => cardName;
    public int GetBloodCost() => bloodCost;
}
