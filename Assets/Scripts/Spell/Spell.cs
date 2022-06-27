using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

[Serializable]
public class Spell:IUseable,IMoveable,IDescribable,ICastable
{
   [SerializeField]
   private string title;

   [SerializeField]
   private float damage;

   [SerializeField]
   private float Range;
   
   [SerializeField]
   private Sprite   icon;

   [SerializeField]
   private float speed;

   [SerializeField]
   private float castTime;

   [SerializeField]
   private GameObject spellPrefab;

   [SerializeField]
   private string description;

   [SerializeField]
   private Color barColor;

   public string MyTitle
   {
      get
      {
         return title;
      }
   } 
   public float MyDamage 
   {
      get
      {
        return Mathf.Ceil(damage);
      }
      set
      {
         damage = value;
      }
      
   }

   
   public Sprite MyIcon
   {
      get
      {
         return icon;
      }
      
   }
  public float MySpeed
  {
     get
     {
        return speed;
     }
    
  }
  public float MyCastTime
  {
     get
     {
         return castTime;
     }
     set
     {
        castTime = value;
     }
    
  }
  public GameObject MySpellPrefab
  {
     get
     {
        return spellPrefab;
     }
     
  }
  public Color MyBarColor
  {
     get
     {
        return barColor;
     }
  }

  public float MyRange
  {
     get
     {
        return Range;
     }
     set
     {
        Range = value;
     }
  }
  public string GetDescription()
  {
     return string.Format("{0}\nCast time:{1} second(s)\n<color=#ffd111>{2}\nthat causes {3} damage</color>", title,castTime,description,MyDamage);
  }

  public void Use()
  {
     Player.MyInstance.CastSpell(this);
  }
}
