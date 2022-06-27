using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class HandScript : MonoBehaviour
{
    private static HandScript instance;

    public  static HandScript MyInstance
    {
        get
        {
            if(instance==null)
            {
                instance=FindObjectOfType<HandScript>();
            }
            return instance;
        }
    }
    
    public IMoveable MyMoveable { get; set; }

    private Image icon;
    
    [SerializeField]
    private Vector3 offset;
    void Start()
    {
        //creates a reference to the image on the hand
        icon = GetComponent<Image>();
    }

    
    void Update()
    {
        //makes sure that the icon follows the hand
        icon.transform.position = Input.mousePosition+offset;
        
        if (Input.GetMouseButton(0) && !EventSystem.current.IsPointerOverGameObject() && MyInstance.MyMoveable != null)
        {
            DeleteItem();
        }
    }

    public void TakeMoveable(IMoveable moveable)
    {
        this.MyMoveable = moveable;
        icon.sprite = moveable.MyIcon;
        icon.color=Color.white;
    }

    public IMoveable Put()
    {
        IMoveable tmp = MyMoveable;

        MyMoveable = null;
        icon.color = new Color(0, 0, 0,0);
        return tmp;
    }

    public void Drop()
    {
        MyMoveable = null;
        icon.color = new Color(0, 0, 0, 0);
        InventoryScript.MyInstance.FromSlot = null;
    }

    public  void DeleteItem()
    {
        if (MyMoveable is Item )
        {
            Item item = (Item) MyMoveable;
            if (item.MySlots!=null)
            {
                item.MySlots.Clear();
            }
            else if (item.MyCharButton!=null)
            {
                item.MyCharButton.DequipArmor();
            }
        }

        Drop();

        InventoryScript.MyInstance.FromSlot = null;
    }
}
