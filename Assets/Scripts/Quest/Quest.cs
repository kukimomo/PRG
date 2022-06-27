using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Quest 
{

    [SerializeField]
    private string title;

    [SerializeField]
    private string description;

    [SerializeField]
    private CollectObjective[] collectObjectives;

    [SerializeField]
    private KillObjective[] killObjectives;
    
    [SerializeField]
    private int level;
    
    [SerializeField]
    private int xp;
    public QuestScript MyQuestScript { get; set; }
    
    public QuestGiver MyQuestGiver { get; set; }

    public string MyTitle
    {
        get
        {
            return title;
        }
        set
        {
            title = value;
        }
    }

    public string MyDescription
    {
        get
        {
            return description;
        }
        
        set
        {
            description = value;
        }
    }

    public int MyLevel
    {
        get
        {
            return level;
        }
    }

    public int MyXp
    {
        get
        {
            return xp;
        }
    }
    public CollectObjective[] MyCollectObjectives
    {
        get
        {
            return collectObjectives;
        }
    }

    public KillObjective[] MyKillObjectives
    {
        get
        {
            return killObjectives;
        }
        set
        {
            killObjectives = value;
        }
    }

    public bool IsComplete
    {
        get
        {
            foreach (Objective o in collectObjectives)
            {
                if (!o.IsComlete)
                {
                    return false;
                }
            }
            foreach (Objective o in killObjectives)
            {
                if (!o.IsComlete)
                {
                    return false;
                }
            }
            return true;
        }
    }
}


[System.Serializable]
public abstract class Objective
{
    [SerializeField]
    private int amount;

    //[SerializeField]
    private int currentAmount;

    [SerializeField]
    private string type;

    public int MyAmount
    {
        get
        {
            return amount;
        }
    }

    public int MyCurrentAmount
    {
        get
        {
            return currentAmount;
        }
        set
        {
            currentAmount = value;
        }
    }

    public string MyType
    {
        get
        {
            return type;
        }
    }

    public bool IsComlete
    {
        get
        {
            return MyCurrentAmount >= MyAmount;
        }
    }
}


[System.Serializable]
public class CollectObjective : Objective
{
    public void UpdateItemCount(Item item)
    {
        
        if (MyType.ToLower()==item.MyTitle.ToLower())
        {
            MyCurrentAmount = InventoryScript.MyInstance.GetItemCount(item.MyTitle);

            if (MyCurrentAmount <= MyAmount)
            {
                MessageFeedManager.MyInstance.WriteMessage(string.Format("{0}:{1}/{2}",item.MyTitle,MyCurrentAmount,MyAmount));
            }

            Questlog.MyInstance.UpdateSelected();
            Questlog.MyInstance.CheckCompletion();
        }
    }
    
    public void UpdateItemCount()
    {
     
        MyCurrentAmount = InventoryScript.MyInstance.GetItemCount(MyType);
       
        Questlog.MyInstance.UpdateSelected();
        Questlog.MyInstance.CheckCompletion();
        
    }

    public void Complete()
    {
        Stack<Item> items = InventoryScript.MyInstance.GetItems(MyType, MyAmount);

        foreach (Item item in items)
        {
            item.Remove();
        }
    }
}

[System.Serializable]
public class KillObjective : Objective
{
    public void UpdateKillCount(Character character)
    {
        if (MyType == character.MyType)
        {
            if (MyCurrentAmount < MyAmount)
            {
                MyCurrentAmount++;
                MessageFeedManager.MyInstance.WriteMessage(string.Format("{0}:{1}/{2}",character.MyType,MyCurrentAmount,MyAmount));
                Questlog.MyInstance.UpdateSelected();
                Questlog.MyInstance.CheckCompletion();
            }
        }
    }
}