using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<AssetReferenceGameObject> testingEnemies;
    [SerializeField] private bool SpawnTesterEnemies;

    // References
    [SerializeField] private Transform nwCorner;
    [SerializeField] private Transform seCorner;

    [Header("Enemy Prefabs")]
    [SerializeField] private List<AssetReferenceGameObject> tier1;
    [SerializeField] private List<AssetReferenceGameObject> tier2;
    [SerializeField] private List<AssetReferenceGameObject> tier3;
    [SerializeField] private List<AssetReferenceGameObject> tier4;
    [SerializeField] private List<AssetReferenceGameObject> bosses;
    [SerializeField] private AssetReferenceGameObject bossSpawnEffect;

    // Logistics
    public int numOfEnemies;
    private List<int> enemyIDs;
    private WaitForSeconds spawnDelay;
    private Vector3 position = Vector3.zero;
    private Quaternion rotation;
    public bool isSpawning;
    public bool isBossStage = false;


    private void Awake()
    {
        GameManager.instance.OnStartSpawning += SpawnSetup;
        GameManager.instance.OnStopShooterPhase += StopSpawning;

        rotation = Quaternion.Euler(Vector3.up * 180);

        if (SaveData.current.playerData.stage == SaveData.current.playerData.nextBossStage)
            isBossStage = true;
    }

    private void OnDestroy()
    {
        GameManager.instance.OnStartSpawning -= SpawnSetup;
        GameManager.instance.OnStopShooterPhase -= StopSpawning;
    }


    /// <summary>
    /// Setup some limitations based on player's progress and start spawning enemies randomly.
    /// </summary>
    private void SpawnSetup()
    {
        isSpawning = true;
        if (isBossStage)
        {
            BossSpawnSetup();
            return;
        }

        int stage = SaveData.current.playerData.stage;
        // Determine enemy count
        switch (stage)
        {
            case 0:
                numOfEnemies = 4;
                break;
            case 1:
                numOfEnemies = 6;
                break;
            case 2:
                numOfEnemies = 8;
                break;
            case 3:
                numOfEnemies = 10;
                break;
            default:
                numOfEnemies = (int)(Mathf.Log(stage) * 10); // y = ln(x) * 10
                break;
        }
        GameManager.instance.SetEnemyCount(numOfEnemies);

        // Determine spawn cooldown
        spawnDelay = new WaitForSeconds(Mathf.Clamp(2.5f - (stage * 0.025f), 0.75f, 2.5f));

        // Determine the enemies that are going to spawn
        GenerateEnemySpawnList();

        // Start spawning!
        StartCoroutine(Spawning());
    }


    /// <summary>
    /// Setup for a boss stage. Pick the boss to play and start the sequence.
    /// </summary>
    private void BossSpawnSetup()
    {
        // Spawn some adds
        int adds = (int)(SaveData.current.playerData.stage * .25f);
        numOfEnemies = Random.Range(adds, adds + 4);
        GenerateEnemySpawnList();

        // Add in the boss
        enemyIDs.Insert(Random.Range(0, numOfEnemies), Random.Range(-bosses.Count, 0));
        numOfEnemies++;

        // Determine spawn cooldown
        spawnDelay = new WaitForSeconds(3);

        // Start spawning!
        GameManager.instance.SetEnemyCount(numOfEnemies);
        StartCoroutine(Spawning());
    }


    /// <summary>
    /// Spawn system routine.
    /// </summary>
    private IEnumerator Spawning()
    {
        yield return spawnDelay;
        position.y = -3;

        for (int i = 0; i < numOfEnemies; i++)
        {
            // Pick a random spawn point
            position.x = Random.Range(nwCorner.position.x, seCorner.position.x);
            position.z = Random.Range(seCorner.position.z, nwCorner.position.z);

            if (SpawnTesterEnemies)
                SpawnTESTING();
            else
            {
                // A boss
                if (enemyIDs[i] < 0)
                {
                    Addressables.InstantiateAsync(bosses[enemyIDs[i] + 3], position, rotation, transform);
                    Addressables.InstantiateAsync(bossSpawnEffect, position, Quaternion.identity, transform);
                    GameManager.instance.PlaySFX("BossSpawn");
                }
                else
                {
                    switch (enemyIDs[i])
                    {
                        case 1:
                            Addressables.InstantiateAsync(tier1[Random.Range(0, tier1.Count)], position, rotation, transform);
                            break;
                        case 2:
                            Addressables.InstantiateAsync(tier2[Random.Range(0, tier2.Count)], position, rotation, transform);
                            break;
                        case 3:
                            Addressables.InstantiateAsync(tier3[Random.Range(0, tier3.Count)], position, rotation, transform);
                            break;
                        case 4:
                            Addressables.InstantiateAsync(tier4[Random.Range(0, tier4.Count)], position, rotation, transform);
                            break;
                    }
                }
            }
                
            yield return spawnDelay;
        }
        isSpawning = false;
    }


    /// <summary>
    /// Randomly picks enemy types to spawn based on the stage.
    /// Chance to spawn harder enemies gets more likely the further the player gets.
    /// </summary>
    private void GenerateEnemySpawnList()
    {
        enemyIDs = new List<int>();
        float tier1Chance = Mathf.Pow(2.718f, -0.0169f * SaveData.current.playerData.stage); // e^(-0.0169 * x)
        float tier2Chance;
        float tier3Chance;
        int stage = SaveData.current.playerData.stage;

        ///  1 -> 1: 98.3% | 2: 1.70%
        /// 15 -> 1: 77.6% | 2: 15.6% | 3: 6.8%
        /// 29 -> 1: 61.3% | 2: 27.0% | 3: 11.7%
        if (stage < 30)
        {
            // 2: 69.69% | 3: 30.31%  (% of leftover after tier 1)
            tier2Chance = tier1Chance + (0.6969f * tier1Chance);
            if (1f - tier2Chance < 0.01f)
                tier2Chance = 1f;

            for (int i = 0; i < numOfEnemies; i++)
            {
                float val = Random.value;

                // Tier 1
                if (val < tier1Chance)
                    enemyIDs.Add(1);
                // Tier 2
                else if (val < tier2Chance)
                    enemyIDs.Add(2);
                // Tier 3
                else
                    enemyIDs.Add(3);
            }
        }
        /// 30 -> 1: 60.2% | 2: 23.8% | 3: 9.67% | 4: 6.33%
        /// 47 -> 1: 45.2% | 2: 32.7% | 3: 13.3% | 4: 8.80%
        /// 68 -> 1: 31.7% | 2: 40.7% | 3: 16.6% | 4: 11.0%
        else if (stage < 69)
        {
            // 2: 59.69% | 3: 24.31% | 4: 16%  (% of leftover after tier 1)
            tier2Chance = tier1Chance + (0.5969f * tier1Chance);
            if (1f - tier2Chance < 0.01f)
                tier2Chance = 1f;

            tier3Chance = tier2Chance + (0.2431f * tier1Chance);
            if (1f - tier3Chance < 0.01f)
                tier3Chance = 1f;

            for (int i = 0; i < numOfEnemies; i++)
            {
                float val = Random.value;

                // Tier 1
                if (val < tier1Chance)
                    enemyIDs.Add(1);
                // Tier 2
                else if (val < tier2Chance)
                    enemyIDs.Add(2);
                // Tier 3
                else if (val < tier3Chance)
                    enemyIDs.Add(3);
                // Tier 4
                else
                    enemyIDs.Add(4);
            }
        }
        /// 69 -> 1: 31.2% | 2: 32.3% | 3: 22.7% | 4: 13.8%
        /// 85 -> 1: 23.8% | 2: 35.7% | 3: 25.2% | 4: 15.3%
        /// 99 -> 1: 18.8% | 2: 38.2% | 3: 26.8% | 4: 16.2%
        else if (stage < 100)
        {
            // 2: 46.9% | 3: 33.1% | 4: 20%  (% of leftover after tier 1)
            tier2Chance = tier1Chance + (0.469f * tier1Chance);
            tier3Chance = tier2Chance + (0.331f * tier1Chance);

            for (int i = 0; i < numOfEnemies; i++)
            {
                float val = Random.value;

                // Tier 1
                if (val < tier1Chance)
                    enemyIDs.Add(1);
                // Tier 2
                else if (val < (tier1Chance += (tier2Chance * tier1Chance)))
                    enemyIDs.Add(2);
                // Tier 3
                else if (val < tier1Chance + (tier3Chance * tier1Chance))
                    enemyIDs.Add(3);
                // Tier 4
                else
                    enemyIDs.Add(4);
            }
        }
        /// 100 -> 1: 18.5% | 2: 32.4% | 3: 29.6% | 4: 19.5%
        /// 149 -> 1: 8.10% | 2: 36.5% | 3: 33.4% | 4: 22.0%
        else if (stage < 150)
        {
            // 2: 39.69% | 3: 36.31% | 4: 24%  (% of leftover after tier 1)
            tier2Chance = tier1Chance + (0.3969f * tier1Chance);
            tier3Chance = tier2Chance + (0.3631f * tier1Chance);

            for (int i = 0; i < numOfEnemies; i++)
            {
                float val = Random.value;

                // Tier 1
                if (val < tier1Chance)
                    enemyIDs.Add(1);
                // Tier 2
                else if (val < (tier1Chance += (tier2Chance * tier1Chance)))
                    enemyIDs.Add(2);
                // Tier 3
                else if (val < tier1Chance + (tier3Chance * tier1Chance))
                    enemyIDs.Add(3);
                // Tier 4
                else
                    enemyIDs.Add(4);
            }
        }
        else
        {
            // 2: 25% | 3: 33% | 4: 42%
            for (int i = 0; i < numOfEnemies; i++)
            {
                float val = Random.value;

                // tier 2 enemy
                if (val < .25f)
                    enemyIDs.Add(2);
                // tier 3 enemy
                else if (val < .58f)
                    enemyIDs.Add(3);
                // tier 4 enemy
                else
                    enemyIDs.Add(4);
            }
        }
    }


    /// <summary>
    /// Stop the spawning of enemies.
    /// </summary>
    public void StopSpawning()
    {
        StopAllCoroutines();
        isSpawning = false;
    }


    /// <summary>
    /// Setup enemy spawning for the tutorial scene.
    /// </summary>
    public void SpawnEnemiesTutorial()
    {
        isSpawning = true;
        GameManager.instance.SetEnemyCount(3);
        StartCoroutine(SpawningTutorial());
    }
    private IEnumerator SpawningTutorial()
    {
        // Spawn enemy number one
        position.x = 0;
        position.z = 69f;
        Addressables.InstantiateAsync(tier1[0], position, rotation, transform);

        yield return new WaitForSeconds(6f);


        // Spawn enemy number two
        position.x = -20f;
        Addressables.InstantiateAsync(tier1[0], position, rotation, transform);

        yield return new WaitForSeconds(6f);


        // Spawn enemy number three
        position.x = 20f;
        Addressables.InstantiateAsync(tier1[0], position, rotation, transform);

        isSpawning = false;
    }





    public bool showGizmo;
    private void OnDrawGizmos()
    {
        if (showGizmo)
        {
            Gizmos.color = Color.magenta;
            Vector3 a = new Vector3(nwCorner.position.x, .33f, nwCorner.position.z);
            Vector3 b = new Vector3(seCorner.position.x, .33f, nwCorner.position.z);
            Vector3 c = new Vector3(seCorner.position.x, .33f, seCorner.position.z);
            Vector3 d = new Vector3(nwCorner.position.x, .33f, seCorner.position.z);
            Gizmos.DrawLine(a, b);
            Gizmos.DrawLine(b, c);
            Gizmos.DrawLine(c, d);
            Gizmos.DrawLine(d, a);
        }
    }


    private int spawnVal = 0;
    private void SpawnTESTING()
    {
        if (spawnVal >= testingEnemies.Count)
            spawnVal = 0;

        Addressables.InstantiateAsync(testingEnemies[spawnVal], position, rotation, transform);
        spawnVal++;
    }
}
