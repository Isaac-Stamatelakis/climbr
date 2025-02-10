using System;
using System.Collections;
using System.Collections.Generic;
using Mountain.Pillar;
using UnityEngine;

public class MountainGenerator : MonoBehaviour
{
    private const int PLANES_PER_FACE = 3;
    [SerializeField] private GameObject planePrefab;
    private const int PLANE_SIZE = 10;
    private const float PILLAR_LENGTH = PLANES_PER_FACE*PLANE_SIZE * (1 + 2 * 1.41421356237f);
    const int LOAD_RANGE = 4;
    const int GARBAGE_COLLETION_RATE = 250;
    private int counter = 0;
    private Dictionary<Vector2Int, GameObject> planeDict = new Dictionary<Vector2Int, GameObject>();
    private HashSet<Vector2Int> farAwayPlanes = new HashSet<Vector2Int>();
    private Dictionary<Vector2Int, MountainPillar> pillarDict = new Dictionary<Vector2Int, MountainPillar>();
    public void Start()
    {
        pillarDict[Vector2Int.zero] = MountainPillarFactory.GeneratePillar(Vector2Int.zero);
        for (int x = -LOAD_RANGE; x <= LOAD_RANGE; x++)
        {
            for (int y = -LOAD_RANGE; y <= LOAD_RANGE; y++)
            {
                GeneratePlane(new Vector2Int(x, y),Vector2Int.zero);
            }
        }
    }
    
    public void Refresh(Transform playerTransform)
    {
        Debug.Log(PILLAR_LENGTH);
        Vector2Int pillarPosition = new Vector2Int((int)((playerTransform.position.x - PILLAR_LENGTH/2)/PILLAR_LENGTH), (int)((playerTransform.position.y - PILLAR_LENGTH/2)/PILLAR_LENGTH));
        Debug.Log(pillarPosition);
        Vector2Int playerPosition = new Vector2Int((int)playerTransform.position.x / PLANE_SIZE, (int)playerTransform.position.y / PLANE_SIZE);
        for (int x = -LOAD_RANGE; x <= LOAD_RANGE; x++)
        {
            for (int y = -LOAD_RANGE; y <= LOAD_RANGE; y++)
            {
                if (y < 0) continue;
                Vector2Int position = playerPosition + new Vector2Int(x, y);
                //GeneratePlane(position);
                farAwayPlanes.Remove(position);
            }
        }

        foreach (var (position, plane) in planeDict)
        {
            Vector2Int dif = position - playerPosition;
            if (dif.x is < -LOAD_RANGE or > LOAD_RANGE || dif.y is < -LOAD_RANGE or > LOAD_RANGE)
            {
                farAwayPlanes.Add(position);
            }
        }
    }
    

    public void FixedUpdate()
    {
        counter++;
        if (counter >= GARBAGE_COLLETION_RATE)
        {
            counter = 0;
            GarbageCollect();
        }
    }

    private void GarbageCollect()
    {
        foreach (Vector2Int position in farAwayPlanes)
        {
            if (!planeDict.Remove(position, out GameObject plane)) continue;
            Destroy(plane);
        }
        
    }

    private void GeneratePlane(Vector2Int planePosition, Vector2Int pillarPosition)
    {
        
        if (!pillarDict.ContainsKey(pillarPosition)) return;
        GameObject plane = GameObject.Instantiate(planePrefab,transform);
        
        const float ROTATION = 45;
        const float X_ROTATION = -90f;
        float dif = Mathf.Sqrt(PLANE_SIZE * PLANE_SIZE / 2f);
        
        if (planePosition.x > PLANES_PER_FACE/2) // Rotated by 45
        {
            int diagOffset = planePosition.x - PLANES_PER_FACE/2;
            plane.transform.localPosition = new Vector3(PLANE_SIZE/2f*(PLANES_PER_FACE/2),0,0) 
                - 1/2f * new Vector3(dif,0,dif) 
                + diagOffset * new Vector3(dif,0,dif) 
                + PLANE_SIZE * new Vector3(PLANES_PER_FACE/2, planePosition.y, pillarPosition.y);
            plane.transform.rotation = Quaternion.Euler(X_ROTATION, -ROTATION, 0);
        } else if (planePosition.x < -PLANES_PER_FACE/2) // Rotated by -45
        {
            int diagOffset = planePosition.x + PLANES_PER_FACE/2;
            plane.transform.localPosition =
                new Vector3(PLANE_SIZE/2f*(PLANES_PER_FACE/2),0,0) 
               - 1/2f * new Vector3(dif,0,dif) 
               + diagOffset * new Vector3(-dif,0,dif) 
               + PLANE_SIZE * new Vector3(-PLANES_PER_FACE/2, planePosition.y, pillarPosition.y);
            plane.transform.rotation = Quaternion.Euler(X_ROTATION, ROTATION, 0);
        }
        else
        {
            plane.transform.localPosition = PLANE_SIZE * new Vector3(planePosition.x, planePosition.y, pillarPosition.y);
            plane.transform.rotation = Quaternion.Euler(X_ROTATION, 0, 0);
        }
        
        plane.name = $"Wall[{planePosition.x},{planePosition.y}]";
        planeDict[planePosition] = plane;
    }
}
