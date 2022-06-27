using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ImproveThunderBolt :  Talent,IPointerEnterHandler,IPointerExitHandler,IDescribable
{
    private int percent = 5;
      public override bool Click()
      {
          if (base.Click())
          {
              Spell thunderBolt = SpellBook.MyInstance.GetSpell("Thunderbolt");
              
              //gives the player talent's  ability
              Debug.Log("thunder damager is===="+thunderBolt.MyDamage);
              thunderBolt.MyDamage = thunderBolt.MyDamage*(1+percent/(float)100) ;
              Debug.Log("after improve,thunder damage is===="+thunderBolt.MyDamage);
              return true;
          }
  
          return false;
      }
      public string GetDescription()
      {
          return string.Format($"improved Thunderbolt\n<color=#ffd100>increases the damage\nof your thunderBolt by {percent}%</color>");
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
