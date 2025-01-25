using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MountainSlopeGenerator : MonoBehaviour
{
    private Terrain terrain;
    void Start()
    {
        terrain = GetComponent<Terrain>();
        Generate(terrain);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static void Generate(Terrain terrain)
    {
        
    }
}
