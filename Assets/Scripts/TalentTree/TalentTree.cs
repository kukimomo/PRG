using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TalentTree : MonoBehaviour
{
    private int points=10;

    public int MyPoints
    {
        get
        {
            return points;
        }
        set
        {
            points = value;
            UpdateTalentPointText();
        }
    }
    
    
    [SerializeField]
    private Talent[] talents;

    [SerializeField]
    private Talent[] unlockedByDefault;

    [SerializeField]
    private Text talentPointText;
    private void Start()
    {
        ResetTalents();
    }
    
    public void TryUseTalent(Talent talent)
    {
        if (MyPoints > 0&& talent.Click())
        {
            MyPoints--;
        }

        if (MyPoints == 0)
        {
            foreach (Talent t in talents)
            {
                if (t.MyCurrentCount == 0)
                {
                    t.Lock();
                }
            }
        }
    }
    
    private void ResetTalents()
    {
        UpdateTalentPointText();
        foreach (Talent talent in talents)
        {
            talent.Lock();
        }

        foreach (Talent talent in unlockedByDefault)
        {
            talent.unLock();
        }
    }

    private void UpdateTalentPointText()
    {
        talentPointText.text = points.ToString();
    }

    
}
