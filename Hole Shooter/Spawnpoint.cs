using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Representation of an item spawnpoint.
/// </summary>
[System.Serializable]
public class Spawnpoint
{
    /// <summary>
    /// Coordinates in the grid of Hexagons. [row, col]
    /// </summary>
    [SerializeField] public Vector2Int coords;
    /// <summary>
    /// Game world position.
    /// </summary>
    [SerializeField] public Vector3 pos;
    /// <summary>
    /// Whether it is in the upper or lower position of the grid row.
    /// </summary>
    [SerializeField] public bool isLower;
    /// <summary>
    /// Whether it is a set of 7 hexagons combined to make one large spawn area.
    /// This is needed for very large patterns to appear
    /// </summary>
    [SerializeField] public bool isMultiHexagonal;


    public Spawnpoint(Vector2Int coordinates, Vector3 position)
    {
        coords = coordinates;
        pos = position;
        isLower = Mathf.Abs(coords.y % 2) == 1;
    }


    /// <summary>
    /// Checks all edges for any neighboring spawnpoints.
    /// </summary>
    /// <returns>List of up to 6 edge IDs that have a neighbor</returns>
    public List<int> GetNeighbors()
    {
        List<int> neighbors = new List<int>();
        Spawnpoint comparer = new Spawnpoint(new Vector2Int(coords.x, coords.y - 1), Vector3.zero);

        // Compare all 6 possible neighbors
        if (isLower)
        {
            if (!SaveData.current.levelData.allPoints.Contains(comparer))
                neighbors.Add(5);

            comparer.coords.y += 2;
            if (!SaveData.current.levelData.allPoints.Contains(comparer))
                neighbors.Add(1);

            comparer.coords.x -= 1;
            if (!SaveData.current.levelData.allPoints.Contains(comparer))
                neighbors.Add(2);

            comparer.coords.y -= 1;
            if (!SaveData.current.levelData.allPoints.Contains(comparer))
                neighbors.Add(3);

            comparer.coords.y -= 1;
            if (!SaveData.current.levelData.allPoints.Contains(comparer))
                neighbors.Add(4);

            comparer.coords.y += 1;
            comparer.coords.x += 2;
            if (!SaveData.current.levelData.allPoints.Contains(comparer))
                neighbors.Add(0);
        }
        else
        {
            if (!SaveData.current.levelData.allPoints.Contains(comparer))
                neighbors.Add(4);

            comparer.coords.y += 2;
            if (!SaveData.current.levelData.allPoints.Contains(comparer))
                neighbors.Add(2);

            comparer.coords.x += 1;
            if (!SaveData.current.levelData.allPoints.Contains(comparer))
                neighbors.Add(1);

            comparer.coords.y -= 1;
            if (!SaveData.current.levelData.allPoints.Contains(comparer))
                neighbors.Add(0);

            comparer.coords.y -= 1;
            if (!SaveData.current.levelData.allPoints.Contains(comparer))
                neighbors.Add(5);

            comparer.coords.y += 1;
            comparer.coords.x -= 2;
            if (!SaveData.current.levelData.allPoints.Contains(comparer))
                neighbors.Add(0);
        }

        // If it found some neighbors, return them
        if (neighbors.Count > 0)
            return neighbors;

        return null;
    }


    public override bool Equals(object obj)
    {
        if (obj is Spawnpoint && coords.x == ((Spawnpoint)obj).coords.x && coords.y == ((Spawnpoint)obj).coords.y)
            return true;
        else
            return false;
    }


    public override int GetHashCode()
    {
        return (coords.x + ", " + coords.y).GetHashCode();
    }


    public override string ToString()
    {
        return "(x:" + pos.x.ToString("0.00") + ", z:" + pos.z.ToString("0.00") + $"), Row: {coords.x}, Col: {coords.y}";
    }


    //public string ToJsonString()
    //{
    //    return "{\"pos\":{\"x\":\"" + pos.x.ToString("0.00") + "\",\"y\":\"0\",\"z\":\"" + pos.z.ToString("0.00") + "\"}," +
    //        "\"coords\":{\"x\":\"" + coords.x.ToString() + "\",\"y\":\"" + coords.y.ToString() + "\"}," +
    //        "\"isLower\":\"" + isLower.ToString() + "\"}";
    //}
}