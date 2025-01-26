using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountainGenerator : MonoBehaviour
{
    [SerializeField] private GameObject planePrefab;
    [SerializeField] private int width = 1;
    [SerializeField] private int height = 3;
    [SerializeField] private int planeSize = 5;

    public void Start()
    {
        for (int x = -width; x <= width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject plane = GameObject.Instantiate(planePrefab,transform);
                plane.transform.localPosition = planeSize * new Vector3(x, y, 0);
                plane.name = $"Wall[{x},{y}]";
            }
        }
    }
}
