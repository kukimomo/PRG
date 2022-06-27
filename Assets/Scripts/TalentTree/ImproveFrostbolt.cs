using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ImproveFrostbolt :  Talent,IPointerEnterHandler,IPointerExitHandler,IDescribable
{
    public override bool Click()
    {
        if (base.Click())
        {
            //gives the player talent's  ability
            SpellBook.MyInstance.GetSpell("Frostbolt").MyRange+= 1f;
            return true;
        }

        return false;
    }
    public string GetDescription()
    {
        return string.Format("improved Frostball\n<color=#ffd100>Increase the range\nof your Frostbolt by 1.</color>");
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        UIManager.MyInstance.ShowTooltip(new Vector2(1,0),transform.position,this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.MyInstance.HideTooltip();
    }

}
