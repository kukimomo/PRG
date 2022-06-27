using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpellBook : MonoBehaviour
{
    private static SpellBook instance;

    public  static SpellBook MyInstance
    {
        get
        {
            if(instance==null)
            {
                instance=FindObjectOfType<SpellBook>();
            }
            return instance;
        }
    }

    [SerializeField]
    private Image castingBar;
    [SerializeField]
    private Text currentSpell;
    [SerializeField]
    private Text castTime;
    [SerializeField]
    private Image icon;
    [SerializeField]
    private CanvasGroup canvasGroup;

    [SerializeField]
    private  Spell[]  spells;

    private Coroutine spellRoutine;
    private Coroutine fadeRoutine;
     void Start()
     {
       //  int a = 1;
     }
 
    void Update()
    {
        
    }

    public void Cast(ICastable castable)
    {
     
         
        //resets the fillamount on the bar
        castingBar.fillAmount=0;

        //changes the bars colors to fit the current spell
        castingBar.color=castable.MyBarColor;

         //changes the text on the bar so that we can read what spell we are casting
        currentSpell.text=castable.MyTitle;

        //changes the icon on the casting bar
        icon.sprite=castable.MyIcon;
        
        //starts casting the spell
        spellRoutine=StartCoroutine(Progress(castable));

        fadeRoutine=StartCoroutine(FadeBar());

     
    }

    private IEnumerator Progress(ICastable castable) 
    {
        float timePassed=Time.deltaTime;

        float rate=1.0f/castable.MyCastTime;

        float progress =0.0f;

        while(progress<=1.0f)
        {
            castingBar.fillAmount=Mathf.Lerp(0,1,progress);
             progress+=rate*Time.deltaTime;
             timePassed+=Time.deltaTime;
             
             castTime.text=(castable.MyCastTime-timePassed).ToString("F2");
             
             if(castable.MyCastTime-timePassed<0.09)
             {
                castTime.text="0.00";
             }
             yield return null;
        }

        stopCating();
    }
    private IEnumerator FadeBar()
    {
        float rate=1.0f/0.5f;

        float progress =0.0f;

        while(progress<=1.0f)
        {
            canvasGroup.alpha=Mathf.Lerp(0,1,progress);
             progress+=rate*Time.deltaTime;
             yield return null;
        }
    }
    public void  stopCating()
    {
        if(fadeRoutine!=null)
        {
            StopCoroutine(fadeRoutine);
            canvasGroup.alpha=0;
            fadeRoutine=null;
        }
        if(spellRoutine!=null)
        {
            StopCoroutine(spellRoutine);
            spellRoutine=null;
        }
    }
     
    public Spell GetSpell(string spellName)
    {
        Spell spell=  Array.Find(spells,x=>x.MyTitle==spellName);
        return spell;
    }
    
}
