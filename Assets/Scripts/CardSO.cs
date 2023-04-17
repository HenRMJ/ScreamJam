using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Card", menuName = "Card")]
public class CardSO : ScriptableObject
{
    [SerializeField] private Material cardImage;
    [TextArea(2, 5)] [SerializeField] private string description;

    [Header("Base Card Values")]
    [SerializeField] private int cost;
    [SerializeField] private int damage;
    [SerializeField] private int health;
    
    public string GetName() => name;
    public Material GetCardImage() => cardImage;
    public string GetDescription() => description;
    public int GetCost() => cost;
    public int GetDamage() => damage;
    public int GetHealth() => health;
}
