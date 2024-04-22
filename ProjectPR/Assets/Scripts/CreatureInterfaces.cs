using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreatureInterfaces : MonoBehaviour
{
    
}

public enum CreatureType
{
    CT_PLAYER = 0,
    CT_NPC = 1,
    CT_MONSTER = 2
}

public enum PlayerClass
{
    PC_BEGINNER = 0,
    PC_WARRIOR = 1,
    PC_WIZARD = 2,
    PC_ARCHER = 3
}

public enum MonsterType
{
    MT_NORMAL = 0,
    MT_NAMED = 1,
    MT_BOSS = 2
}
public interface IDamageable
{
    public float Health { get; set; }
    public int Defence { get; set; }
    public int Damage {get; set;}

    public void Die();
    public void GetDamaged();
    public void RestoreHealth();
}

public interface IUnitStats
{
    public float Stamina { get; set; }

    public int Strength { get; set; }
    public int Dexerity { get; set; }
    public int Inteligence { get; set; }
}

public interface IMoveable
{
    public float MoveSpeed { get; set; }

    public void GoForward();
    public void GoBackward();
}