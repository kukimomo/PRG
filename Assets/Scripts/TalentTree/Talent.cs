using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Talent : MonoBehaviour
{
    private Image sprite;

    [SerializeField]
    private Text countText;

    [SerializeField]
    private int maxCount;
    
    private int currentCount;

    public int MyCurrentCount
    {
        get
        {
            return currentCount;
        }
        set
        {
            currentCount = value;
        }
    }
    
    private bool unlocked;

    [SerializeField]
    private Talent childTalent;

    [SerializeField]
    private Sprite arrowSpriteLocked;

    [SerializeField]
    private Sprite arrowSpriteUnLocked;
    
    [SerializeField]
    private Image arrowImage;
    
    private void Awake()
    {
        sprite = GetComponent<Image>();
        
        countText.text = $"{currentCount}/{maxCount}";
        
        if (unlocked)
        {
            unLock();
        }
    }

    public virtual bool Click()
    {
        if (currentCount < maxCount&&unlocked)
        {
            currentCount++;
            countText.text = $"{currentCount}/{maxCount}";

            if (currentCount == maxCount)
            {
                if (childTalent != null)
                {
                    childTalent.unLock();
                }
            }
            
            return true;
        }
        return false;
    }
    
    public void Lock()
    {
        sprite.color = Color.grey;
        countText.color = Color.grey;
        
        
        if (arrowImage != null)
        {
            arrowImage.sprite = arrowSpriteLocked;
        }

        if (countText != null)
        {
            countText.color = Color.gray;
        }
    }

    public void unLock()
    {
        sprite.color = Color.white;
        countText.color = Color.white;

        if (arrowImage != null)
        {
            arrowImage.sprite = arrowSpriteUnLocked;
        }

        if (countText != null)
        {
            countText.color = Color.white;
        }
        
        unlocked = true;
    }
}
