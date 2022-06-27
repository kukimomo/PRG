using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public  abstract  class Item : ScriptableObject,IMoveable,IDescribable
{
    //icon used when moving and placing the items
    [SerializeField]
    private Sprite icon;

    //the size of the stack,less than 2 is not stackable
    [SerializeField]
    private int stackSize;

    [SerializeField]
    private Quality quality;
    
    [SerializeField]
    private string title;
    
    private SlotScript slot;//a reference to the slot that this item is setting on

    private CharButton charButton;

    [SerializeField]
    private int price;
    
    public Sprite MyIcon //property for accessing the icon
    {
        get
        {
            return icon;
        }
    }

    public int MyStackSize
    {
        get
        {
            return stackSize;
        }
    }

    public  SlotScript MySlots
    {
        get
        {
            return slot;
        }
        set
        {
            slot = value;
        }
    }

    public int MyPrice
    {
        get
        {
            return price;
        }
    }

    public virtual string GetDescription()//return a description of this specific item
    {
        return string.Format("<color={0}>{1}</color>",QualityColor.MyColors[quality],title);
    }

    
    //removes the item rom the inventory
    public void Remove()
    {
        if (MySlots != null)
        {
            MySlots.RemoveItem(this);
          
        }
    }

    public Quality MyQuality
    {
        get
        {
            return quality;
        }
    }

    public string MyTitle
    {
        get
        {
            return title;
        }
    }

    public CharButton MyCharButton
    {
        get
        {
            return charButton;
        }
        set
        {
            MySlots = null;
            charButton = value;
        }
    }
}
