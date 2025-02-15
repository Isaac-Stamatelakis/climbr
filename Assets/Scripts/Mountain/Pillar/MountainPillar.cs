using System.Collections.Generic;
using UnityEngine;

namespace Mountain.Pillar
{
    public class GeneratedMountainPillar
    {
        public MountainPillar Pillar;
        public MountainPillarObjects Objects;
    }

    public class MountainPillarObjects
    {
        private Dictionary<Vector2Int, GameObject> planeDict = new Dictionary<Vector2Int, GameObject>();
        private HashSet<Vector2Int> farAwayPlanes = new HashSet<Vector2Int>();
       
        public bool IsEmpty()
        {
            return planeDict.Count == 0;
        }
    }
    public class MountainPillar
    {
        public Vector2Int Position;
        public MountainPillarData Data;

        public MountainPillar(Vector2Int position, MountainPillarData data)
        {
            Position = position;
            Data = data;
        }
    }

    public class MountainPillarData
    {
    
    }

    public static class MountainPillarFactory
    {
        public static MountainPillar GeneratePillar(Vector2Int position)
        {
            return new MountainPillar(position, GeneratePillarData());
        }

        private static MountainPillarData GeneratePillarData()
        {
            // TODO
            return null;
        }
    }
}