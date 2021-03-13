using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum DoorType
{
    Left,
    Right,
    Top,
    Bottom
}

public class DoorController : MonoBehaviour
{
    [SerializeField]
    public DoorType doorType;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
