using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

[CreateAssetMenu(fileName = "SkillData", menuName = "ScriptableObject/Skill Data", order = int.MaxValue)]
public class Skill : ScriptableObject, ISwitchable
{
    public string skillName = "default";
    public int level = 0;
    [SerializeField]
    private float baseDamage = 0;
    [SerializeField]
    private float additionalDamage = 0;
    public float damage = 0;
    [SerializeField]
    private bool isActive = true;
    public bool IsActive { get => isActive; }
    public float cooltime = 0;
    public float timeLeft = 0;

    public void LevelUp(int point)
    {
        level += point;
        additionalDamage += baseDamage * 0.01f * point;
        damage = baseDamage + additionalDamage;

        Debug.Log($"Skill name = {name}, skill level = {level}, skill damage = {damage}");
    }

    public void Activate()
    {
        if (isActive)
            return;

        isActive = true;
    }

    public void Deactivate()
    {
        if (!isActive)
            return;

        isActive = false;

        MonoInstance.instance.StartCoroutine(WaitCooltime());
    }

    public void UseSkill()
    {
        Debug.Log("SkillUsed");
    }

    public IEnumerator WaitCooltime()
    {
        timeLeft = cooltime;
        while (timeLeft > 0)
        {
            yield return new WaitForSeconds(Time.deltaTime);

            timeLeft -= Time.deltaTime;
        }

        Activate();
    }

}
