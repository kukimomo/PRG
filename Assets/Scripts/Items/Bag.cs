using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Bag",menuName="Items/Bag",order = 1)]
public class Bag : Item,IUseable
{
    [SerializeField]
    private int slots;

    [SerializeField]
    private GameObject bagPrefab;

    public BagScript MyBagScript // a reference to the bagScript,that this  bag belongs to
    {
        get;
        set;
    }

    public BagButton MyBagButton // a reference to the  bag button this bag is attached to
    {
        get;
        set;
    }

    public int MySlotCount
    {
        get
        {
            return slots;
        }
    }

    public void Initialize(int slots)
    {
        this.slots = slots;
    }

    public void Use()
    {
        if (InventoryScript.MyInstance.CanAddBag)
        {
            Remove();
            MyBagScript = Instantiate(bagPrefab,InventoryScript.MyInstance.transform).GetComponent<BagScript>();
            MyBagScript.AddSlots(slots);


            if (MyBagButton==null)
            {
                InventoryScript.MyInstance.AddBag(this);
            }
            else
            {
                InventoryScript.MyInstance.AddBag(this,MyBagButton);
            }

            MyBagScript.MyBagIndex = MyBagButton.MyBagIndex;
        }
       
    }

    public void SetupScript()
    {
        MyBagScript = Instantiate(bagPrefab, InventoryScript.MyInstance.transform).GetComponent<BagScript>();
        
        MyBagScript.AddSlots(slots);
    }

    public override string GetDescription()
    {
        return base.GetDescription() + string.Format("\n{0} slot bag", slots);
    }
}
