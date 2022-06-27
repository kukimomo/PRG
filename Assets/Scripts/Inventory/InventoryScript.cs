using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;


public delegate void ItemCountChanged (Item item);

public class InventoryScript : MonoBehaviour
{
    public event ItemCountChanged itemCountChangedEvent;
    
    private static InventoryScript instance;

    public  static InventoryScript MyInstance
    {
        get
        {
            if(instance==null)
            {
                instance=FindObjectOfType<InventoryScript>();
            }
            return instance;
        }
    }

    private SlotScript fromSlot;

    private List<Bag> bags = new List<Bag>();


    [SerializeField]
    private BagButton[] bagButtons;
    
    //for debugging
    [SerializeField]
    private Item[] items;

    public bool CanAddBag
    {
        get { return bags.Count < 5; }
    }

    public int MyEmptySlotCount
    {
        get
        {
            int count = 0;

            foreach (Bag bag in bags)
            {
                count += bag.MyBagScript.MyEmptySlotCount;
            }

            return count;
        }
    }

    public int MyTotalSlotCount
    {
        get
        {
            int count = 0;

            foreach (Bag bag in bags)
            {
                count += bag.MyBagScript.MySlots.Count;
            }

            return count;
        }
    }

    public int MyFullSlotCount
    {
        get
        {
            return MyTotalSlotCount - MyEmptySlotCount;
        }
    }
    
    public SlotScript FromSlot
    {
        get
        {
            return fromSlot;
        }
        set
        {
            fromSlot = value;
            if (value !=null)
            {
                fromSlot.MyIcon.color=Color.grey;
            }
        }
    }

    public List<Bag> MyBags
    {
        get
        {
            return bags;
        }
    }

    private void Awake()
    {
        Bag bag = (Bag) Instantiate(items[0]);
        bag.Initialize(20);
        bag.Use();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            Bag bag = (Bag) Instantiate(items[0]);
            bag.Initialize(20);
            bag.Use();
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            Bag bag = (Bag) Instantiate(items[0]);
            bag.Initialize(20);
            AddItem(bag);
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            HealthPotion potion = (HealthPotion) Instantiate(items[1]);
            AddItem(potion);
        }

