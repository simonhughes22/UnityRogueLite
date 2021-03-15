using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;
    private AudioSource audioSource;    

    // need to initialize to non zero as used to get the facing direction
    private Vector3 lookDirection = new Vector3(1,0);
    private Vector3 change = Vector3.zero;

    private bool firePressed = false;            
    private bool moving = false;
    private bool facingForward = true;    

    [SerializeField] public GameObject projectilePrefab;
    [SerializeField] public GameObject roomPrefab;

    [SerializeField] public AudioClip zapSound;
    [SerializeField] public AudioClip playerHitSound;

    [SerializeField] public float speed = 6f;
    [SerializeField] public float projectileSpeed = 10.0f;
    [SerializeField] public float projectileScaleMultiplier = 2.0f;
    
    [SerializeField] public int numberOfRooms = 10;
    [SerializeField] public float roomWidth = 16f;
    [SerializeField] public float roomHeight = 8.5f;

    private RoomCreator roomCreator;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        
        // start camera on player
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
        roomCreator = new RoomCreator(this.gameObject, this.roomPrefab, this.roomWidth, this.roomHeight);
        roomCreator.SpawnRooms(numberOfRooms);
    }

    public GameObject CreateRoom(Vector2 position)
    {
        return Instantiate(roomPrefab, position, Quaternion.identity);
    }

    public void DestroyRooms(List<GameObject> rooms)
    {
        foreach (GameObject room in rooms)
        {
            Destroy(room);
        }
        rooms.Clear();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) {
            roomCreator.SpawnRooms(numberOfRooms);
        }

        GetInput();
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        Movement();
    }
  
    private void Movement()
    {
        Vector3 scale = transform.localScale;
        if (facingForward)
        {
            transform.localScale = new Vector3(Math.Abs(scale.x), scale.y, scale.z);
        }
        else
        {
            transform.localScale = new Vector3(-1.0f * Math.Abs(scale.x), scale.y, scale.z);
        }
        
        if (change != Vector3.zero)
        {
            rb.MovePosition(
                transform.position + change * speed * Time.deltaTime
            );
        }        
    }

    private void GetInput()
    {
        moving = false;
        firePressed = Input.GetKeyDown(KeyCode.Space);

        change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal");
        // prevent moving along diagonal by forcing only one direction
        if (change.x == 0f)
        {
            change.y = Input.GetAxisRaw("Vertical");
        }

        if (change.x > 0f)
        {
            facingForward = true;
        }
        else if (change.x < 0f)
        {
            facingForward = false;
        }

        // keep track of direction pointed for projectile firing
        if (change != Vector3.zero)
        {
            lookDirection = change;
            moving = true;
        }
        if (firePressed)
        {
            FireBullet();
        }
    }

    private void FireBullet()
    {
        // move the bullet to behind the barrel (need to use the change to get the direction pointing in)
        Vector2 position = new Vector2(rb.position.x + (lookDirection.x * 0.15f), rb.position.y + (lookDirection.y * 0.15f));        

        Quaternion rotation = Quaternion.identity;        
        if (lookDirection.x != 0f)
        {
            // need to rotate around the z axis, otherwise it moves you into the 3D plane I think
            rotation = Quaternion.Euler(0, 0, 90);
        }
        GameObject projectileObject = Instantiate(projectilePrefab, position, rotation);        
        projectileObject.GetComponent<BulletController>().Launch(lookDirection, projectileSpeed, projectileScaleMultiplier);

        // sounds
        PlaySound(zapSound);
    }

    private void UpdateAnimation()
    {
        anim.SetBool("moving", moving);        
    }

    // Update is called once per frame
    protected void PlaySound(AudioClip clip)
    {
        audioSource.PlayOneShot(clip);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            GameState.Instance.PlayerHealth -= 1;
            PlaySound(playerHitSound);

            // knockback - use move towards with a negative amount
            Vector2 change = Vector2.MoveTowards(transform.position, collision.gameObject.transform.position, -50f * Time.deltaTime);
            transform.position = change;
        }

        if (GameState.Instance.PlayerHealth <= 0)
        {
            SceneManager.LoadScene("GameOver", LoadSceneMode.Single);
        }
    }
}
