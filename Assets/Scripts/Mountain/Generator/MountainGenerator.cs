using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountainGenerator : MonoBehaviour
{
    [SerializeField] private GameObject planePrefab;
    [SerializeField] private int planeSize = 5;
    const int LOAD_RANGE = 1;
    const int GARBAGE_COLLETION_RATE = 250;
    private int counter = 0;
    private Dictionary<Vector2Int, GameObject> planeDict = new Dictionary<Vector2Int, GameObject>();
    private HashSet<Vector2Int> farAwayPlanes = new HashSet<Vector2Int>();
    public void Start()
    {
        for (int x = LOAD_RANGE; x <= LOAD_RANGE; x++)
        {
            for (int y = 0; y < LOAD_RANGE; y++)
            {
                GeneratePlane(new Vector2Int(x,y));
            }
        }
    }

    public void Refresh(Transform playerTransform)
    {
        
        Vector2Int playerPosition = new Vector2Int((int)playerTransform.position.x / planeSize, (int)playerTransform.position.y / planeSize);
        for (int x = -LOAD_RANGE; x <= LOAD_RANGE; x++)
        {
            for (int y = -LOAD_RANGE; y <= LOAD_RANGE; y++)
            {
                Vector2Int position = playerPosition + new Vector2Int(x, y);
                GeneratePlane(position);
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

    private void GeneratePlane(Vector2Int position)
    {
        if (planeDict.ContainsKey(position)) return;
        GameObject plane = GameObject.Instantiate(planePrefab,transform);
        plane.transform.localPosition = planeSize * new Vector3(position.x, position.y, 0);
        plane.name = $"Wall[{position.x},{position.y}]";
        planeDict[position] = plane;
    }
}
