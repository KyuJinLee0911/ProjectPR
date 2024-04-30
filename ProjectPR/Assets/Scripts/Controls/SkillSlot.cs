using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.UI;

public class SkillSlot : MonoBehaviour, ISwitchable
{
    public Skill skill;

    private bool isActive = true;
    public bool IsActive => isActive;

    public GameObject disabledImage;
    public Text cooltimeText;

    void Awake()
    {

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        cooltimeText.text = skill.timeLeft.ToString("F1");
    }

    public void Activate()
    {
        isActive = true;
        disabledImage.SetActive(false);
    }

    public void Deactivate()
    {
        if(!skill.IsActive)
            return;
            
        skill.UseSkill();
        disabledImage.SetActive(true);
        skill.Deactivate();
        StartCoroutine(Reactivate());
    }

    IEnumerator Reactivate()
    {
        yield return new WaitUntil(() => skill.IsActive == true);
        Activate();
    }

    
}
