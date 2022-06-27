﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Object = System.Object;

public class SaveManager : MonoBehaviour
{
    [SerializeField]
    private Item[] items;
    
    private Chest[] chests;

    private CharButton[] equipment;

    [SerializeField]
    private ActionButton[] actionButtons;

    [SerializeField]
    private SaveGame[] saveSlots;

    [SerializeField]
    private GameObject dialogue;

    [SerializeField]
    private Text dialogueText;

    private SaveGame current;
    
    private string action;
    void Awake()
    {
        chests = FindObjectsOfType<Chest>();
        equipment = FindObjectsOfType<CharButton>();
        foreach (SaveGame saved in saveSlots)
        {
            //we need to show the saved files here
            ShowSavedFiles(saved);
        }

    }

    private void Start()
    {
        if (PlayerPrefs.HasKey("Load"))
        {
            Load(saveSlots[PlayerPrefs.GetInt("Load")]);
            PlayerPrefs.DeleteKey("Load");
        }
        else
        {
            Player.MyInstance.SetDefaultValues();
        }
    }
    
    void Update()
    {
       
    }

    public void ShowDialogue(GameObject clickButton)
    {
        action = clickButton.name;
        switch (action)
        {
            case "Load":
                //Load(clickButton.GetComponentInParent<SaveGame>());
                dialogueText.text = "Load game?";
                break;
            case "Save":
                //Save(clickButton.GetComponentInParent<SaveGame>());
                dialogueText.text = "Save Game?";
                break;
            case "Delete":
                //Delete(clickButton.GetComponentInParent<SaveGame>());
                dialogueText.text = "Delete savefile";
                break;
        }

        current = clickButton.GetComponentInParent<SaveGame>();
        dialogue.SetActive(true);
    }

    public void ExecuteAction()
    {
        switch (action)
        {
            case "Load":
                LoadScene(current);
                break;
            case "Save":
                Save(current);
                break;
            case "Delete":
                Delete(current);
                break; 
        }
        CloseDialogue();
    }

