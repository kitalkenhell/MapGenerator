using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Generator : MonoBehaviour
{
    [System.Serializable]
    public struct ZoneParameters
    {
        public Zone zone;
        public float areaPercent;
        public MinMax islandSize;
    }

    [System.Serializable]
    public class Coordinates
    {
        public int x;
        public int y;

        public Coordinates(int x = 0, int y = 0)
        {
            this.x = x;
            this.y = y;
        }
    }

    public float tileSize;
    public int width;
    public int height;
    public ZoneParameters[] zones;

    public GameObject[,] generatedZones;

    void Awake()
    {
        generatedZones = new GameObject[width, height];
    }

    void Start()
    {
        NormalizeAreas();
        StartCoroutine(Generate());
        Generate();
    }

    void NormalizeAreas()
    {
        float totalArea = 0;

        foreach (var zone in zones)
        {
            totalArea += zone.areaPercent;
        }

        for (int i = 0; i < zones.Length; ++i)
        {
            zones[i].areaPercent /= totalArea;
        }
    }

    IEnumerator Generate()
    {
        int totalTiles = width * height;

        for (int i = 0; i < zones.Length; i++)
        {
            int amount = Mathf.CeilToInt(totalTiles * zones[i].areaPercent);
            List<Coordinates> currentZoneTiles = new List<Coordinates>(amount);
            
            for (int spotsToFill = 0; spotsToFill < amount; spotsToFill++)
            {
                int islandSize = Mathf.CeilToInt(zones[i].islandSize.Random());

                Coordinates freeSpot = FindFreeSpot();
                currentZoneTiles.Clear();

                if (freeSpot == null)
                {
                    break;
                }

                generatedZones[freeSpot.x, freeSpot.y] = GenerateTile(zones[i].zone, freeSpot.x * tileSize, freeSpot.y * tileSize);
                currentZoneTiles.Add(freeSpot);
                //yield return new WaitForSeconds(0.05f);

                while (freeSpot != null && spotsToFill < amount)
                {
                    int index = Random.Range(0, currentZoneTiles.Count);

                    --islandSize;
                    freeSpot = null;

                    if (islandSize < 0)
                    {
                        break;
                    }

                    for (int j = index; j < currentZoneTiles.Count && freeSpot == null; ++j)
                    {
                        freeSpot = FindFreeNeighbour(currentZoneTiles[j].x, currentZoneTiles[j].y);
                    }

                    for (int j = index - 1; j >= 0 && freeSpot == null; --j)
                    {
                        freeSpot = FindFreeNeighbour(currentZoneTiles[j].x, currentZoneTiles[j].y);
                    }

                    if (freeSpot != null)
                    {
                        generatedZones[freeSpot.x, freeSpot.y] = GenerateTile(zones[i].zone, freeSpot.x * tileSize, freeSpot.y * tileSize);
                        currentZoneTiles.Add(freeSpot);
                        ++spotsToFill;
                        //yield return new WaitForSeconds(0.05f);
                    }
                }
            }
        }

       yield return null;
    }

    Coordinates FindFreeNeighbour(int x, int y)
    {
        const int maxNeighbours = 4;

        Coordinates left = new Coordinates(x - 1, y);
        Coordinates right = new Coordinates(x + 1, y);
        Coordinates top = new Coordinates(x, y + 1);
        Coordinates bottom = new Coordinates(x, y - 1);

        List<Coordinates> neighbours = new List<Coordinates>(maxNeighbours);

        if (IsSpotFree(left)) neighbours.Add(left);
        if (IsSpotFree(right)) neighbours.Add(right);
        if (IsSpotFree(bottom)) neighbours.Add(bottom);
        if (IsSpotFree(top)) neighbours.Add(top);

        if (neighbours.Count > 0)
        {
            int index = Random.Range(0, neighbours.Count);

            return neighbours[index];
        }

        return null;
    }

    Coordinates FindFreeSpot()
    {
        int x = Random.Range(0, width);

        for (int i = x; i < width; i++)
        {
            int y = Random.Range(0, height);

            for (int j = y; j < height; j++)
            {
                if (generatedZones[i, j] == null)
                {
                    return new Coordinates(i, j);
                }
            }

            for (int j = y - 1; j >= 0; --j)
            {
                if (generatedZones[i, j] == null)
                {
                    return new Coordinates(i, j);
                }
            }
        }

        for (int i = x - 1; i >= 0; --i)
        {
            int y = Random.Range(0, height);

            for (int j = y; j < height; j++)
            {
                if (generatedZones[i, j] == null)
                {
                    return new Coordinates(i, j);
                }
            }

            for (int j = y - 1; j >= 0; --j)
            {
                if (generatedZones[i, j] == null)
                {
                    return new Coordinates(i, j);
                }
            }
        }

        return null;
    }

    bool IsSpotFree(int x, int y)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            return generatedZones[x, y] == null;
        }

        return false;
    }

    bool IsSpotFree(Coordinates coords)
    {
        return IsSpotFree(coords.x, coords.y);
    }

    GameObject GenerateTile(Zone zone, float x, float y)
    {
        GameObject tile = zone.GenerateTile(tileSize);

        tile.transform.position = new Vector3(x, 0, y);

        return tile;
    }
}
