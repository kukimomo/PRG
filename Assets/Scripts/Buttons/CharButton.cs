using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CharButton : MonoBehaviour,IPointerClickHandler,IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField]
    private ArmorType armorType;

    private Armor equippedArmor;

    [SerializeField]
    private Image icon;

    [SerializeField]
    private GearSocket gearSocket;

    public Armor MyEquippedArmor
    {
        get
        {
            return equippedArmor;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (HandScript.MyInstance.MyMoveable is Armor)
            {
                Armor tmp =(Armor) HandScript.MyInstance.MyMoveable;
                    
                if (tmp.myArmorType==armorType)
                {
                    EquipArmor(tmp);
                }
                
                UIManager.MyInstance.RefreshTooltip(tmp);
            }
            else if(HandScript.MyInstance.MyMoveable==null&& equippedArmor!=null)// 将装备重新放回背包
            {
                HandScript.MyInstance.TakeMoveable(equippedArmor);
                CharacterPanel.MyInstance.MySelectedButton = this;
                icon.color=Color.grey;
            }
        }
    }

    public void EquipArmor(Armor armor)
    {
        armor.Remove();

        if (equippedArmor != null)
        {
            if (equippedArmor != armor)
            {
                armor.MySlots.AddItem(equippedArmor);
            }
            UIManager.MyInstance.RefreshTooltip(equippedArmor);
        }
        else
        {
            UIManager.MyInstance.HideTooltip();
        }
        
        icon.enabled = true;
        icon.sprite = armor.MyIcon;
        icon.color=Color.white;
        this.equippedArmor = armor;//a reference to the equipped equipped Armor
        this.equippedArmor.MyCharButton = this;

        if (HandScript.MyInstance.MyMoveable == (armor as IMoveable))
        {
            HandScript.MyInstance.Drop();
        }

        if (gearSocket != null && equippedArmor.MyAnimationlips!= null)
        {
            gearSocket.Equip(equippedArmor.MyAnimationlips);
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (equippedArmor!=null)
        {
            UIManager.MyInstance.ShowTooltip(new Vector2(0,0),transform.position,equippedArmor);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UIManager.MyInstance.HideTooltip();
    }

    public void DequipArmor()
    {
        icon.color=Color.white;
        icon.enabled = false;
        //equippedArmor = null;
        if (gearSocket != null && equippedArmor.MyAnimationlips!= null)
        {
            gearSocket.Dequip();
        }

        equippedArmor.MyCharButton = null;
        equippedArmor = null;
    }
}
