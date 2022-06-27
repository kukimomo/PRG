using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearSocket : MonoBehaviour
{
    public Animator MyAnimator { get; set; }

    protected SpriteRenderer spriteRenderer;

    private Animator parentAnimator;

    private AnimatorOverrideController animatorOverrideController;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        parentAnimator = GetComponentInParent<Animator>();
        MyAnimator = GetComponent<Animator>();

        animatorOverrideController = new AnimatorOverrideController(MyAnimator.runtimeAnimatorController);

        MyAnimator.runtimeAnimatorController = animatorOverrideController;
    }
    
    public virtual void SetXAndY(float x,float y)
    {
        // sets the animation parameter so that he faces the correct direction 
        MyAnimator.SetFloat("X",x);
        MyAnimator.SetFloat("Y",y);   

    }

    public void ActivateLayer(string layerName)
    {
        for (int i=0;i<MyAnimator.layerCount;i++)
        {
            MyAnimator.SetLayerWeight(i,0);
        }

        MyAnimator.SetLayerWeight(MyAnimator.GetLayerIndex(layerName),1);
    }

    public void Equip(AnimationClip[] animations)
    {
        spriteRenderer.color=Color.white;
        animatorOverrideController["attack_back"] = animations[0];
        animatorOverrideController["attack_front"] = animations[1];
        animatorOverrideController["attack_left"] = animations[2];
        animatorOverrideController["attack_right"] = animations[3];
        
        animatorOverrideController["idle_back"] = animations[4];
        animatorOverrideController["idle_front"] = animations[5];
        animatorOverrideController["idle_left"] = animations[6];
        animatorOverrideController["idle_right"] = animations[7];
        
        animatorOverrideController["walk_back"] = animations[8];
        animatorOverrideController["walk_front"] = animations[9];
        animatorOverrideController["walk_left"] = animations[10];
        animatorOverrideController["walk_right"] = animations[11];
    }

    public void Dequip()
    {
        animatorOverrideController["attack_back"] = null;
        animatorOverrideController["attack_front"] = null;
        animatorOverrideController["attack_left"] = null;
        animatorOverrideController["attack_right"] = null;
        
        animatorOverrideController["idle_back"] = null;
        animatorOverrideController["idle_front"] = null;
        animatorOverrideController["idle_left"] = null;
        animatorOverrideController["idle_right"] = null;
        
        animatorOverrideController["walk_back"] = null;
        animatorOverrideController["walk_front"] = null;
        animatorOverrideController["walk_left"] = null;
        animatorOverrideController["walk_right"] = null;

        Color c = spriteRenderer.color;
        c.a = 0;
        spriteRenderer.color = c;
    }
}
