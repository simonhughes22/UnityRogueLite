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
    private System.Random random = new System.Random(1234);

    private AudioSource audioSource;

    [SerializeField] public GameObject projectilePrefab;
    [SerializeField] public GameObject roomPrefab;

    [SerializeField] public float speed;
    [SerializeField] public float projectileSpeed = 300.0f;
    [SerializeField] public float maxHealth = 10f;

    [SerializeField] public int numberOfRooms = 10;
    [SerializeField] public float roomWidth = 16f;
    [SerializeField] public float roomHeight = 8.5f;

    private List<GameObject> rooms = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        rigidbody2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        currentHealth = maxHealth;
        audioSource = GetComponent<AudioSource>();

        SpawnRooms(numberOfRooms);
        // start camera on player
        Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
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

    private Tuple<int,int> roomToOffset(GameObject room)
    {
        Vector3 pos = room.transform.position;
        Vector3 diff = pos - transform.position;
        return new Tuple<int, int>((int)(diff.x / roomWidth), (int)(diff.y / roomHeight));
    }

    private void SpawnRooms(int numberOfRooms)
    {
        DestroyExistingRooms();
        HashSet<Tuple<int, int>> roomPositions = new HashSet<Tuple<int, int>>();
        for (int i = 0; i < numberOfRooms; i++)
        {
            Vector2 position;
            if (rooms.Count == 0)
            {
                position = rigidbody2d.position;
                roomPositions.Add(new Tuple<int, int>(0, 0));
            }
            else
            {
                Tuple<int, int> offset = GetNextRoomPosition(roomPositions);
                if (offset == null)
                {   // can't add any more rooms
                    break;
                }
                position = new Vector2(transform.position.x + offset.Item1 * roomWidth, transform.position.y + offset.Item2 * roomHeight);
                roomPositions.Add(offset);
            }
            GameObject newRoom = Instantiate(roomPrefab, position, Quaternion.identity);
            rooms.Add(newRoom);            
        }

        CloseExternalDoors(roomPositions);        
    }

    private void CloseExternalDoors(HashSet<Tuple<int, int>> roomPositions)
    {
        foreach (GameObject room in rooms)
        {
            Tuple<int, int> pos = roomToOffset(room);
            RoomController rC = room.GetComponent<RoomController>();
            List<Tuple<int, int>> adjPositions = GetEmptyAdjacentRoomPositions(room, roomPositions);
            foreach (Tuple<int, int> adJpos in adjPositions)
            {
                if (adJpos.Item1 < pos.Item1)
                {
                    // open to the left
                    rC.DoorLeft.SetActive(true);
                }
                else if(adJpos.Item1 > pos.Item1)
                {
                    rC.DoorRight.SetActive(true);
                }
                else if (adJpos.Item2 < pos.Item2)
                {
                    rC.DoorBottom.SetActive(true);
                }
                else if (adJpos.Item2 > pos.Item2)
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

    private Tuple<int, int> GetNextRoomPosition(HashSet<Tuple<int, int>> roomPositions)
    {
        // copy rooms, before shuffling
        List<GameObject> rms = rooms.ToList();
        rms.Shuffle();

        foreach (GameObject room in rms)
        {            
            List<Tuple<int, int>> adjPositions = GetEmptyAdjacentRoomPositions(room, roomPositions);
            if (adjPositions.Count > 0)
            {                
                return adjPositions[random.Next(adjPositions.Count)];                
            }

        }
        return null;
    }

    private List<Tuple<int,int>> GetEmptyAdjacentRoomPositions(GameObject room, HashSet<Tuple<int, int>> roomPositions)
    {
        Tuple<int, int> rPos = roomToOffset(room);
        // pick a random direction
        Tuple<int,int>[] positions = new Tuple<int, int>[] {
                    // right
                    new Tuple<int,int>( 1 + rPos.Item1,  rPos.Item2),
                    // left
                    new Tuple<int,int>(-1 + rPos.Item1, rPos.Item2),
                    // top
                    new Tuple<int,int>(rPos.Item1,  1 + rPos.Item2),
                    // bottom
                    new Tuple<int,int>(rPos.Item1, -1 + rPos.Item2),
                };

        List<Tuple<int, int>> possiblePositions = new List<Tuple<int, int>>();
        foreach (Tuple<int, int> pos in positions)
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
