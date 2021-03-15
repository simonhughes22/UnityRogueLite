using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyControllerBase : MonoBehaviour
{
    protected Rigidbody2D rb;
    protected Animator anim;

    protected float currentHealth;
    private AudioSource audioSource;

    [SerializeField] public float maxHealth = 1f;
    [SerializeField] public float speed = 3f;
    [SerializeField] public GameObject deathEffectPrefab;    
    [SerializeField] public AudioClip deathClip;
    // Start is called before the first frame update

    protected GameObject player;

    protected void Start()
    {
        currentHealth = maxHealth;
        player = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = player.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    protected void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    // Calling destroy on the parent does not destroy the child
    protected virtual void DestroyChild()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Projectile"))
        {
            currentHealth -= 1f;
        }
        if (currentHealth <= 0.0f)
        {
            PlaySound(deathClip);
            // create blooder splatter, or whatever effect is desired
            GameObject particleEffect = Instantiate(deathEffectPrefab, transform.position, Quaternion.identity);
            Destroy(particleEffect, 3f);
            DestroyChild();
        }
    }
}
