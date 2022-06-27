using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stat : MonoBehaviour
{
    private Image content;

    [SerializeField]
    private Text statValue;
    
    [SerializeField]
    private float lerpSpeed;
    
    private float currentFill;

    private float overflow;//升级时多余的经验值
    
    public float MyMaxValue{get;set;}// the stat's max value for example max health or mama
    
    private float currentValue;//the currentValue for example the current health or mama

    public bool isFull
    {
        get
        {
            return content.fillAmount == 1;
        }
    }

    public float MyOverflow
    {
        get
        {
            float tmp = overflow;
            overflow = 0;
            return tmp;
        }
    }
    
    public float MyCurrentValue
    {
        get
        {
            return currentValue;
        }
        set
        {
            if(value>MyMaxValue)//makes sure that we dont get too much health
            {
                overflow = value - MyMaxValue;
                currentValue=MyMaxValue;
            }
            else if(value<0)//makes sure that we dont get health below 0
            {
                currentValue=0;
            }
            else   // makes sure that we set the current value withing the bounds of 0
            {
                currentValue=value;
            }
           //calculate the currentFill,so that we can lerp
           currentFill=(float)currentValue/(float)MyMaxValue;
        
            if(statValue!=null)
            {

                //makes sure that we update thee value text
                statValue.text=currentValue+"/"+MyMaxValue;
            }

        }
    }

    void Start()
    {
       // MyMaxValue=100;
        content=GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
      // content.fillAmount=currentFill;
      HandleBar();
    }

    public void Initialize(float currentValue,float maxVaue)
    {
        if(content==null)
        {
            content=GetComponent<Image>();
        }
        MyMaxValue=maxVaue;
        MyCurrentValue=currentValue;
        content.fillAmount=MyCurrentValue/MyMaxValue;
    }
    public void HandleBar()
    {
        if(currentFill!=content.fillAmount)
        {
            content.fillAmount=Mathf.MoveTowards(content.fillAmount,currentFill,Time.deltaTime*lerpSpeed);        
        }
    }

    public void Reset()
    {
        content.fillAmount = 0;
    }
}
