using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public enum SCTYPE{DAMAGE,HEAL,XP}

public class CombatTextManager : MonoBehaviour
{
    private static CombatTextManager instance;

    public static CombatTextManager MyInstance
    {
        get
        {
            if (instance==null)
            {
                instance = FindObjectOfType<CombatTextManager>();
            }

            return instance;
        }
    }

    [SerializeField]
    private GameObject combatTextPrefab;

    public void CreateText(Vector2 position,string text,SCTYPE type,bool crit)
    { 
        position.y += 0.8f;//offset
       Text sct= Instantiate(combatTextPrefab, transform).GetComponent<Text>();
       sct.transform.position = position;
       
       string before=String.Empty;
       string after=String.Empty;
       switch (type)
       {
           case SCTYPE.DAMAGE:
               before = "-";
               sct.color=Color.red;
               break;
           case SCTYPE.HEAL:
               before = "+";
               sct.color=Color.green;
               break;
           case SCTYPE.XP:
               before = "+";
               after = "  XP";
               sct.color=Color.yellow;
               break;
       }

       sct.text = before + text + after;

       if (crit)
       {
           sct.GetComponent<Animator>().SetBool("Crit",crit);
       }
    }
}
