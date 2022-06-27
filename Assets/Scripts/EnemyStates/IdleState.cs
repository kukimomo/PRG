using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

class IdleState:IState
{
     private Enemy parent;
    public void Enter(Enemy parent) 
   {
     this.parent=parent;
    
     this.parent.Reset();
   }

    public void Exit() 
   {
     
   }
    public void Update()
   {
     
     //change into follow state if the player is close
    if(parent.MyTarget!=null)//if we have a target,then we need to follow iit
    {
        parent.ChangeState(new PathState());
        
    }
   }
}