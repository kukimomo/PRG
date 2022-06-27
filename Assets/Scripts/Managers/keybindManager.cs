using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class keybindManager : MonoBehaviour
{
    private static keybindManager instance;
    public static keybindManager MyInstance
    {
        get
        {
            if(instance==null)
            {
              instance=FindObjectOfType<keybindManager>();
            }
            return instance;
        }
    }

    public Dictionary<string,KeyCode> Keybinds{get;private set;}

    public Dictionary<string,KeyCode> ActionBinds{get;private set;}

    private string bindName;

    // Start is called before the first frame update
    void Start()
    {
        Keybinds=new Dictionary<string,KeyCode>();

        ActionBinds=new Dictionary<string,KeyCode>();

        BindKey("UP",KeyCode.W);
        
        BindKey("LEFT",KeyCode.A);
        
        BindKey("DOWN",KeyCode.S);
        
        BindKey("RIGHT",KeyCode.D);


       
        BindKey("ACT1",KeyCode.Z);        
        BindKey("ACT2",KeyCode.X);        
        BindKey("ACT3",KeyCode.C);
    }

    public void BindKey(string key,KeyCode keyBind)
    {
        Dictionary<string,KeyCode> currentDictionary=Keybinds;

        if(key.Contains("ACT"))
        {
           currentDictionary=ActionBinds;
        }

        if(!currentDictionary.ContainsKey(key))
        {
            currentDictionary.Add(key,keyBind);
            UIManager .MyInstance.UpdateKeyText(key,keyBind);
        }
        
        else if(currentDictionary.ContainsValue(keyBind))
        {
            string myKey=currentDictionary.FirstOrDefault(x=>x.Value==keyBind).Key;

            currentDictionary[myKey]=KeyCode.None;
            UIManager .MyInstance.UpdateKeyText(key,KeyCode.None);
        }

        currentDictionary[key]=keyBind;
        UIManager .MyInstance.UpdateKeyText(key,keyBind);
        bindName=string.Empty;
    }

    public void KeyBindOnClick(string bindName)
    {
        this.bindName = bindName;
    }

    private void OnGUI()
    {
        if (bindName != String.Empty)
        {
            Event e = Event.current;
            if (e.isKey)
            {
                BindKey(bindName,e.keyCode);
            }
        }
    }
}
