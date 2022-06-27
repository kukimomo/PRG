using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellScript : MonoBehaviour
{
    private Rigidbody2D myRigidbody;
    [SerializeField]

    private float speed;

    private Transform target;

    public Transform MyTarget{get;private set;}

    [SerializeField]
    private float damage;

    private Transform source;
    void Start()
    {
        myRigidbody=GetComponent<Rigidbody2D>();
       
    }
    public void  Initialize(Transform target,float damage,Transform source)
    {
       this.MyTarget=target;
       this.damage=damage;
       this.source=source;
    }

    private void FixedUpdate() 
    {
        if(MyTarget!=null)
       {

         Vector2 direction=MyTarget.position-transform.position;
         myRigidbody.velocity=direction.normalized*speed;
         float angle=Mathf.Atan2(direction.y,direction.x)*Mathf.Rad2Deg;
         transform.rotation=Quaternion.AngleAxis(angle,Vector3.forward);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag=="HitBox"&&collision.transform==MyTarget)
        {
            Character c= collision.GetComponentInParent<Enemy>();
            speed=0;
            c.TakeDamage(damage,source);
            GetComponent<Animator>().SetTrigger("impact");
            myRigidbody.velocity=Vector2.zero;
            MyTarget=null;
        }
    }
}
