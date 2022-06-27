using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Player : Character
{  
     private static Player instance;

     public  static Player MyInstance
     {
          get
          {
               if(instance==null)
               {
                    instance=FindObjectOfType<Player>();
               }
               return instance;
          }
     }

     private List<Enemy> attackers = new List<Enemy>();

     [SerializeField]
     private Stat  mana;

     [SerializeField]
     private Stat xpStat;
     
     [SerializeField]
     private Text levelText;

     private float  initMana=50;
     //[SerializeField]
     //private GameObject[] spellPrefab;

     [SerializeField]
     private Block[] blocks;

     [SerializeField]
     private Transform[] exitPoints;

     [SerializeField]
     private Animator ding;

     [SerializeField]
     private Transform minimapIcon;
     
     private int exitIndex=2;//2 is defalut dowm
     
     private List<IInteractable> interactables=new List<IInteractable>();
     
     private Vector3 min, max;

     #region PATHFINDING
     
     private Stack<Vector3> path;
     
     private Vector3 destination;

     private Vector3 current;
     
     private Vector3 goal;

     [SerializeField]
     private AStar astar;
     
     #endregion
     
     [SerializeField]
     private GearSocket[] gearSockets;

     [SerializeField]
     private Profession profession;
     
     public Coroutine MyInitRoutine { get; set; }
     public Coroutine initroutine { get; set; }
     public int MyGold { get; set; }

     public List<IInteractable> MyInteractables
     {
          get
          {
               return interactables;
          }
          set
          {
               interactables = value;
          }
     }

     public Stat MyXp
     {
          get
          {
               return xpStat;
          }
          set

          {
               xpStat = value;
          }
     }

     public Stat MyMana
     {
          get
          {
               return mana;
          }
          set
          {
               mana = value;
          }
     }

     public List<Enemy> MyAttackers
     {
          get
          {
               return attackers;
          }
          set
          {
               attackers = value;
          }
     }
     protected override void Update()
     {
        GetInput();
        ClickToMove();
        transform.position=new Vector3(Mathf.Clamp(transform.position.x,min.x,max.x),
                                       Mathf.Clamp(transform.position.y,min.y,max.y),transform.position.z);

        base.Update();
        
     }

     protected void FixedUpdate()
     {
          Move();
     }

     public void SetDefaultValues()
     {
          MyGold = 10;
          health.Initialize(initHealth,initHealth);
          mana.Initialize(initMana,initMana);
          xpStat.Initialize(0,Mathf.Floor(100*MyLevel*Mathf.Pow(MyLevel,0.5f)));
          levelText.text = MyLevel.ToString();
     }
   
     private void GetInput()
     {
        Direction=Vector2.zero;

         if (Input.GetKeyDown(KeyCode.P))
         {
             GainXP(600);
         }
         if(Input.GetKeyDown(KeyCode.I))
         {
            health.MyCurrentValue-=10;
            mana.MyCurrentValue-=10;       
         }
         if (Input.GetKeyDown(KeyCode.O))
         {
            health.MyCurrentValue+=10;
            mana.MyCurrentValue+=10;
         }
         if(Input.GetKey(keybindManager.MyInstance.Keybinds["UP"]))//Moves up
         {
              exitIndex=0;
             Direction+=Vector2.up;
             minimapIcon.eulerAngles = new Vector3(0, 0, 0);
         }
         if(Input.GetKey(keybindManager.MyInstance.Keybinds["LEFT"]))//moves left
         {
              exitIndex=3;
             Direction+=Vector2.left;
             if (Direction.y == 0)
             {
                  minimapIcon.eulerAngles = new Vector3(0, 0, 90);
             }
             
         }
         if(Input.GetKey(keybindManager.MyInstance.Keybinds["DOWN"]))//moves down
         {
              exitIndex=2;
             Direction+=Vector2.down;
             
             minimapIcon.eulerAngles = new Vector3(0, 0, 180);
         }
         if(Input.GetKey(keybindManager.MyInstance.Keybinds["RIGHT"]))//moves right
         {
              exitIndex=1;
             Direction+=Vector2.right;
           
             if (Direction.y == 0)
             {
                  minimapIcon.eulerAngles = new Vector3(0, 0, 270);
             }
         }
         if(isMoving)
         {
             StopAction();
             StopInit();
         }
         foreach(string action in keybindManager.MyInstance.ActionBinds.Keys)
         {
              if(Input.GetKeyDown(keybindManager.MyInstance.ActionBinds[action]))
              {
                   UIManager.MyInstance.ClickActionButton(action);
              }
         }
         
    }
     
    
    public void SetLimits(Vector3 min,Vector3  max)
    {
         this.min=min;
         this.max=max;
    }


    private IEnumerator AttackRoutine(ICastable castable)
    {    
           Transform currentTarget=MyTarget;

           yield return actionRoutine = StartCoroutine(ActionRoutine(castable));
         
         
           if(currentTarget!=null&&InLineOfSight())
           {
                Spell newSpell = SpellBook.MyInstance.GetSpell(castable.MyTitle);
                
              SpellScript s=Instantiate(newSpell.MySpellPrefab,exitPoints[exitIndex].position,Quaternion.identity).GetComponent
                                                                                                              <SpellScript>();
              s.Initialize(currentTarget,newSpell.MyDamage,transform);
           }
           StopAction();//ends the attack       
    }

    private IEnumerator GatherRoutine(ICastable castable, List<Drop> items)
    {
     
         yield return actionRoutine = StartCoroutine(ActionRoutine(castable));   //this is a hardcoded cast time,for debugging
         
         
         LootWindow.MyInstance.CreatePages(items);
    }

    public IEnumerator CraftRoutine(ICastable castable)
    {
         yield return actionRoutine = StartCoroutine(ActionRoutine(castable));
         
         profession.AddItemsToInventory();
    }

    private IEnumerator ActionRoutine(ICastable castable)
    {
         SpellBook.MyInstance.Cast(castable);
         
         IsAttacking=true;//indicates if we are attacking
         MyAnimator.SetBool("attack",IsAttacking);//starts the attack animation
         foreach (GearSocket g in gearSockets)
         {
              g.MyAnimator.SetBool("attack",IsAttacking);
         }

         yield return new WaitForSeconds(castable.MyCastTime);
         
         StopAction();
    }
    public void CastSpell(ICastable castable)
    {
          Block();
          if(MyTarget!=null&&MyTarget.GetComponentInParent<Character>().ISAlive&&
             !IsAttacking&&!isMoving&&InLineOfSight()&&InRange(castable as Spell, MyTarget.transform.position))//check if we are able to attack
          {
              initroutine= StartCoroutine(AttackRoutine(castable));
          }
       
    }

    private bool InRange(Spell spell,Vector2 targetPos)
    {
         if (Vector2.Distance(targetPos, transform.position) <= spell.MyRange)
         {
              return true;
         }
         Debug.Log("u r out ot range");
         return false;
    }
    
    public void Gather(ICastable castable, List<Drop> items)
    {
         if (!IsAttacking)
         {  
              initroutine = StartCoroutine(GatherRoutine(castable, items));
         }
    }
    
    private bool InLineOfSight()
    {
         if(MyTarget!=null)
         {
          //calculate the taget'sDirection
          Vector3 targetDirection=(MyTarget.transform.position-transform.position).normalized;

          //throws a raycast in theDirection of the target
          RaycastHit2D hit=Physics2D.Raycast(transform.position,targetDirection,
                                                  Vector2.Distance(transform.position,MyTarget.transform.position),256);

          //if we didnt hit the block,then we can cast a spell
          if(hit.collider==null)
          {
               return true;
          }
         }

         //if we hit the block we cant cast a spell
          return false;
    }

    private void Block()
    {
         foreach(Block b in blocks)
         {
              b.Deactivate();
         }
         blocks[exitIndex].Activate();
    }

    

     public  void StopAction()
    {
        SpellBook.MyInstance.stopCating();//stop the spellbook from casting

        IsAttacking=false;//mare sure that we are not attacking

        MyAnimator.SetBool("attack",IsAttacking);//stops the attack animation

        foreach (GearSocket g in gearSockets)
        {
             g.MyAnimator.SetBool("attack",IsAttacking);
        }

        if(actionRoutine!=null)//check if we have a reference to an corountine
        {
            StopCoroutine(actionRoutine);
        }
    }

     private void StopInit()
     {
          if (initroutine != null)
          {
               StopCoroutine(initroutine);
          }
     }
     
     public override void HandleLayers()
     {
          base.HandleLayers();

          if (isMoving)
          {
               foreach (GearSocket g in gearSockets )
               {
                    g.SetXAndY(Direction.x,Direction.y);
               }
          }
     }

     public override void ActivateLayer(string layerName)
     {
          base.ActivateLayer(layerName);
          
          foreach (GearSocket g in gearSockets)
          {
            g.ActivateLayer(layerName);     
          }
     }

  

     public void GainXP(int xp)
     {
          xpStat.MyCurrentValue += xp;
          CombatTextManager.MyInstance.CreateText(transform.position,xp.ToString(),SCTYPE.XP,false);

          if (xpStat.MyCurrentValue >= xpStat.MyMaxValue)
          {
               StartCoroutine(Ding());
          }
     }

     public void AddAttacker(Enemy enemy)
     {
          if (!MyAttackers.Contains(enemy))
          {
               MyAttackers.Add(enemy);
          }
     }
     
     private IEnumerator Ding()
     {
          while (!xpStat.isFull)
          {
               yield return null;
          }

          MyLevel++;
          ding.SetTrigger("Ding");
          levelText.text = MyLevel.ToString();
          xpStat.MyMaxValue = 100 * MyLevel * Mathf.Pow(MyLevel, 0.5f);
          xpStat.MyMaxValue = Mathf.Floor(xpStat.MyMaxValue);
          xpStat.MyCurrentValue = xpStat.MyOverflow;//升级，多余的经验保留 和 overflow 有关
          xpStat.Reset();
          
          
          if (xpStat.MyCurrentValue >= xpStat.MyMaxValue)
          {
               StartCoroutine(Ding());
          }
     }

     public void UpdateLevel()
     {
          levelText.text = MyLevel.ToString();
     }

     public void GetPath(Vector3 goal)
     {
          path = astar.Algorithm(transform.position, goal);
          current = path.Pop();
          destination = path.Pop();
          this.goal = goal;
     }

     private void ClickToMove()
     {
          if (path != null)
          {
               transform.parent.position =
                    Vector2.MoveTowards(transform.parent.position, destination, Speed * Time.deltaTime);

               Vector3Int dest = astar.MyTilemap.WorldToCell(destination);
               Vector3Int cur = astar.MyTilemap.WorldToCell(current);
               
               float distance = Vector2.Distance(destination,transform.parent.position);

               if (cur.y > dest.y)
               {
                    Direction=Vector2.down;
               }
               else if (cur.y < dest.y)
               {
                    Direction=Vector2.up;
               }

               if (cur.y == dest.y)
               {
                    if (cur.x > dest.x)
                    {
                         Direction=Vector2.left;
                    }
                    else if (cur.x < dest.x)
                    {
                         Direction=Vector2.right;
                    }
               }
               
               if (distance <= 0f)
               {
                    if (path.Count > 0)
                    {
                         current = destination;
                         destination = path.Pop();
                    }
                    else
                    {
                         path = null;
                    }
               }
          }
     }
     
     public void Move() 
     {
          if (path == null)
          {
               if(ISAlive)
               {
                    //makes sure that the player moves
                    myRigidbody.velocity = Direction.normalized * Speed;
               } 
          }
          
        
     }
     
     public void OnTriggerEnter2D(Collider2D collision)
     {
          if (collision.tag=="Enemy"|| collision.tag=="Interactable")
          {
               IInteractable interactable = collision.GetComponent<IInteractable>();
               if (!MyInteractables.Contains(interactable))
               {
                    MyInteractables.Add(interactable);
               }
          }
     }
     
     public void OnTriggerExit2D(Collider2D collision)
     {
          if (collision.tag=="Enemy"|| collision.tag=="Interactable")
          {
               if (interactables.Count > 0)
               {
                    IInteractable interactable =
                         MyInteractables.Find(x => x == collision.GetComponent<IInteractable>());

                    if (interactable != null)
                    {
                         interactable.StopInteract();
                    }

                    MyInteractables.Remove(interactable);
               }
               
            
          }
     }
}
  