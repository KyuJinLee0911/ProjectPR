using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Player : MonoBehaviour, IMoveable, IDamageable, IUnitStats
{
    [SerializeField]
    private PlayerData playerData;

    [SerializeField]
    PlayerData[] playerDatas;

    private CreatureType creatureType = CreatureType.CT_PLAYER;
    public PlayerClass playerClass = PlayerClass.PC_BEGINNER;

    public float MoveSpeed { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public float Acceleration { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
    public float Health { get => playerData.Hp; set { } }
    public int Defence { get => playerData.Defence; set { } }
    public float Stamina { get { return playerData.Stamina; } set { } }
    public int Damage { get => playerData.Damage; set { } }

    [SerializeField]
    private int str = 1;
    public int Strength { get => str; set => str = value; }

    [SerializeField]
    private int dex = 1;
    public int Dexerity { get => dex; set => dex = value; }

    [SerializeField]
    private int inteligence = 1;
    public int Inteligence { get => inteligence; set => inteligence = value; }

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

    public void ChangeClass()
    {

    }

    public void Die()
    {

    }

    public void GetDamaged()
    {

    }

    public void GoBackward()
    {

    }

    public void GoForward()
    {

    }

    public void RestoreHealth()
    {

    }
}
