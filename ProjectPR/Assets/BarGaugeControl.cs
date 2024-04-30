using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum BarType
{
    BT_HP = 0,
    BT_STM = 1,
    BT_EXP = 2
}

public class BarGaugeControl : MonoBehaviour
{
    public Player player;
    Slider bar;
    public BarType barType;

    // Start is called before the first frame update
    void Start()
    {
        bar = gameObject.GetComponent<Slider>();
        switch(barType)
        {
            case BarType.BT_HP:
                bar.maxValue = player.Health;
                break;
            case BarType.BT_STM:
                bar.maxValue = player.Stamina;
                break;
            case BarType.BT_EXP:
                bar.maxValue = player.Exp;
                break;
        }
    }

    // Update is called once per frame
    void Update()
    {
        switch(barType)
        {
            case BarType.BT_HP:
                bar.value = player.Health;
                break;
            case BarType.BT_STM:
                bar.value = player.currentStamina;
                break;
            case BarType.BT_EXP:
                bar.value = player.Exp;
                break;
        }
    }
}
