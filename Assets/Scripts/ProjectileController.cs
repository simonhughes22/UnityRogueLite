using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileController : MonoBehaviour
{
    protected Rigidbody2D rigidbody2D;

    // Start is called before the first frame update
    protected virtual void Awake()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }    

    public void Launch(Vector2 direction, float force, float scaleMultipler = 1.0f)
    {
        rigidbody2D.velocity = (direction * force);
        Vector3 sc = transform.localScale;
        transform.localScale = new Vector3(sc.x * scaleMultipler, sc.y * scaleMultipler, 1.0f);
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Door") | collision.gameObject.CompareTag("Walls"))
        {
            Destroy(gameObject);
        }
    }
}
