using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : IState
{
    private Enemy parent;
    private float attackCooldown=3;

    private float extraRange=0.1f;
    public void Enter(Enemy parent)
    {
        this.parent=parent;
    }

    public void Exit() 
    {

    }

    public void Update()
    {
//       Debug.Log("attacking");

       if(parent.MyAttackTime>=attackCooldown&&!parent.IsAttacking)
       {
         parent.MyAttackTime=0;

         parent.StartCoroutine(Attack());
       }


      if(parent.MyTarget!=null)
      {
          //calculates the distance between the target and the enemy
          float distance=Vector2.Distance(parent.MyTarget.position,parent.transform.position);

          if(distance>=parent.MyAttackRange+extraRange&&!parent.IsAttacking)
          {
              //follows the target
              parent.ChangeState(new FollowState());
          }
          //we need to check range and attack

      }
      else//if we lost the target then we need to idle
      {
          parent.ChangeState(new IdleState());
      }
    }

    public IEnumerator Attack()
    {
        parent.IsAttacking=true;

        parent.MyAnimator.SetTrigger("attack");

        yield return new WaitForSeconds(parent.MyAnimator.GetCurrentAnimatorStateInfo(2).length);

        parent.IsAttacking=false;
    }

}
