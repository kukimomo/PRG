using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public delegate void KillConfirmed(Character character);

public class GameManager : MonoBehaviour
{
    public event KillConfirmed killConfirmedEvent;

    private Camera mainCamera;
    
    private static GameManager instance;

    public static GameManager MyInstance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<GameManager>();
            }

            return instance;
        }
    }

    [SerializeField]
    private Player player;

    [SerializeField] 
    private LayerMask clickableLayer, groundLayer;
    
    private Enemy currentTarget;

    private int targetIndex;

    private HashSet<Vector3Int> blocked = new HashSet<Vector3Int>();

    public HashSet<Vector3Int> Blocked
    {
        get
        {
            return blocked;
        }
        set
        {
            blocked = value;
        }
    }
    private void Start()
    {
        mainCamera = Camera.main;
    }

    void Update()
    {
           //Ray ray=Camera.main.ScreenPointToRay(Input.mousePosition);
           //Debug.DrawRay(ray.origin,ray.direction,Color.green);
       ClickTarget();
       NextTarget();
    }

    private void ClickTarget()
    {
       if(Input.GetMouseButtonDown(0)&&!EventSystem.current.IsPointerOverGameObject())
       {       
           //make a raycast from mouse position into the game world
           RaycastHit2D hit= Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition),Vector2.zero,Mathf.Infinity,512);

           
           if(hit.collider!=null && hit.collider.tag=="Enemy")//if we hit something
           {
                DeSelectTarget();
                SelectTarget(hit.collider.GetComponent<Enemy>());
           }  
           else//deselect the target
           {
               UIManager.MyInstance.HideTargetFrame();
               DeSelectTarget();
               
               //we remove the references to the target
               currentTarget=null;
               player.MyTarget=null;
           } 
           
       }
       else if (Input.GetMouseButtonDown(1)&&!EventSystem.current.IsPointerOverGameObject())
       {
           //makes a raycast from the mouse position into the game world
           RaycastHit2D hit= Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition),Vector2.zero,Mathf.Infinity,clickableLayer);

           if (hit.collider != null)
           {
               IInteractable entity = hit.collider.gameObject.GetComponent<IInteractable>();
               if (hit.collider != null && (hit.collider.tag == "Enemy" || hit.collider.tag == "Interactable") &&
                   player.MyInteractables.Contains(entity))
               {
                   entity.Interact();
               }
           }
           else
           {
               hit= Physics2D.Raycast(mainCamera.ScreenToWorldPoint(Input.mousePosition),Vector2.zero,Mathf.Infinity,groundLayer);
               if (hit.collider != null)
               {
                   player.GetPath(mainCamera.ScreenToWorldPoint(Input.mousePosition));
               }
           }
       }
    }

    private void NextTarget()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            DeSelectTarget();

            if (Player.MyInstance.MyAttackers.Count>0)
            {
                if (targetIndex < Player.MyInstance.MyAttackers.Count)
                {
                    SelectTarget(Player.MyInstance.MyAttackers[targetIndex]);
                    targetIndex++;
                    if (targetIndex >= Player.MyInstance.MyAttackers.Count)
                    {
                        targetIndex = 0;
                    }
                }
                else
                {
                    targetIndex = 0;
                }
            }
        }
    }

    private void DeSelectTarget()
    {
        if (currentTarget != null)
        {
            currentTarget.DeSelect();
        }
    }

    private void SelectTarget(Enemy enemy)
    {
        currentTarget = enemy;
        player.MyTarget = currentTarget.Select();
        UIManager.MyInstance.ShowTargetFrame(currentTarget);
    }
    public void OnKillConfirmed(Character character)
    {
        if (killConfirmedEvent!=null)
        {
            killConfirmedEvent(character);
        }
    }
}
