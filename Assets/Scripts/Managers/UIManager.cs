using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class UIManager : MonoBehaviour
{
    private static UIManager instance;

    public  static UIManager MyInstance
    {
        get
        {
            if(instance==null)
            {
                instance=FindObjectOfType<UIManager>();
            }
            return instance;
        }
    }

    [SerializeField]
    private ActionButton[] actionButtons;

    [SerializeField]
    private CanvasGroup[] menus;
    
    [SerializeField]
    private GameObject targetFrame;

    private Stat healthStat;

    [SerializeField]
    private Text levelText;

    [SerializeField]
    private Image portraitFrame;

    [SerializeField]
    private GameObject tooltip;

    private Text tooltipText;

    [SerializeField]
    private RectTransform tooltipRect;

    [SerializeField]
    private CharacterPanel charPanel;
    
    [SerializeField]
    private CanvasGroup  keybindMenu;

    [SerializeField]
    private CanvasGroup spellBook;

    private GameObject[] keybindButtons;

    private void Awake()
    {
        keybindButtons=GameObject.FindGameObjectsWithTag("Keybind");
        tooltipText = tooltip.GetComponentInChildren<Text>();
    }


    void Start()
    {
        healthStat=targetFrame.GetComponentInChildren<Stat>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            OpenClose(menus[0]);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            OpenClose(menus[1]);
        }
        
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
             InventoryScript.MyInstance.OpenClose();
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            OpenClose(menus[2]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            OpenClose(menus[3]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha6))
        {
            OpenClose(menus[6]);
        }
        if (Input.GetKeyDown(KeyCode.Alpha7))
        {
            OpenClose(menus[7]);
        }
        // if(Input.GetKeyDown(KeyCode.Alpha1))
        // {
        //     OpenClose(keybindMenu);
        // }
        //
        // if (Input.GetKeyDown(KeyCode.Alpha2))
        // {
        //     OpenClose(spellBook);
        // }
        //
        // if (Input.GetKeyDown(KeyCode.Alpha3))
        // {
        //     InventoryScript.MyInstance.OpenClose();
        // }
        //
        // if (Input.GetKeyDown(KeyCode.Alpha4))
        // {
        //     charPanel.OpenClose();
        // }
    }
    public void ShowTargetFrame(Enemy target)
    {
        targetFrame.SetActive(true);
        
        healthStat.Initialize(target.MyHealth.MyCurrentValue,target.MyHealth.MyMaxValue);
        
        portraitFrame.sprite=target.MyPortrait;

        levelText.text = target.MyLevel.ToString();

        target.healthChanged+=new HealthChanged(UpdateTargetFrame);

        target.characterRemoeved+=new CharacterRemoved(HideTargetFrame);

        //敌人的颜色等级
        if (target.MyLevel>=Player.MyInstance.MyLevel+5)
        {
            levelText.color=Color.red;
        }
        else if (target.MyLevel == Player.MyInstance.MyLevel + 3 || target.MyLevel == Player.MyInstance.MyLevel + 4)
        {
            levelText.color = new Color(255, 97, 0, 255);
        }
        else if (target.MyLevel >= Player.MyInstance.MyLevel - 2 && target.MyLevel <= Player.MyInstance.MyLevel + 2)
        {
            levelText.color=Color.yellow;
        }
        else if (target.MyLevel <= Player.MyInstance.MyLevel - 3 && target.MyLevel >= XPManager.CalculateGrayLevel())
        {
            levelText.color=Color.green;
        }
        else
        {
            levelText.color=Color.grey;
        }
    }
    public void HideTargetFrame()
    {
        targetFrame.SetActive(false);
    }

    public void UpdateTargetFrame(float health)
    {
        healthStat.MyCurrentValue=health;
    }

 

    public void UpdateKeyText(string key,KeyCode code)
    {
       Text tmp= Array.Find(keybindButtons,x=>x.name==key).GetComponentInChildren<Text>();
       tmp.text=code.ToString();
    }

    public void ClickActionButton(string buttonName)
    {
        Array.Find(actionButtons,x=>x.gameObject.name==buttonName).MyButton.onClick.Invoke();
    }

    

    public void OpenClose(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha=canvasGroup.alpha>0?0:1;
        canvasGroup.blocksRaycasts=canvasGroup.blocksRaycasts==true?false:true;
    }

    public void OpenSingle(CanvasGroup canvasGroup)
    {
        foreach (CanvasGroup canvas in menus)
        {
            CloseSingle(canvas);
        }

        canvasGroup.alpha = canvasGroup.alpha > 0 ? 0 : 1;
        canvasGroup.blocksRaycasts = canvasGroup.blocksRaycasts == true ? false : true;
    }

    public void CloseSingle(CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }
    
    public void UpdateStackSize(IClickable clickable)
    {
        if (clickable.MyCount>1)
        {
            clickable.MyStackText.text = clickable.MyCount.ToString();
            clickable.MyStackText.color=Color.white;
            clickable.MyIcon.color=Color.white;
        }
        else
        {
            clickable.MyStackText.color = new Color(0, 0, 0, 0);
            clickable.MyIcon.color=Color.white;
        }
        if (clickable.MyCount == 0)//if the slot is empty,then we need to hide the icon
        {
            clickable.MyIcon.color = new Color(0, 0, 0, 0);
            clickable.MyStackText.color = new Color(0, 0, 0, 0);
        }
    }

    public void ClearStackCount(IClickable clickable)
    {
        clickable.MyStackText.color = new Color(0, 0, 0, 0);
        clickable.MyIcon.color=Color.white;
    }

    public void ShowTooltip(Vector2 pivot,Vector3 position,IDescribable description)
    {
        tooltipRect.pivot = pivot;
        tooltip.SetActive(true);
        tooltip.transform.position = position;
        tooltipText.text = description.GetDescription();
    }

    public void HideTooltip()
    {
        tooltip.SetActive(false);
    }

    public void RefreshTooltip(IDescribable description)
    {
        tooltipText.text = description.GetDescription();
    }
}