using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ItemSpawner : MonoBehaviour
{
    public bool drawHexs;
    public bool drawCenterHex;


    private float RADIUS;
    private float EDGE;

    [Header("\n------ TIER 1 ------")]
    public List<AssetReference> tier1A;
    public List<AssetReference> tier1B;
    public List<AssetReference> tier1C;
    public List<AssetReference> tier1D;
    public List<AssetReference> tier1F;

    [Header("\n\n------ TIER 2 ------")]
    public List<AssetReference> tier2A;
    public List<AssetReference> tier2B;
    public List<AssetReference> tier2C;
    public List<AssetReference> tier2D;

    [Header("\n\n------ TIER 3 ------")]
    public List<AssetReference> tier3A;
    public List<AssetReference> tier3B;
    public List<AssetReference> tier3C;
    public List<AssetReference> tier3D;

    [Header("\n\n------ TIER 4 ------")]
    public List<AssetReference> tier4A;
    public List<AssetReference> tier4B;
    public List<AssetReference> tier4C;
    public List<AssetReference> tier4D;

    [Header("\n\n------ TIER 5 ------")]
    public List<AssetReference> tier5A;
    public List<AssetReference> tier5B;
    public List<AssetReference> tier5C;
    public List<AssetReference> tier5D;

    
    /// <summary>
    /// Randomly generate items based on the Editors serialized inputs.
    /// </summary>
    private void Awake()
    {
        RADIUS = 17.5f;
        EDGE = RADIUS * 0.5f * Mathf.Sqrt(3);

        GameManager.instance.OnGenerateNewSpawnpoints += GenerateNewSpawnpoints;
        GameManager.instance.OnGenerateItems += RandomlySpawn;
    }

    private void OnDestroy()
    {
        GameManager.instance.OnGenerateNewSpawnpoints -= GenerateNewSpawnpoints;
        GameManager.instance.OnGenerateItems -= RandomlySpawn;
    }


    /// <summary>
    /// Randomly picks a tier + grade and spawns a random item group at
    /// each point from said tier + grade for every valid spawnpoint.
    /// </summary>
    private void RandomlySpawn()
    {
        // Generate small spawn area stuff
        Addressables.InstantiateAsync(tier1F[Random.Range(0, tier1F.Count)], new Vector3(15, 0, 0), Quaternion.identity, transform);
        Addressables.InstantiateAsync(tier1F[Random.Range(0, tier1F.Count)], new Vector3(8.5f, 0, 12), Quaternion.identity, transform);
        Addressables.InstantiateAsync(tier1F[Random.Range(0, tier1F.Count)], new Vector3(-8.5f, 0, 12), Quaternion.identity, transform);
        Addressables.InstantiateAsync(tier1F[Random.Range(0, tier1F.Count)], new Vector3(-15, 0, 0), Quaternion.identity, transform);
        Addressables.InstantiateAsync(tier1F[Random.Range(0, tier1F.Count)], new Vector3(-8.5f, 0, -12), Quaternion.identity, transform);
        Addressables.InstantiateAsync(tier1F[Random.Range(0, tier1F.Count)], new Vector3(8.5f, 0, -12), Quaternion.identity, transform);
        Addressables.InstantiateAsync(tier1F[Random.Range(0, tier1F.Count)], new Vector3(30, 0, 0), Quaternion.identity, transform);
        Addressables.InstantiateAsync(tier1F[Random.Range(0, tier1F.Count)], new Vector3(-30, 0, 0), Quaternion.identity, transform);

        float gradeAChance = Mathf.Pow(2.718f, -0.0169f * SaveData.current.playerData.stage);

        // Generate groups in all valid locations
        foreach (Spawnpoint spawnpoint in SaveData.current.levelData.GetAllSpawnpoints(false))
        {
            int tier = ChooseTier(spawnpoint);

            switch (tier)
            {
                // Tier 1
                case 1:
                    // Grade A, higher chance further into the game
                    if (Random.value > gradeAChance)
                        Addressables.InstantiateAsync(tier1A[Random.Range(0, tier1A.Count)], spawnpoint.pos, Quaternion.identity, transform);
                    // Grade B (25%), C (35%), D (40%)
                    else
                    {
                        switch (Random.value)
                        {
                            // B
                            case < 0.25f:
                                Addressables.InstantiateAsync(tier1B[Random.Range(0, tier1B.Count)], spawnpoint.pos, Quaternion.identity, transform);
                                break;
                            // C
                            case < 0.60f:
                                Addressables.InstantiateAsync(tier1C[Random.Range(0, tier1C.Count)], spawnpoint.pos, Quaternion.identity, transform);
                                break;
                            // D
                            default:
                                Addressables.InstantiateAsync(tier1D[Random.Range(0, tier1D.Count)], spawnpoint.pos, Quaternion.identity, transform);
                                break;
                        }
                    }
                    break;
                // Tier 2
                case 2:
                    // Grade A, higher chance further into the game
                    if (Random.value > gradeAChance)
                        Addressables.InstantiateAsync(tier2A[Random.Range(0, tier2A.Count)], spawnpoint.pos, Quaternion.identity, transform);
                    // Grade B (25%), C (35%), D (40%)
                    else
                    {
                        switch (Random.value)
                        {
                            // B
                            case < 0.25f:
                                Addressables.InstantiateAsync(tier2B[Random.Range(0, tier2B.Count)], spawnpoint.pos, Quaternion.identity, transform);
                                break;
                            // C
                            case < 0.60f:
                                Addressables.InstantiateAsync(tier2C[Random.Range(0, tier2C.Count)], spawnpoint.pos, Quaternion.identity, transform);
                                break;
                            // D
                            default:
                                Addressables.InstantiateAsync(tier2D[Random.Range(0, tier2D.Count)], spawnpoint.pos, Quaternion.identity, transform);
                                break;
                        }
                    }
                    break;
                // Tier 3
                case 3:
                    // Grade A, higher chance further into the game
                    if (Random.value > gradeAChance)
                        Addressables.InstantiateAsync(tier3A[Random.Range(0, tier3A.Count)], spawnpoint.pos, Quaternion.identity, transform);
                    // Grade B (25%), C (35%), D (40%)
                    else
                    {
                        switch (Random.value)
                        {
                            // B
                            case < 0.25f:
                                Addressables.InstantiateAsync(tier3B[Random.Range(0, tier3B.Count)], spawnpoint.pos, Quaternion.identity, transform);
                                break;
                            // C
                            case < 0.60f:
                                Addressables.InstantiateAsync(tier3C[Random.Range(0, tier3C.Count)], spawnpoint.pos, Quaternion.identity, transform);
                                break;
                            // D
                            default:
                                Addressables.InstantiateAsync(tier3D[Random.Range(0, tier3D.Count)], spawnpoint.pos, Quaternion.identity, transform);
                                break;
                        }
                    }
                    break;
                // Tier 4
                case 4:
                    // Grade A, higher chance further into the game
                    if (Random.value > gradeAChance)
                        Addressables.InstantiateAsync(tier4A[Random.Range(0, tier4A.Count)], spawnpoint.pos, Quaternion.identity, transform);
                    // Grade B (22%), C (38%), D (40%)
                    else
                    {
                        switch (Random.value)
                        {
                            // B
                            case < 0.22f:
                                Addressables.InstantiateAsync(tier4B[Random.Range(0, tier4B.Count)], spawnpoint.pos, Quaternion.identity, transform);
                                break;
                            // C
                            case < 0.60f:
                                Addressables.InstantiateAsync(tier4C[Random.Range(0, tier4C.Count)], spawnpoint.pos, Quaternion.identity, transform);
                                break;
                            // D
                            default:
                                Addressables.InstantiateAsync(tier4D[Random.Range(0, tier4D.Count)], spawnpoint.pos, Quaternion.identity, transform);
                                break;
                        }
                    }
                    break;
                // Tier 5
                case 5:
                    // Grade A, higher chance further into the game
                    if (Random.value > gradeAChance)
                        Addressables.InstantiateAsync(tier5A[Random.Range(0, tier5A.Count)], spawnpoint.pos, Quaternion.identity, transform);
                    // Grade B (20%), C (40%), D (40%)
                    else
                    {
                        switch (Random.value)
                        {
                            // B
                            case < 0.20f:
                                Addressables.InstantiateAsync(tier5B[Random.Range(0, tier5B.Count)], spawnpoint.pos, Quaternion.identity, transform);
                                break;
                            // C
                            case < 0.60f:
                                Addressables.InstantiateAsync(tier5C[Random.Range(0, tier5C.Count)], spawnpoint.pos, Quaternion.identity, transform);
                                break;
                            // D
                            default:
                                Addressables.InstantiateAsync(tier5D[Random.Range(0, tier5D.Count)], spawnpoint.pos, Quaternion.identity, transform);
                                break;
                        }
                    }
                    break;
            }
        }

        GameManager.instance.DoneLoadingItems();
    }


    /// <summary>
    /// Picks a tier for the given Spawnpoint based on the distance from the center of the map.
    /// </summary>
    /// <returns>Tier 1-5</returns>
    private int ChooseTier(Spawnpoint spawnpoint)
    {
        int tier;

        if (SaveData.current.playerData.stage < 12)
        {
            // Total value of the x, y coordinates in the grid
            switch (System.Math.Abs(spawnpoint.coords.x) + System.Math.Abs(spawnpoint.coords.y))
            {
                // Ring 1
                case 0:
                case 1:
                case 2:
                case 3:
                    tier = 1;
                    break;
                // Ring 2
                case 4:
                    if (Random.value < .5f)
                        tier = 2;
                    else
                        tier = 1;
                    break;
                // Ring 3
                case 5:
                case 6:
                    if (Random.value < .3f)
                        tier = 2;
                    else
                        tier = 3;
                    break;
                // Ring 4
                default:
                    if (Random.value < .3f)
                        tier = 3;
                    else
                        tier = 4;
                    break;
            }
        }
        else
        {
            float val = Random.value;

            /**
             * Ring 1 -> 1 = 50%, 2 = 30%, 3 = 20%
             * Ring 2 -> 1 = 30%, 2 = 35%, 3 = 25%, 4 = 10%
             * Ring 3 -> 1 = 10%, 2 = 40%, 3 = 30%, 4 = 20%
             * Ring 4 -> 2 = 15%, 3 = 25%, 4 = 40%, 5 = 20%
             * Ring 5 -> 3 = 15%, 4 = 40%, 5 = 45%
             **/

            // Total value of the x, y coordinates in the grid
            switch (System.Math.Abs(spawnpoint.coords.x) + System.Math.Abs(spawnpoint.coords.y))
            {
                // Ring 1
                case < 4:
                    if (val < .5f)
                        tier = 1;
                    else if (val < .8f)
                        tier = 2;
                    else
                        tier = 3;
                    break;
                // Ring 2
                case < 6:
                    if (val < .3f)
                        tier = 1;
                    else if (val < .65f)
                        tier = 2;
                    else if (val < .9f)
                        tier = 3;
                    else
                        tier = 4;
                    break;
                // Ring 3
                case < 8:
                    if (val < .1f)
                        tier = 1;
                    else if (val < .5f)
                        tier = 2;
                    else if (val < .8f)
                        tier = 3;
                    else
                        tier = 4;
                    break;
                // Ring 4
                case < 11:
                    if (val < .15f)
                        tier = 2;
                    else if (val < .4f)
                        tier = 3;
                    else if (val < .8f)
                        tier = 4;
                    else
                        tier = 5;
                    break;
                // Ring 5
                case < 13:
                    if (val < .15f)
                        tier = 3;
                    else if (val < .55f)
                        tier = 4;
                    else
                        tier = 5;
                    break;
                // Beyond Ring 5
                default:
                    tier = 5;
                    break;
            }
        }

        return tier;
    }


    /// <summary>
    /// Generate an additional set of spawnpoints to add onto the current grid of points.
    /// </summary>
    private void GenerateNewSpawnpoints()
    {
        HashSet<Spawnpoint> pointsToAdd = new HashSet<Spawnpoint>();
        List<Spawnpoint> pointsToRemove = new List<Spawnpoint>();

        // For every current spawnpoint
        foreach (Spawnpoint point in SaveData.current.levelData.expandablePoints)
        {
            // Get all the neighbors of the spawnpoint
            List<int> neighbors = point.GetNeighbors();

            if (neighbors != null)
            {
                Spawnpoint newPoint = new Spawnpoint(new Vector2Int(point.coords.x, point.coords.y), point.pos);

                // Pick random open edge
                int num = neighbors[Random.Range(0, neighbors.Count)];
                switch (num)
                {
                    case 0:
                        newPoint.coords.x += 1;
                        newPoint.pos.z += 2 * EDGE;
                        break;
                    case 1:
                        if (!newPoint.isLower)
                            newPoint.coords.x += 1;

                        newPoint.coords.y += 1;
                        newPoint.pos.z += EDGE;
                        newPoint.pos.x += 1.5f * RADIUS;
                        break;
                    case 2:
                        if (newPoint.isLower)
                            newPoint.coords.x -= 1;

                        newPoint.coords.y += 1;
                        newPoint.pos.z -= EDGE;
                        newPoint.pos.x += 1.5f * RADIUS;
                        break;
                    case 3:
                        newPoint.coords.x -= 1;
                        newPoint.pos.z -= 2 * EDGE;
                        break;
                    case 4:
                        if (newPoint.isLower)
                            newPoint.coords.x -= 1;

                        newPoint.coords.y -= 1;
                        newPoint.pos.z -= EDGE;
                        newPoint.pos.x -= 1.5f * RADIUS;
                        break;
                    case 5:
                        if (!newPoint.isLower)
                            newPoint.coords.x += 1;

                        newPoint.coords.y -= 1;
                        newPoint.pos.z += EDGE;
                        newPoint.pos.x -= 1.5f * RADIUS;
                        break;
                }

                pointsToAdd.Add(newPoint);
            }
            else
                pointsToRemove.Add(point);

        }

        // Add spawnpoint to the level
        foreach (var newPoint in pointsToAdd)
            SaveData.current.levelData.AddSpawnpoint(newPoint);


        // Clear out old spawnpoints
        foreach (var point in pointsToRemove)
            SaveData.current.levelData.expandablePoints.Remove(point);

        GameManager.instance.DoneGeneratingSpawnpoints();
    }
}
