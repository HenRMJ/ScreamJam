using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

#endif

[CreateAssetMenu(fileName ="New Card", menuName = "Card")]
public class CardSO : ScriptableObject
{
    [SerializeField] private Material cardImage;
    [TextArea(2, 5)] [SerializeField] private string description;
    [SerializeField] private CardType cardType;

    [Header("Base Card Stats")]
    [SerializeField] private int bloodCost;

    [SerializeField, HideInInspector] private int attack, defense;

    #region Editor
#if UNITY_EDITOR

    [CustomEditor(typeof(CardSO))]
    public class CardStatEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            CardSO cardSO = (CardSO)target;

            if (cardSO.cardType == CardType.Monster)
            {
                DrawMonsterStats(cardSO);
            }
        }

        private static void DrawMonsterStats(CardSO cardSO)
        {
            EditorGUILayout.LabelField("Monster Stats");

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("ATK", GUILayout.MaxWidth(25));
            cardSO.attack = EditorGUILayout.IntField(cardSO.attack, GUILayout.MaxWidth(50));

            EditorGUILayout.LabelField(" ", GUILayout.MaxWidth(50));

            EditorGUILayout.LabelField("DEF", GUILayout.MaxWidth(25));
            cardSO.defense = EditorGUILayout.IntField(cardSO.defense, GUILayout.MaxWidth(50));

            EditorGUILayout.EndHorizontal();
        }
    }

#endif
    #endregion

    public string GetName() => name;
    public Material GetCardImage() => cardImage;
    public string GetDescription() => description;
    public int GetBloodCost() => bloodCost;
    public int GetAttack() => attack;
    public int GetDefense() => defense;
    public CardType GetCardType() => cardType;
}
