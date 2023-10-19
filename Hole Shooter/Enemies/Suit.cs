using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// The Suit boss cannot be staggered and occasionally spawns minions.
/// </summary>
public class Suit : Enemy
{
    [SerializeField] private AssetReferenceGameObject minion;
    [SerializeField] private GameObject spawnEffect;
    private int minionSpawnCount;
    private float minionSpawnCooldown;
    private bool isSpawningMinion;
    private bool canSpawnMinions;


    protected override void Start()
    {
        canSpawnMinions = true;
        isStaggered = true;
        minionSpawnCount = Random.Range(5, 10);
        minionSpawnCooldown = Mathf.Clamp(3f - (SaveData.current.playerData.stage * 0.069f), 1.69f, 4f);

        base.Start();
    }


    protected override async void RunAtPlayer()
    {
        StartCoroutine(SummonMinionsRoutine());
        anim.SetFloat("Speed", Mathf.Lerp(0.69f, 1.15f, info.Speed));

        // While still alive and not yet at the target, run towards target
        while (!isDead && !hasWon && transform.position.z > 0.33f)
        {
            if (!isSpawningMinion)
            {
                transform.LookAt(target);
                movement = (target - transform.position).normalized;
                transform.position += movement * (runningSpeed * Time.deltaTime);
            }
            await Task.Yield();
        }

        // If at the target and not dead, attack target
        if (!isDead && !hasWon)
        {
            canSpawnMinions = false;
            transform.position = target;
            transform.LookAt(new Vector3(target.x, 0, -5f));
            anim.SetBool("isAttacking", true);
        }
    }


    /// <summary>
    /// Stop and spawn a minion every minionSpawnCooldown seconds and repeat minionSpawnCount times.
    /// </summary>
    private IEnumerator SummonMinionsRoutine()
    {
        // Wait a few seconds
        WaitForSeconds waitASec = new WaitForSeconds(minionSpawnCooldown);
        WaitForSeconds waitQuarterSec = new WaitForSeconds(.25f);

        yield return waitASec;

        for (int i = 0; !hasWon && canSpawnMinions && i < minionSpawnCount; i++)
        {
            // Spawn a minion
            isSpawningMinion = true;
            anim.SetTrigger("HitHead");

            yield return waitQuarterSec;
            yield return waitQuarterSec;

            if (canSpawnMinions)
                SummonAMinion();

            // Wait for summoning animation to finish
            yield return waitQuarterSec;
            while (anim.GetCurrentAnimatorStateInfo(0).IsTag("Hit"))
                yield return null;

            // Cooldown
            yield return waitQuarterSec;
            isSpawningMinion = false;
            yield return waitASec;
        }
    }


    /// <summary>
    /// Calculate a position in front of the Summoner, spawn a minion, and play a particle effect.
    /// </summary>
    private void SummonAMinion()
    {
        GameManager.instance.AddEnemyToCount();

        Vector3 spawnPos = transform.position + new Vector3(Random.Range(-2.25f, 2.25f), 0, Random.Range(-11f, -14f));

        GameManager.instance.PlaySFX("Summon");
        Instantiate(spawnEffect, spawnPos, Quaternion.identity);
        Addressables.InstantiateAsync(minion, spawnPos, transform.rotation);
    }
}
