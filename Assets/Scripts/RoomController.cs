using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{

    [SerializeField] public GameObject DoorLeft;
    [SerializeField] public GameObject DoorRight;
    [SerializeField] public GameObject DoorTop;
    [SerializeField] public GameObject DoorBottom;   

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {            
            Camera.main.transform.position = new Vector3(transform.position.x, transform.position.y, Camera.main.transform.position.z);
        }
    }
}
