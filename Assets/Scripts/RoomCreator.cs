using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RoomCreator
{
    private GameObject player;
    private PlayerController playerController;
    public float roomWidth;
    public float roomHeight;
    private List<GameObject> rooms = new List<GameObject>();
    private GameObject roomPrefab;
    private System.Random random = new System.Random(1234);

    public RoomCreator(GameObject player, GameObject roomPrefab, float roomWidth, float roomHeight)
    {
        this.player = player;
        this.playerController = player.GetComponent<PlayerController>();
        this.roomWidth = roomWidth;
        this.roomHeight = roomHeight;
        this.roomPrefab = roomPrefab;
    }

    private Tuple<int, int> roomToOffset(GameObject room)
    {
        Vector3 pos = room.transform.position;
        Vector3 diff = pos - player.transform.position;
        return new Tuple<int, int>((int)(diff.x / roomWidth), (int)(diff.y / roomHeight));
    }

    public void SpawnRooms(int numberOfRooms)
    {
        DestroyExistingRooms();
        HashSet<Tuple<int, int>> roomPositions = new HashSet<Tuple<int, int>>();
        for (int i = 0; i < numberOfRooms; i++)
        {
            Vector2 position;
            if (rooms.Count == 0)
            {
                position = player.transform.position;
                roomPositions.Add(new Tuple<int, int>(0, 0));
            }
            else
            {
                Tuple<int, int> offset = GetNextRoomPosition(roomPositions);
                if (offset == null)
                {   // can't add any more rooms
                    break;
                }
                position = new Vector2(player.transform.position.x + offset.Item1 * roomWidth, player.transform.position.y + offset.Item2 * roomHeight);
                roomPositions.Add(offset);
            }

            GameObject newRoom = playerController.CreateRoom(position);
            rooms.Add(newRoom);
        }

        CloseExternalDoors(roomPositions);
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

    private List<Tuple<int, int>> GetEmptyAdjacentRoomPositions(GameObject room, HashSet<Tuple<int, int>> roomPositions)
    {
        Tuple<int, int> rPos = roomToOffset(room);
        // pick a random direction
        Tuple<int, int>[] positions = new Tuple<int, int>[] {
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


    public void CloseExternalDoors(HashSet<Tuple<int, int>> roomPositions)
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
                else if (adJpos.Item1 > pos.Item1)
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

    public void DestroyExistingRooms()
    {
        playerController.DestroyRooms(rooms);
    }

}
