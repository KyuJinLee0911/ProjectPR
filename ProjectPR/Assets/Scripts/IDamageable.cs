using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IDamageable
{
    public float Health { get; set; }
    public int Defence { get; set; }

    public void Die();
    public void GetDamaged();
    public void RestoreHealth();
}
