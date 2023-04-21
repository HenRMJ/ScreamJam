using System;
using UnityEngine;

/// <summary>
/// Class <c>Player</c> represents each player in the game including
/// their personal attributes (i.e. Health), Deck, Hand and PlayArea
/// </summary>
public class Player : MonoBehaviour
{
    public bool CanSummon { get; private set; }

    [SerializeField] private int blood;
    [SerializeField] private Hand playerHand;

    // Start is called before the first frame update
    void Start()
    {
        playerHand.OnCardSummoned += PlayerHand_OnCardSummoned;
        StartRoundState.OnStartRound += StartRoundState_OnStartRound;
    }

    private void StartRoundState_OnStartRound(object sender, EventArgs e)
    {
        CanSummon = true;
    }

    private void PlayerHand_OnCardSummoned(object sender, EventArgs e)
    {
        CanSummon = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool TrySummonCard(int cost)
    {
        if (cost >= blood) return false;
        if (cost <= 0) return true;

        blood -= cost;
        return true;
    }
}
