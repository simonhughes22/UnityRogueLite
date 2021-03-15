using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGhostController : EnemyControllerBase
{
    void FixedUpdate()
    {
        Vector2 change = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        rb.MovePosition(change);
    }

    protected override void DestroyChild()
    {
        Destroy(gameObject);
    }
   
}