    private void LoadScene(SaveGame savedGame)
    {
        if (File.Exists(Application.persistentDataPath + "/" + savedGame.gameObject.name + ".dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + savedGame.gameObject.name + ".dat",FileMode.Open);
            SaveData data = (SaveData) bf.Deserialize(file);
            file.Close();
            
            PlayerPrefs.SetInt("Load",savedGame.MyIndex);
            SceneManager.LoadScene(data.MyScene);
        }
    }
    
    public void CloseDialogue()
    {
        dialogue.SetActive(false);
    }
    
    private void Delete(SaveGame savedGame)
    {
        File.Delete(Application.persistentDataPath+"/"+savedGame.gameObject.name+".dat");
        savedGame.HideVisuals();
    }
    
    private void ShowSavedFiles(SaveGame savedGame)
    {
        if (File.Exists(Application.persistentDataPath + "/" + savedGame.gameObject.name + ".dat"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream file = File.Open(Application.persistentDataPath + "/" + savedGame.gameObject.name + ".dat",FileMode.Open);
            SaveData data = (SaveData) bf.Deserialize(file);
            file.Close();
            savedGame.ShowInfo(data);
        }
    }
    
    public void Save(SaveGame savedGame)
    {
        try
        {
            BinaryFormatter bf = new BinaryFormatter();

            print("Application.persistentDataPath"+Application.persistentDataPath);
            FileStream file = File.Open(Application.persistentDataPath + "/" + savedGame.gameObject.name+".dat",FileMode.Create);

            SaveData data = new SaveData();

            data.MyScene = SceneManager.GetActiveScene().name;
            
            SaveEquipment(data);
            
            SaveBags(data);
            
            SavaInventory(data);
            
            SavePlayer(data);
            
            SavaChests(data);
            
            SaveActionButtons(data);
            
            SaveQuests(data);
            
            SaveQuestGivers(data);
            
            bf.Serialize(file,data);
            
            file.Close();
            print("Save");
            
            ShowSavedFiles(savedGame);
            
        }
        catch (System.Exception)
        {
            //this is for handling errors
            Delete(savedGame);
            PlayerPrefs.DeleteKey("Load");
        }
    }


    private void SavePlayer(SaveData data)
    {
        data.MyPlayerData = new PlayerData(Player.MyInstance.MyLevel,Player.MyInstance.MyXp.MyCurrentValue,
            Player.MyInstance.MyXp.MyMaxValue, Player.MyInstance.MyHealth.MyCurrentValue,
            Player.MyInstance.MyHealth.MyMaxValue,Player.MyInstance.MyMana.MyCurrentValue,
            Player.MyInstance.MyMana.MyMaxValue,Player.MyInstance.transform.position);
    }

    private void SavaChests(SaveData data)
    {
        for (int i = 0; i < chests.Length; i++)
        {
            data.MyChestData.Add(new ChestData(chests[i].name));

            foreach (Item item in chests[i].MyItems)
            {
                if (chests[i].MyItems.Count > 0)
                {
                    data.MyChestData[i].MyItems.Add(new ItemData(item.MyTitle,item.MySlots.MyItems.Count,item.MySlots.MyIndex));
                }
            }
        }
    }

    private void SaveBags(SaveData data)
    {
        for (int i = 1; i < InventoryScript.MyInstance.MyBags.Count; i++)
        {
            data.MyInventoryData.MyBags.Add(new BagData(( InventoryScript.MyInstance.MyBags[i]).MySlotCount,
                 InventoryScript.MyInstance.MyBags[i].MyBagButton.MyBagIndex));
        }
    }

    private void SaveEquipment(SaveData data)
    {
        foreach(CharButton charButton in  equipment)
        {
            if (charButton.MyEquippedArmor!=null)
            {
                data.MyEquipmentData.Add(new EquipmentData(charButton.MyEquippedArmor.MyTitle,charButton.name));
            }
        }
    }

    private void SaveActionButtons(SaveData data)
    {
        for (int i = 0; i <actionButtons.Length; i++)
        {
            if (actionButtons[i].MyUseable != null)
            {
                ActionButtonData action;
                if (actionButtons[i].MyUseable is Spell)
                {
                    action = new ActionButtonData((actionButtons[i].MyUseable as Spell).MyTitle, false, i);
                }
                else
                {
                    action = new ActionButtonData((actionButtons[i].MyUseable as Item).MyTitle, true, i);
                }
                data.MyActionButtonData.Add(action);
            }
        }
    }

    private void SavaInventory(SaveData data)
    {
        List<SlotScript> slots = InventoryScript.MyInstance.GetAllItems();
        foreach (SlotScript slot in slots)
        {
            data.MyInventoryData.MyItems.Add(new ItemData(slot.MyItem.MyTitle,slot.MyItems.Count,slot.MyIndex,slot.MyBag.MyBagIndex));
        }
    }

    private void SaveQuests(SaveData data)
    {
        foreach (Quest quest in Questlog.MyInstance.MyQuests)
        {
            data.MyQuestData.Add(new QuestData(quest.MyTitle,quest.MyDescription,quest.MyCollectObjectives,quest.MyKillObjectives,quest.MyQuestGiver.MyQuestGiverID));
        }
    }

    private void SaveQuestGivers (SaveData data)
    {
        QuestGiver[] questGivers = FindObjectsOfType<QuestGiver>();

        foreach (QuestGiver questGiver in questGivers)
        {
            data.MyQuestGiverData.Add(new QuestGiverData(questGiver.MyQuestGiverID,questGiver.MyCompletedQuests));
        }
    }
    
    private void Load(SaveGame savedGame)
    {
        try
        {
            BinaryFormatter bf = new BinaryFormatter();

            FileStream file = File.Open(Application.persistentDataPath + "/" +savedGame .gameObject.name + ".dat",FileMode.Open);

            SaveData data = (SaveData)bf.Deserialize(file);
            
            file.Close();
            
            LoadEquipment(data);
            
            LoadBags(data);
            
            LoadInventory(data);
            
            LoadPlayer(data);
            
            LoadChests(data);
            
            LoadActionButtons(data);
            
            LoadQuests(data);
            
            LoadQuestGiver(data);
            print("load");
        }
        catch (System.Exception)
        {
            //this is for handling errors
            Delete(savedGame);
            PlayerPrefs.DeleteKey("Load");
            SceneManager.LoadScene(0);
        }
    }

    private void LoadPlayer(SaveData data)
    {
        Player.MyInstance.MyLevel = data.MyPlayerData.MyLevel;
        Player.MyInstance.UpdateLevel();
        Player.MyInstance.MyHealth.Initialize(data.MyPlayerData.MyHealth,data.MyPlayerData.MyMaxHealth);
        Player.MyInstance.MyMana.Initialize(data.MyPlayerData.MyMana,data.MyPlayerData.MyMaxMana);
        Player.MyInstance.MyXp.Initialize(data.MyPlayerData.MyXp,data.MyPlayerData.MyMaxXp);
        Player.MyInstance.transform.position = new Vector2(data.MyPlayerData.MyX, data.MyPlayerData.MyY);
    }


    private void LoadChests(SaveData data)
    {
        foreach (ChestData chest in data.MyChestData)
        {
            Chest c = Instantiate(Array.Find(chests, x => x.name == chest.MyName));

            foreach (ItemData itemData in chest.MyItems)
            {
                Item item = Array.Find(items, x => x.MyTitle == itemData.MyTitle);
                item.MySlots= c.MyBag.MySlots.Find(x => x.MyIndex == itemData.MySlotIndex);
                c.MyItems.Add(item);
            }
        }
    }

    private void LoadBags(SaveData data)
    {
        foreach (BagData bagData in data.MyInventoryData.MyBags)
        {
            Bag newBag = (Bag) Instantiate(items[0]);
            
            newBag.Initialize(bagData.MySlotCount);
            
            InventoryScript.MyInstance.AddBag(newBag,bagData.MyBagIndex);
        }
    }

    private void LoadEquipment(SaveData data)
    {
        foreach (EquipmentData equipmentData in data.MyEquipmentData)
        {
            CharButton cb = Array.Find(equipment, x => x.name == equipmentData.MyType);
            
            cb.EquipArmor(Array.Find(items,x=>x.MyTitle==equipmentData.MyTitle)as  Armor);
        }
    }

    private void LoadActionButtons(SaveData data)
    {
        foreach (ActionButtonData buttonData in data.MyActionButtonData)
        {
            if (buttonData.IsItem)
            {
                actionButtons[buttonData.MyIndex].SetUseable(InventoryScript.MyInstance.GetUseable(buttonData.MyAction));
            }
            else
            {
                actionButtons[buttonData.MyIndex].SetUseable(SpellBook.MyInstance.GetSpell(buttonData.MyAction));
            }
        }
        
    }

    private void LoadInventory(SaveData data)
    {
        foreach (ItemData itemData in data.MyInventoryData.MyItems)
        {
            Item item = Instantiate(Array.Find(items, x => x.MyTitle == itemData.MyTitle));

            for (int i = 0; i < itemData.MyStackCount; i++)
            {
                InventoryScript.MyInstance.PlaceInSpecific(item,itemData.MySlotIndex,itemData.MyBagIndex);
            }
        }
    }

    private void LoadQuests(SaveData data)
    {
        QuestGiver[] questGivers = FindObjectsOfType<QuestGiver>();

        foreach (QuestData questData in data.MyQuestData)
        {
            QuestGiver qg = Array.Find(questGivers, x => x.MyQuestGiverID == questData.MyQuestGiverID);
            Quest q = Array.Find(qg.MyQuests, x => x.MyTitle == questData.MyTitle);
            q.MyQuestGiver = qg;
            q.MyKillObjectives = questData.MyKillObjectives;
            Questlog.MyInstance.AcceptQuest(q);
        }
    }

    private void LoadQuestGiver(SaveData data)
    {
        QuestGiver[] questGivers = FindObjectsOfType<QuestGiver>();

        foreach (QuestGiverData questGiverData in data.MyQuestGiverData)
        {
            QuestGiver questGiver = Array.Find(questGivers, x => x.MyQuestGiverID == questGiverData.MyQuestGiverID);
            questGiver.MyCompletedQuests = questGiverData.MyCompleteQuests;
            questGiver.UpdateQuestStatus();
        }
    }
    
    
}
