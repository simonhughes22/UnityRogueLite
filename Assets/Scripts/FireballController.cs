using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballController : ProjectileController
{
    protected Animator anim;    

    protected override void Awake()
    {
        base.Awake();
        anim = GetComponent<Animator>();
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Door")
            | collision.gameObject.CompareTag("Walls")
            | collision.gameObject.CompareTag("Enemy"))
        {
            base.rigidbody2D.velocity = Vector2.zero;
            base.rigidbody2D.simulated = false;
            anim.SetTrigger("dead");
            Destroy(gameObject, 1.1f);
        }        
    }
}
