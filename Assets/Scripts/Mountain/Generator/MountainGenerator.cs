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
    private const int PILLAR_HEIGHT = 10;
    const int GARBAGE_COLLETION_RATE = 250;
    private int counter = 0;
    private Dictionary<Vector2Int, GameObject> planeDict = new Dictionary<Vector2Int, GameObject>();
    private HashSet<Vector2Int> farAwayPlanes = new HashSet<Vector2Int>();
    private Dictionary<Vector2Int, MountainPillar> pillarDict = new Dictionary<Vector2Int, MountainPillar>();
    private const float ROTATION = 45;
    private const float X_ROTATION = -90f;
    private const float PLANE_DIAGONAL_SIZE = PLANE_SIZE * 1.41421356237f/2;
    public void Start()
    {
        const int TEST_SIZE = 8;
        List<Vector2Int> pillars = new List<Vector2Int>();
        for (int x = -TEST_SIZE; x < TEST_SIZE; x++)
        {
            for (int y = 0; y < TEST_SIZE; y++)
            {
                pillars.Add(new Vector2Int(x,y));
            }
        }
        
        foreach (Vector2Int pillarPosition in pillars)
        {
            pillarDict[pillarPosition] = MountainPillarFactory.GeneratePillar(pillarPosition);
        }

        foreach (Vector2Int pillarPosition in pillars)
        {
            GameObject pillar = new GameObject();
            pillar.name = $"Pillar[{pillarPosition.x},{pillarPosition.y}]";
            float zPosition = 2 * pillarPosition.y * PLANES_PER_FACE * PLANE_DIAGONAL_SIZE;
            float zOffset = Mathf.Abs(PLANES_PER_FACE * PLANE_DIAGONAL_SIZE * (pillarPosition.x % 2));
            float xPosition = pillarPosition.x * PLANES_PER_FACE * (PLANE_SIZE + PLANE_DIAGONAL_SIZE);
            float yPosition = pillarPosition.y * PILLAR_HEIGHT * PLANE_SIZE;
            float yOffset = -Mathf.Abs(PLANE_SIZE * PILLAR_HEIGHT / 2f * ((pillarPosition.x+1) % 2));
            pillar.transform.SetParent(transform);
            pillar.transform.localPosition = new Vector3(xPosition, yPosition + yOffset, zPosition + zOffset);
            for (int x = -LOAD_RANGE; x <= LOAD_RANGE; x++)
            {
                for (int y = 0; y <= PILLAR_HEIGHT; y++)
                {
                    GeneratePlane(new Vector2Int(x, y), pillarPosition,pillar.transform);
                }
            }
        }
        
    }
    
    public void Refresh(Transform playerTransform)
    {
        Vector2Int pillarPosition = new Vector2Int((int)((playerTransform.position.x - PILLAR_LENGTH/2)/PILLAR_LENGTH), (int)((playerTransform.position.y - PILLAR_LENGTH/2)/PILLAR_LENGTH));
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

    private void GeneratePlane(Vector2Int planePosition, Vector2Int pillarPosition, Transform parent)
    {
        
        if (!pillarDict.ContainsKey(pillarPosition)) return;
        GameObject plane = GameObject.Instantiate(planePrefab,parent);
        
        if (planePosition.x > PLANES_PER_FACE/2) // Rotated by 45
        {
            plane.transform.localPosition = GetWallPosition(planePosition);
            plane.transform.rotation = Quaternion.Euler(X_ROTATION, -ROTATION, 0);
        } else if (planePosition.x < -PLANES_PER_FACE/2) // Rotated by -45
        {
            Vector2Int mirror = planePosition;
            mirror.x = -mirror.x;
            Vector3 position = GetWallPosition(mirror);
            position.x = -position.x;
            plane.transform.localPosition = position;
            plane.transform.rotation = Quaternion.Euler(X_ROTATION, ROTATION, 0);
        }
        else
        {
            plane.transform.localPosition = PLANE_SIZE * new Vector3(planePosition.x, planePosition.y,0);
            plane.transform.rotation = Quaternion.Euler(X_ROTATION, 0, 0);
        }
        
        plane.name = $"Wall[{planePosition.x},{planePosition.y}]";
        planeDict[planePosition] = plane;
    }

    private Vector3 GetWallPosition(Vector2Int planePosition)
    {
        int diagOffset = planePosition.x - PLANES_PER_FACE/2-1;
        return new Vector3(PLANE_SIZE / 2f * (PLANES_PER_FACE / 2), 0, 0)
               + 1 / 2f * new Vector3(PLANE_DIAGONAL_SIZE, 0, PLANE_DIAGONAL_SIZE)
               + diagOffset * new Vector3(PLANE_DIAGONAL_SIZE, 0, PLANE_DIAGONAL_SIZE)
               + PLANE_SIZE * new Vector3(PLANES_PER_FACE / 2, planePosition.y, 0);

    }
}
