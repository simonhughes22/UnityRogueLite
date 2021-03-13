using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{

    private Rigidbody2D rigidbody2D;

    // Start is called before the first frame update
    void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }    

    public void Launch(Vector2 direction, float force)
    {
        rigidbody2D.velocity = (direction * force);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
