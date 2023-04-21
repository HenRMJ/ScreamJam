using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private Hand playerHand;

    // Start is called before the first frame update
    void Start()
    {
        StartRoundState.OnStartRound += StartRoundState_OnStartRound;
        playerHand.OnCardSummoned += PlayerHand_OnCardSummoned;
    }

    private void PlayerHand_OnCardSummoned(object sender, EventArgs e)
    {
        animator.SetBool("CanSummon", false);
    }

    private void StartRoundState_OnStartRound(object sender, EventArgs e)
    {
        animator.SetBool("CanSummon", true);
    }
}
