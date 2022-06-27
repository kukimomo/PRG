using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class SlotScript : MonoBehaviour,IPointerClickHandler,IClickable,IPointerEnterHandler,IPointerExitHandler
{
   private ObservableStack<Item> items = new ObservableStack<Item>();
   
   //a reference to the slot's icon
   [SerializeField]
   private Image icon;

   [SerializeField]
   private Text stackSize;
   
   //a reference to the bag that this slot is belong to
   public BagScript MyBag { get; set; }

   public int MyIndex { get; set; }
   
   //checks if the item is empty
   public bool IsEmpty
   {
      get
      {
         return MyItems.Count == 0;
      }
   }

   public Image MyIcon
   {
      get
      {
         return icon;
      }
      set
      {
         icon = value;
      }
   }

   public int MyCount
   {
      get
      {
         return MyItems.Count;
      }
   }

   public Text MyStackText
   {
      get
      {
         return stackSize;
      }
   }

   public ObservableStack<Item> MyItems
   {

      get
      {
         return items;
      }
   }

   public bool IsFull
   {
      get
      {
         if (IsEmpty||MyCount<MyItem.MyStackSize)
         {
            return false;
         }

         return true;
      }
   }

   public Item MyItem
   {
      get
      {
         if (!IsEmpty)
         {
            return MyItems.Peek();
         }

         return null;
      }
   }

   private void Awake()
   {
      MyItems.OnPop += new UpdateStackEvent(UpdateSlot);
      MyItems.OnPush += new UpdateStackEvent(UpdateSlot);
      MyItems.OnClear += new UpdateStackEvent(UpdateSlot);
   }

   public bool AddItem(Item item)
   {
      MyItems.Push(item);
      icon.sprite = item.MyIcon;
      icon.color=Color.white;
      item.MySlots = this;
      return true;
   }

   public bool AddItems(ObservableStack<Item> newItems)
   {
      if (IsEmpty||newItems.Peek().GetType()==MyItem.GetType())
      {
         int count = newItems.Count;

         for (int i = 0; i < count; i++)
         {
            if (IsFull)
            {
               return false;
            }

            AddItem(newItems.Pop());
         }

         return true;
      }
      return false;
   }


   //removes the item from the slot
   public void RemoveItem(Item item)
   {
      if (!IsEmpty)
      {
         InventoryScript.MyInstance.OnItemCountChanged(MyItems.Pop());
      }
         
   }

   private bool MergeItems(SlotScript from)
   {
      if (IsEmpty)
      {
         return false;
      }

      if (from.MyItem.GetType()==MyItem.GetType()&& !IsFull)
      {
         //how mant free slots do we have in the stack
         int free = MyItem.MyStackSize - MyCount;

         for (int i = 0; i < free; i++)
         {
            AddItem(from.MyItems.Pop());
         }

         return true;
      }

      return false;
   }

   private void UpdateSlot()
   {
      UIManager.MyInstance.UpdateStackSize(this);
   }

   public void Clear()
   {
      int initCount = MyItems.Count;
      
      if (initCount > 0)
      {

         for (int i = 0; i < initCount; i++)

         {
            InventoryScript.MyInstance.OnItemCountChanged(MyItems.Pop());
         }
         
      }
   }


   private bool SwapItems(SlotScript from)
   {
      if (IsEmpty)
      {
         return false;
      }

      if (from.MyItem.GetType() != MyItem.GetType() || from.MyCount + MyCount > MyItem.MyStackSize)
      {
         //copy all the MyItems we need to swap from A
         ObservableStack<Item> tmpFrom = new ObservableStack<Item>(from.MyItems);
         
         //clear slot A
         from.MyItems.Clear();
         
         //all MyItems from slot B and copy them into A
         from.AddItems(MyItems);
         
         //clear B
         MyItems.Clear();
         
         //move the MyItems from ACopy to B
         AddItems(tmpFrom);

         return true;
      }

      return false;
   }

   //add an item to the slot
   public void UseItem()
   { 
      if (MyItem is IUseable)
      {
         (MyItem as IUseable).Use();
        
         
      }
      else if (MyItem is Armor)
      {
         (MyItem as Armor).Equip();
      }
   }


   public bool StackItem(Item item)
   {
      if (!IsEmpty&&item.name==MyItem.name&&MyItems.Count<MyItem.MyStackSize)
      {
         MyItems.Push(item);
         item.MySlots = this;
         return true;
      }

      return false;
   }


   public bool PutItemBack()
   {
      if (InventoryScript.MyInstance.FromSlot == this)
      {
         InventoryScript.MyInstance.FromSlot.MyIcon.color = Color.white;
         return true;
      }

      return false;
   }
   
   //when the slot is clicked
   public void OnPointerClick(PointerEventData eventData)
   {
      if (eventData.button == PointerEventData.InputButton.Left)
      {
         if (InventoryScript.MyInstance.FromSlot == null&&!IsEmpty)//if we dont have something to move
         {
            if (HandScript.MyInstance.MyMoveable!=null )
            {
               if (HandScript.MyInstance.MyMoveable is Bag)
               {
                  if (MyItem is Bag)
                  {
                     InventoryScript.MyInstance.SwapBags(HandScript.MyInstance.MyMoveable as Bag, MyItem as  Bag);
                  }
               }
               else if (HandScript.MyInstance.MyMoveable is Armor)
               {
                  if (MyItem is Armor &&(MyItem as Armor).myArmorType==(HandScript.MyInstance.MyMoveable as Armor).myArmorType)
                  {
                     (MyItem as Armor).Equip();
                  
                     HandScript.MyInstance.Drop();
                  }
               }
            }
            else
            {
               HandScript.MyInstance.TakeMoveable(MyItem as IMoveable);
               InventoryScript.MyInstance.FromSlot = this;
            }
         }
         else if (InventoryScript.MyInstance.FromSlot==null && IsEmpty )
         {
            if (HandScript.MyInstance.MyMoveable is Bag)
            {
               //dequips a bag from the inventory
               Bag bag =(Bag) HandScript.MyInstance.MyMoveable;
            
               //makes sure we can dequip it into itself and that we have enough space for the items from the dequipped bag
               if (bag.MyBagScript!=MyBag&& InventoryScript.MyInstance.MyEmptySlotCount-bag.MySlotCount>0)
               {
                  AddItem(bag);
                  bag.MyBagButton.RemoveBag();
                  HandScript.MyInstance.Drop();
                 
               }
            }

            else if (HandScript.MyInstance.MyMoveable is Armor)
            {
               Armor armor = (Armor) HandScript.MyInstance.MyMoveable;
               CharacterPanel.MyInstance.MySelectedButton.DequipArmor();
               AddItem(armor);
               HandScript.MyInstance.Drop();
            }
           
            
         }
         else if (InventoryScript.MyInstance.FromSlot != null)//if we have something to move
         {
            //we will try to different things to place the item back into the inventory
            if (PutItemBack()||MergeItems(InventoryScript.MyInstance.FromSlot)||SwapItems(InventoryScript.MyInstance.FromSlot)|| AddItems(InventoryScript.MyInstance.FromSlot.MyItems))
            {
               HandScript.MyInstance.Drop();
               InventoryScript.MyInstance.FromSlot = null;
            }
         }
      }
      if (eventData.button == PointerEventData.InputButton.Right&& HandScript.MyInstance.MyMoveable==null)//if we rightclick on the slot
      {
         UseItem();
      }
   }


   public void OnPointerEnter(PointerEventData eventData)
   {
      //we need to show tooltip
      if (!IsEmpty)
      {
         UIManager.MyInstance.ShowTooltip(new Vector2(1,0),transform.position,MyItem);
      }
    
   }

   public void OnPointerExit(PointerEventData eventData)
   {
      UIManager.MyInstance.HideTooltip();
   }
}
