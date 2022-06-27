using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public delegate void HealthChanged(float health);

public delegate void CharacterRemoved();

public class Enemy : Character,IInteractable
{
    
    public event HealthChanged healthChanged; 
    public event CharacterRemoved characterRemoeved;
    
  [SerializeField]
  private CanvasGroup  healthGroup;
  
  private IState currentState;

  [SerializeField]
  private LootTable lootTable;

  [SerializeField]
  private AStar astar;
  
  public float MyAttackRange{get;set;}

  public float  MyAttackTime{get;set;}

  public Vector3 MyStartPosition{get;set;}
  
  
  
  [SerializeField] private Sprite portrait;

  public Sprite MyPortrait
  {
      get { return portrait; }
  }
  
  [SerializeField]
  private float initAggroRange;
  public float MyAggroRange{get;set;}

  public bool InRange
  {
      get
      {
        return Vector2.Distance(transform.position,MyTarget.position)<MyAggroRange;
      }
      
  }

  public AStar MyAstar
  {
      get
      {
          return astar;
      }
      
  }

  protected void Awake()
    {
        health.Initialize(initHealth,initHealth);
        MyStartPosition=transform.position;
        MyAggroRange=initAggroRange;
        MyAggroRange=1;
        MyAttackRange=1;
        ChangeState(new IdleState());
    }
    protected override void Update()
    {
        if(ISAlive)
        {

          if(!IsAttacking)
          {
            MyAttackTime+=Time.deltaTime;
          }

          currentState.Update();
        }

        base.Update();
        
    }
    public  Transform Select()//when the enemy is selected
    {
        //shows the health bar
        healthGroup.alpha=1;
        return hitBox;
    }
    
    
    //when we deselect our enemy
    public  void DeSelect()
    {
        healthGroup.alpha=0;
        healthChanged -= new HealthChanged(UIManager.MyInstance.UpdateTargetFrame);
        characterRemoeved -= new CharacterRemoved(UIManager.MyInstance.HideTargetFrame);
    }
    
    public override void TakeDamage(float damage,Transform source)
    {
        if(!(currentState is EvadeState))
        {
            if (ISAlive)
            {
                SetTarget(source);
                base.TakeDamage(damage,source);

                OnHealthChanged(health.MyCurrentValue);

                if (!ISAlive)
                {
                    Player.MyInstance.MyAttackers.Remove(this);
                    Player.MyInstance.GainXP(XPManager.CalculateXP((this as Enemy)));
                }
            }
            
        }
    }

  
    public void ChangeState(IState newState) 
    {
        if(currentState!=null)
        {
            currentState.Exit();
        }

        currentState=newState;
        currentState.Enter(this);
    }

    public void SetTarget(Transform target)
    {
        if(MyTarget==null&&!(currentState is EvadeState))
        {
            float distance=Vector2.Distance(transform.position,target.position);
            MyAggroRange=initAggroRange;
            MyAggroRange+=distance;
            MyTarget=target;
        }
    }

    public void Reset()
    {
        this.MyTarget=null;
        this.MyAggroRange=initAggroRange;
        this.MyHealth.MyCurrentValue=this.MyHealth.MyMaxValue;
        OnHealthChanged(health.MyCurrentValue);
    }

    public  void Interact()
    {
        if (!ISAlive)
        { 
            //lootTable.GetLoot();
            List<Drop> drops = new List<Drop>();

            foreach (IInteractable interactable in  Player.MyInstance.MyInteractables)
            {
                if (interactable is Enemy &&!(interactable as Enemy).ISAlive)
                {
                    drops.AddRange((interactable as Enemy).lootTable.GetLoot());
                }
            }

           LootWindow.MyInstance.CreatePages(drops);
        }
    }

    public  void StopInteract()
    {
        LootWindow.MyInstance.Close();
    }
    
    public void OnHealthChanged(float health)
    {
        if (healthChanged != null)
        {
            healthChanged(health);
        }

    }
    
    public void OnCharacterRemoved()
    {
        if (characterRemoeved != null)
        {
            characterRemoeved();
        }

        Destroy(gameObject);
    }
}
