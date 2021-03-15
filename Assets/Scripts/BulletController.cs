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
}
