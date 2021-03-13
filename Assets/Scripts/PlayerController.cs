using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rigidbody2d;
    private Animator animator;
    private Vector3 change = Vector3.zero;

    // need to initialize to non zero as used to get the facing direction
    private Vector3 lastNonZeroChange = new Vector3(1,0);
    private bool firePressed = false;
    private float currentHealth;
    private System.Random random = new System.Random();

    private AudioSource audioSource;

    [SerializeField] public GameObject projectilePrefab;
    [SerializeField] public GameObject roomPrefab;

    [SerializeField] public float speed;
    [SerializeField] public float projectileSpeed = 300.0f;
    [SerializeField] public float maxHealth = 10f;

    [SerializeField] public int numberOfRooms = 10;
    [SerializeField] public float roomWidth = 11.5f;
    [SerializeField] public float roomHeight = 11.5f;

    private List<GameObject> rooms = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R)) {
            SpawnRooms(numberOfRooms);
        }

        GetInput();
        UpdateAnimation();
    }

    void FixedUpdate()
    {
        Movement();
    }

    private void GetInput()
    {
        firePressed = Input.GetKeyDown(KeyCode.Space);        
        
        change = Vector3.zero;
        change.x = Input.GetAxisRaw("Horizontal");
        // prevent moving along diagonal by forcing only one direction
        if (change.x == 0f)
        {
            change.y = Input.GetAxisRaw("Vertical");
        }       

        // keep track of direction pointed for projectile firing
        if (change != Vector3.zero) {
            lastNonZeroChange = change;            
        }
        if (firePressed)
        {            
            FireBullet();
        }
    }
   
    private void Movement()
    {
        if (change != Vector3.zero)
        {
            rigidbody2d.MovePosition(
                transform.position + change * speed * Time.deltaTime
            );
        }        
    }
    

    private void SpawnRooms(int numberOfRooms)
    {
        DestroyExistingRooms();

        HashSet<Vector2> roomPositions = new HashSet<Vector2>();
        for (int i = 0; i < numberOfRooms; i++)
        {
            Vector2 position;
            if (rooms.Count == 0)
            {
                position = rigidbody2d.position;
            }
            else
            {
                position = GetNextRoomPosition(roomPositions);
                if (position == Vector2.zero)
                {
                    // can't add any more rooms
                    break;
                }
            }
            GameObject newRoom = Instantiate(roomPrefab, position, Quaternion.identity);
            rooms.Add(newRoom);
            roomPositions.Add(position);
        }

        CloseExternalDoors(roomPositions);
    }

    private void CloseExternalDoors(HashSet<Vector2> roomPositions)
    {
        foreach (GameObject room in rooms)
        {
            Vector2 pos = room.transform.position;
            RoomController rC = room.GetComponent<RoomController>();
            List<Vector2> adjPositions = GetEmptyAdjacentRoomPositions(room, roomPositions);
            foreach (Vector2 adJpos in adjPositions)
            {
                if (adJpos.x < pos.x)
                {
                    // open to the left
                    rC.DoorLeft.SetActive(true);
                }
                else if(adJpos.x > pos.x)
                {
                    rC.DoorRight.SetActive(true);
                }
                else if (adJpos.y < pos.y)
                {
                    rC.DoorBottom.SetActive(true);
                }
                else if (adJpos.y > pos.y)
                {
                    rC.DoorTop.SetActive(true);
                }
            }
        }
    }

    private void DestroyExistingRooms()
    {
        foreach (GameObject room in rooms)
        {
            Destroy(room);
        }
        rooms.Clear();
    }

    private Vector2 GetNextRoomPosition(HashSet<Vector2> roomPositions)
    {
        // copy rooms, before shuffling
        List<GameObject> rms = rooms.ToList();
        rms.Shuffle();

        foreach (GameObject room in rms)
        {            
            List<Vector2> adjPositions = GetEmptyAdjacentRoomPositions(room, roomPositions);
            if (adjPositions.Count > 0)
            {                
                return adjPositions[random.Next(adjPositions.Count)];                
            }

        }
        return Vector2.zero;
    }

    private List<Vector2> GetEmptyAdjacentRoomPositions(GameObject room, HashSet<Vector2> roomPositions)
    {
        Vector2 lastPosition = room.transform.position;
        // pick a random direction
        Vector2[] positions = new Vector2[] {
                    // right
                    new Vector2(lastPosition.x + roomWidth, lastPosition.y),
                    // left
                    new Vector2(lastPosition.x - roomWidth, lastPosition.y),
                    // top
                    new Vector2(lastPosition.x, lastPosition.y + roomHeight),
                    // bottom
                    new Vector2(lastPosition.x, lastPosition.y - roomHeight)
                };

        List<Vector2> possiblePositions = new List<Vector2>();
        foreach (Vector2 pos in positions)
        {
            if (false == roomPositions.Contains(pos))
            {
                possiblePositions.Add(pos);
            }
        }
        return possiblePositions;
    }

    private void FireBullet()
    {
        // move the bullet to behind the barrel (need to use the change to get the direction pointing in)
        Vector2 position = new Vector2(rigidbody2d.position.x + (lastNonZeroChange.x * 0.15f), rigidbody2d.position.y + (lastNonZeroChange.y * 0.15f));        

        Quaternion rotation = Quaternion.identity;        
        if (lastNonZeroChange.x != 0f)
        {
            // need to rotate around the z axis, otherwise it moves you into the 3D plane I think
            rotation = Quaternion.Euler(0, 0, 90);
        }
        GameObject projectileObject = Instantiate(projectilePrefab, position, rotation);        
        projectileObject.GetComponent<BulletController>().Launch(lastNonZeroChange, projectileSpeed);        
    }

    private void UpdateAnimation()
    {
        if (change != Vector3.zero)
        {
            animator.SetFloat("moveX", change.x);
            animator.SetFloat("moveY", change.y);            
        }
        if (firePressed)
        {
            animator.SetTrigger("fire");
        }
    }

}
