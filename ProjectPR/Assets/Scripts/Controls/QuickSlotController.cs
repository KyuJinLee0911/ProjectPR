using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class QuickSlot : MonoBehaviour
{
    public SkillSlot[] quickSlots = new SkillSlot[5];

    void OnQuickSlot1()
    {
        QuickSlotUse(0);
    }

    void OnQuickSlot2()
    {
        QuickSlotUse(1);
    }

    void OnQuickSlot3()
    {
        QuickSlotUse(2);
    }

    void OnQuickSlot4()
    {
        QuickSlotUse(3);
    }

    void OnQuickSlot5()
    {
        QuickSlotUse(4);
    }

    void QuickSlotUse(int slotNum)
    {
        quickSlots[slotNum].Deactivate();
    }
}
