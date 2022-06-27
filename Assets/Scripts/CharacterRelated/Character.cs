using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public abstract class Character : MonoBehaviour
{
    [SerializeField]
    private float speed;

    [SerializeField]
    private string type;

    [SerializeField]
    private int level;

    public Animator MyAnimator{get;set;}
    private  Vector2 direction;

    [SerializeField]
    protected Rigidbody2D myRigidbody;
   
    public  bool IsAttacking {get;set;}



    protected Coroutine actionRoutine; 

    [SerializeField]
    protected Transform hitBox;

    [SerializeField]
    protected Stat health;

    public Transform MyTarget{get;set;}

    public Stat MyHealth
    {
        get
        {
            return health;
        }
    }

    [SerializeField]
    protected float  initHealth;

    //indicates if we are moving or not
    public bool isMoving
    {
        get 
        {
           return direction.x!=0||direction.y!=0;
        }
        set 
        {

        }
    }
    public Vector2 Direction
    {
        get 
        {
            return direction;
        }
        set 
        {
            direction=value;
        }
    }
    public float Speed
    {
        get
        {
            return speed;
        }
        set
        {
            speed=value;
        }
    }

    public bool ISAlive
    {
        get
        {
            return  health.MyCurrentValue>0;
        }
    }

    public string MyType
    {
        get
        {
            return type;
        }
    }

    public int MyLevel
    {
        get
        {
            return level;
        }
        set
        {
            
            level = value;
        }
    }

    protected virtual void  Start()
    {
       
        MyAnimator=GetComponent<Animator>();
  
    }

    protected virtual void Update()
    {
        HandleLayers();
    }



   

    public virtual void HandleLayers()
    {
      if(ISAlive)
      {
        //checks if we are moving or standing still,if we are moving then need to play the move
        if(isMoving)
        {
         //animate's the Player's movement
         
          ActivateLayer("walkLayer");

          // sets the animation parameter so that he faces the correct direction 
          MyAnimator.SetFloat("X",direction.x);
          MyAnimator.SetFloat("Y",direction.y);   

       
        }
        else if(IsAttacking)
        {
            ActivateLayer("attackLayer");
        }
        else
        {
            //make sure that we will go back to  idle when we arent pressing any keys
           ActivateLayer("idleLayer");
        }
      }
      else
      {
          ActivateLayer("deathLayer");
      }
        
    }

    public virtual void ActivateLayer(string layerName)
    {
        for (int i=0;i<MyAnimator.layerCount;i++)
        {
            MyAnimator.SetLayerWeight(i,0);
        }

        MyAnimator.SetLayerWeight(MyAnimator.GetLayerIndex(layerName),1);
    }

   
    public virtual void TakeDamage(float damage,Transform source)
    {
      health.MyCurrentValue-=damage;
      CombatTextManager.MyInstance.CreateText(transform.position,damage.ToString(),SCTYPE.DAMAGE,false);
      if(health.MyCurrentValue<=0)
      {
          // makes sure that the character stops moving when its dead
          Direction=Vector2.zero;
          myRigidbody.velocity=Direction;
          GameManager.MyInstance.OnKillConfirmed(this);
          MyAnimator.SetTrigger("die");

         
      }
    }

    public void GetHealth(int health)
    {
        MyHealth.MyCurrentValue += health;
        CombatTextManager.MyInstance.CreateText(transform.position,health.ToString(),SCTYPE.HEAL,true);
    }
}
