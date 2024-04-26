using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Player : MonoBehaviour, IDamageable, IUnitStats
{
    [SerializeField]
    private PlayerData playerData;

    [SerializeField]
    PlayerData[] playerDatas;

    private CreatureType creatureType = CreatureType.CT_PLAYER;
    public PlayerClass playerClass = PlayerClass.PC_BEGINNER;
    public CreatureState creatureState = CreatureState.CS_IDLE;

    public float Health { get => playerData.Hp; set { } }
    public int Defence { get => playerData.Defence; set { } }
    public float Stamina { get { return playerData.Stamina; } set { } }
    public int Damage { get => playerData.Damage; set { } }

    [SerializeField]
    private int lv = 1;
    public int Level { get => lv; set => lv = value; }

    [SerializeField]
    private float exp = 0.00f;
    public float Exp { get => exp; set => exp = value; }

    [SerializeField]
    private int str = 1;
    public int Strength { get => str; set => str = value; }

    [SerializeField]
    private int dex = 1;
    public int Dexerity { get => dex; set => dex = value; }

    [SerializeField]
    private int inteligence = 1;
    public int Inteligence { get => inteligence; set => inteligence = value; }

    Animator animator;
    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
    }

    public void SelectClass(int classNum)
    {
        if (playerClass != PlayerClass.PC_BEGINNER)
            return;

        playerData = playerDatas[classNum];

        Debug.Log($"Class : {playerData.ClassName}, HP : {Health}, Stamina : {Stamina}, Defence : {Defence}, Damage : {Damage}");

    }

    public void ChangeStats(int statNum)
    {
        switch (statNum)
        {
            case 0: Strength++;
                break;
            case 1: Dexerity++;
                break;
            case 2: Inteligence++;
                break;
        }

        Debug.Log($"str : {Strength}, dex : {Dexerity}, int : {Inteligence}");
    }

    public void Die()
    {
        creatureState = CreatureState.CS_DEAD;
    }

    public void GetDamaged()
    {
        
    }

    public void RestoreHealth()
    {

    }

    public void Attack()
    {
        animator.SetTrigger("Attack");
    }
}