        if (Input.GetKeyDown(KeyCode.H))
        {
            GoldNugget nugget = (GoldNugget) Instantiate(items[11]);
            AddItem(nugget);
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            AddItem((Armor) Instantiate(items[2]));
            AddItem((Armor) Instantiate(items[3]));
            AddItem((Armor) Instantiate(items[4]));
            AddItem((Armor) Instantiate(items[5]));
            AddItem((Armor) Instantiate(items[6]));
            AddItem((Armor) Instantiate(items[7]));
            AddItem((Armor) Instantiate(items[8]));
            AddItem((Armor) Instantiate(items[9]));
            AddItem((Armor) Instantiate(items[10]));
        }
    }

    public void AddBag(Bag bag)
    {
        foreach (BagButton bagButton in bagButtons)
        {
            if (bagButton.MyBag == null)
            {
                bagButton.MyBag = bag;
                bags.Add(bag);
                bag.MyBagButton = bagButton;
                bag.MyBagScript.transform.SetSiblingIndex(bagButton.MyBagIndex);
                break;
            }
        }
    }

    public void AddBag(Bag bag,BagButton bagButton)
    {
        bags.Add(bag);
        bagButton.MyBag = bag;
        bag.MyBagScript.transform.SetSiblingIndex(bagButton.MyBagIndex);
    }

    public void AddBag(Bag bag, int bagIndex)
    {
        bag.SetupScript();
        MyBags.Add(bag);
        bag.MyBagScript.MyBagIndex = bagIndex;//这个错误是视频有的  才加的   我自己并没出现这个错误，经检测，加了也没事！
        bag.MyBagButton = bagButtons[bagIndex];
        bagButtons[bagIndex].MyBag = bag;
    }

    //remove the bag from the inventory
    public void RemoveBag(Bag bag)
    {
        bags.Remove(bag);
        Destroy(bag.MyBagScript.gameObject);
    }

    public void SwapBags(Bag oldBag, Bag newBag)
    {
        int newSlotCount = (MyTotalSlotCount - oldBag.MySlotCount) + newBag.MySlotCount;

        if (newSlotCount-MyFullSlotCount>=0)
        {
            //Do Swap
            List<Item> bagItems = oldBag.MyBagScript.GetItems();
            
            RemoveBag(oldBag);

            newBag.MyBagButton = oldBag.MyBagButton;
            
            newBag.Use();

            foreach (Item item in bagItems)
            {
                if (item != newBag) //no duplicates
                {
                    AddItem(item);
                }
            }
            
            AddItem(oldBag);
            
            HandScript.MyInstance.Drop();

            MyInstance.fromSlot = null;
        }
    }

    public bool AddItem(Item item)
    {
        if (item.MyStackSize > 0)
        {
            if (PlaceInStack(item))
            {
                return true;
            }
        }
        return  PlaceInEmpty(item);
    }

    
    //places an item on an empty slot in the game
    private bool PlaceInEmpty(Item item)
    {
        foreach (Bag bag in bags) //checks all bags
        {
            if (bag.MyBagScript.AddItem(item)) //tries to add the item
            {
                OnItemCountChanged(item);
                return true;//it was possible to add the item
            }
        }

        return false;
    }

    private bool PlaceInStack(Item item)
    {
        foreach (Bag bag in bags)//checks all bags
        {
            foreach (SlotScript slots in bag.MyBagScript.MySlots)
            {
                if (slots.StackItem(item))//tries to stack the item
                {
                    OnItemCountChanged(item);
                    return true;//it was possible to stack the item
                }
            }
        }

        return false;//it was'nt possible to stack the item
    }
    public void OpenClose()
    {
        bool closedBag = bags.Find(X => !X.MyBagScript.IsOpen);
        //if closed bag==true,then open all closed bags
        //if closed bag==false,then close all open bags
        
        foreach (Bag bag in bags)
        {
            if (bag.MyBagScript.IsOpen!=closedBag)
            {
                bag.MyBagScript.OpenClose();
            }
        }
    }

    public List<SlotScript> GetAllItems()
    {
        List<SlotScript> slots = new List<SlotScript>();
        foreach (Bag bag in MyBags)
        {
            foreach (SlotScript slot in bag.MyBagScript.MySlots)
            {
                if (!slot.IsEmpty)
                {
                    slots.Add(slot);
                }
            }
        }
        return slots;
    }

    public void PlaceInSpecific(Item item, int slotIndex, int bagIndex)
    {
        bags[bagIndex].MyBagScript.MySlots[slotIndex].AddItem(item);
    }
    
    public Stack<IUseable> GetUseables(IUseable type)
    {
        Stack<IUseable> useables = new Stack<IUseable>();

        foreach (Bag bag in bags)
        {
            foreach (SlotScript slot in bag.MyBagScript.MySlots )
            {
                if (!slot.IsEmpty && slot.MyItem.GetType()==type.GetType())
                {
                    foreach (Item item in slot.MyItems)
                    {
                        useables.Push(item as IUseable);
                    }
                }
            }
        }

        return useables;
    }

    public IUseable GetUseable(string type)
    {
        Stack<IUseable> useables = new Stack<IUseable>();

        foreach (Bag bag in bags)
        {
            foreach (SlotScript slot in bag.MyBagScript.MySlots )
            {
                if (!slot.IsEmpty && slot.MyItem.MyTitle==type)
                {
                    return (slot.MyItem as IUseable);
                }
            }
        }

        return null;
    }

    public int GetItemCount(string type)
    {
        int itemCount = 0;

        foreach (Bag bag in bags)
        {
            foreach (SlotScript slot in bag.MyBagScript.MySlots)
            {
                if (!slot.IsEmpty&&slot.MyItem.MyTitle==type)
                {
                    itemCount += slot.MyItems.Count;
                }
            }
        }

        return itemCount;
    }

    public Stack<Item> GetItems(string type,int count)
    {
        Stack<Item> items = new Stack<Item>();
        
        foreach (Bag bag in bags)
        {
            foreach (SlotScript slot in bag.MyBagScript.MySlots)
            {
                if (!slot.IsEmpty&&slot.MyItem.MyTitle==type)
                {
                    foreach (Item item in slot.MyItems)
                    {
                        items.Push(item);
                        if (items.Count == count)
                        {
                            return items;
                        }
                    }
                }
            }
        }

        return items;
    }

    public void RemoveItem(Item item)
    {

        foreach (Bag bag in bags)
        {
            foreach (SlotScript slot in bag.MyBagScript.MySlots)
            {
                if (!slot.IsEmpty&&slot.MyItem.MyTitle==item.MyTitle)
                {
                    slot.RemoveItem(item);
                    break;
                }
            }
        }
    }
    
    public void OnItemCountChanged(Item item)
    {
        if (itemCountChangedEvent != null)
        {
            itemCountChangedEvent.Invoke(item);
        }
    }
}
