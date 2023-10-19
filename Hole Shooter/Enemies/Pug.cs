using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// The Pug spawns mini versions of itself upon death.
/// </summary>
public class Pug : Enemy
{
    [SerializeField] private AssetReferenceGameObject minion;
    [SerializeField] private GameObject spawnEffect;
    private int minionSpawnCount;


    protected override void Start()
    {
        minionSpawnCount = Random.Range(3, 6);

        base.Start();
    }


    /// <summary>
    /// Spawn minions then remove body after a couple seconds.
    /// </summary>
    protected override IEnumerator Deactivate()
    {
        if (isDead && !hasWon)
        {
            GameManager.instance.AddEnemyToCount();
            yield return new WaitForSeconds(.1f);
            GameManager.instance.EnemyKilled(info.CoinValue);
            yield return new WaitForSeconds(2.9f);

            SummonFirstMinion();
            for (int i = 0; i < minionSpawnCount - 1; i++)
                SummonAMinion();

            Vector3 startPos = transform.position;
            Vector3 targetPos = new Vector3(startPos.x, -5f, startPos.z);

            float t = 0f;
            float duration = 0.6f;
            // Animate over time (duration)
            while (t < duration)
            {
                transform.position = Vector3.Lerp(startPos, targetPos, t / duration);
                t += Time.deltaTime;
                yield return null;
            }
            
            if (!hasWon)
                Destroy(gameObject);
        }
    }


    /// <summary>
    /// Calculate a position around the body, spawn a minion, and play a particle effect.
    /// </summary>
    private void SummonFirstMinion()
    {
        Vector3 spawnPos = transform.position + new Vector3(Random.Range(-4f, 4f), 0, Random.Range(1f, 4f));

        GameManager.instance.PlaySFX("Summon");
        Instantiate(spawnEffect, spawnPos, Quaternion.identity);
        Addressables.InstantiateAsync(minion, spawnPos, transform.rotation);
    }


    /// <summary>
    /// Calculate a position around the body, spawn a minion, and play a particle effect.
    /// </summary>
    private void SummonAMinion()
    {
        GameManager.instance.AddEnemyToCount();

        Vector3 spawnPos = transform.position + new Vector3(Random.Range(-4f, 4f), 0, Random.Range(1f, 4f));

        Instantiate(spawnEffect, spawnPos, Quaternion.identity);
        Addressables.InstantiateAsync(minion, spawnPos, transform.rotation);
    }
}
