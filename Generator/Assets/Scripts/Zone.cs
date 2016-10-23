using UnityEngine;
using System.Collections;

public class Zone : ScriptableObject
{
    [System.Serializable]
    public class ZoneProps
    {
        public GameObject prefab;
        public MinMax density;
    }

    [System.Serializable]
    public class ZoneGround
    {
        public GameObject prefab;
        public float areaPercent;
    }

    public ZoneGround[] zoneGrounds;
    public ZoneProps[] zoneProps;

    public GameObject GenerateTile(float size)
    {

        GameObject ground = Instantiate(zoneGrounds.First().prefab) as GameObject;

        return ground;
    }
}
