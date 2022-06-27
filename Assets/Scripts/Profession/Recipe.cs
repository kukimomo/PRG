using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UI;

public class Recipe : MonoBehaviour,ICastable
{
   [SerializeField]
   private CraftingMaterial[] materials;

   [SerializeField]
   private Item output;

   [SerializeField]
   private int outputCount;
   
   [SerializeField]
   private string description;

   [SerializeField]
   private Image highlight;

   [SerializeField]
   private float craftTime;

   [SerializeField]
   private Color barColor;
   
   
   public Item Output
   {
      get
      {
         return output;
      }
   }

   public int MyOutputCount
   {
      get
      {
         return outputCount;
      }
      set
      {
         outputCount = value;
      }
   }

   public string MyDescription
   {
      get
      {
         return description;
      }
   }

   public CraftingMaterial[] MyMaterials
   {
      get
      {
         return materials;
      }
   }

   void Start()
   {
      GetComponent<Text>().text = output.MyTitle;
   }

   public void Select()
   {
      Color c = highlight.color;
      c.a = 0.3f;
      highlight.color = c;
   }

   public void Deselect()
   {
      Color c = highlight.color;
      c.a = 0f;
      highlight.color = c;
   }

   public string MyTitle
   {
      get
      {
         return Output.MyTitle;
      }
   }

   public Sprite MyIcon
   {
      get
      {
         return Output.MyIcon;
      }
   }

   public float MyCastTime
   {
      get
      {
         return craftTime;
      }
   }

   public Color MyBarColor
   {
      get
      {
         return barColor;
      }
   }
}
