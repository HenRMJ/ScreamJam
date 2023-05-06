using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BloodVial : MonoBehaviour
{
    [SerializeField] private Hand hand;
    [SerializeField] private Transform bloodLevel;
    [SerializeField] private GameObject canvas;
    [SerializeField] private TextMeshProUGUI bloodLevelText;

    private int startingBlood;
    private int currentBlood;

    // Start is called before the first frame update
    void Start()
    {
        if (!hand.BelongsToPlayer)
        {
            Enemy.Instance.OnEnemyHealthChanged += Enemy_OnEnemyHealthChanged;
            startingBlood = Enemy.Instance.Blood;
            currentBlood = startingBlood;
        } else
        {
            Player.Instance.OnPlayerHealthChanged += Player_OnPlayerHealthChanged;
            startingBlood = Player.Instance.GetBlood();
            currentBlood = startingBlood;
        }

        hand.OnCardSummoned += Hand_OnCardSummoned;

    }

    private void Update()
    {
        canvas.SetActive(false);

        if (Utils.GetTransformUnderCursor() != transform) return;
        
        canvas.SetActive(true);
        canvas.transform.forward = Camera.main.transform.forward;
    }

    private void OnDisable()
    {
        if (!hand.BelongsToPlayer)
        {
            Enemy.Instance.OnEnemyHealthChanged -= Enemy_OnEnemyHealthChanged;
        }
        else
        {
            Player.Instance.OnPlayerHealthChanged -= Player_OnPlayerHealthChanged;
        }

        hand.OnCardSummoned -= Hand_OnCardSummoned;
    }

    private void Player_OnPlayerHealthChanged(object sender, EventArgs e)
    {
        currentBlood = Player.Instance.GetBlood();
        bloodLevelText.text = currentBlood.ToString();
        StartCoroutine(UpdateBloodLevel());
    }

    private void Enemy_OnEnemyHealthChanged(object sender, EventArgs e)
    {
        currentBlood = Enemy.Instance.Blood;
        bloodLevelText.text = currentBlood.ToString();
        StartCoroutine(UpdateBloodLevel());
    }

    private void Hand_OnCardSummoned(object sender, Transform e)
    {
        if (e.TryGetComponent(out PlaySummonSound summonSoundComponent))
        {
            SummonSound summonSound = summonSoundComponent.GetSummonSound();

            switch (summonSound)
            {
                case SummonSound.High:
                    AkSoundEngine.PostEvent("BloodPayHigh", gameObject);
                    break;
                case SummonSound.Medium:
                    AkSoundEngine.PostEvent("BloodPayMed", gameObject);
                    break;
                case SummonSound.Low:
                    AkSoundEngine.PostEvent("BloodPaySmall", gameObject);
                    break;
            }
        }
        
    }

    private IEnumerator UpdateBloodLevel()
    {
        float t = 0f;
        float speed = 1f;

        Vector3 startingBloodLevel = bloodLevel.localScale;
        float newBloodPercent = (float)currentBlood / (float)startingBlood;
        Vector3 finalBloodLevel = new Vector3(1, newBloodPercent, 1);
        while (t < 1)
        {
            t += Time.deltaTime / speed;
            if (t > 1) { t = 1; }

            bloodLevel.localScale = Vector3.Lerp(startingBloodLevel, finalBloodLevel, t);
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
}
