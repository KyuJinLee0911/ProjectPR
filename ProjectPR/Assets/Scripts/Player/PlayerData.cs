using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Player Data", menuName = "ScriptableObject/Player Data", order = int.MaxValue)]
public class PlayerData : ScriptableObject
{
    [SerializeField]
    private PlayerClass playerClass;
    public PlayerClass PlayerClass { get { return playerClass; } }

    [SerializeField]
    private string className;
    public string ClassName { get { return className; } }

    [SerializeField]
    private float hp;
    public float Hp { get { return hp; } }

    [SerializeField]
    private int damage;
    public int Damage { get { return damage; }}

    [SerializeField]
    private float stamina;
    public float Stamina { get { return stamina; } }

    [SerializeField]
    private int defence;
    public int Defence { get { return defence; } }
}
