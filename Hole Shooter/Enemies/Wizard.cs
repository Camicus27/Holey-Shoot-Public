using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// The Wizard can cast a protection shield upon them.
/// </summary>
public class Wizard : Enemy
{
    [SerializeField] private int shieldCastCount;
    [SerializeField] private int shieldProtectionHealth;
    [SerializeField] private GameObject castEffect;
    [SerializeField] private GameObject shieldEffect;

    protected override void Start()
    {
        StartCoroutine(ShieldCastRoutine());

        base.Start();
    }


    /// <summary>
    /// Stop and spawn a minion every minionSpawnCooldown seconds and repeat minionSpawnCount times.
    /// </summary>
    private IEnumerator ShieldCastRoutine()
    {
        // Wait a few seconds
        WaitForSeconds waitASec = new WaitForSeconds(6f);
        WaitForSeconds waitQuarterSec = new WaitForSeconds(.25f);

        yield return waitASec;

        for (int i = 0; !hasWon && !isDead && i < shieldCastCount; i++)
        {
            // Wait if already staggered by something
            yield return new WaitUntil(() => !isStaggered);

            isStaggered = true;
            // This is a special use case to trigger the specific animation
            StartCoroutine(IndicateDamage(3));

            // Wait for casting animation to finish
            yield return new WaitUntil(() => !isStaggered);

            // Cooldown
            yield return waitQuarterSec;
            yield return waitASec;
        }
    }


    /// <summary>
    /// Override allows for special use case of 'HitBodyL'.
    /// </summary>
    protected override IEnumerator IndicateDamage(int hitLoc)
    {
        // Set the animation
        switch (hitLoc)
        {
            case 0:
                anim.SetFloat("PickHitHead", PickRandomAnimation());
                anim.SetTrigger("HitHead");

                // Perform a color shift on the skin
                if (colorShiftRoutine != null)
                    StopCoroutine(colorShiftRoutine);
                colorShiftRoutine = StartCoroutine(ColorShift());
                break;
            case 1:
            case 2:
                anim.SetFloat("PickHitBodyUp", PickRandomAnimation());
                anim.SetTrigger("HitBodyUp");

                // Perform a color shift on the skin
                if (colorShiftRoutine != null)
                    StopCoroutine(colorShiftRoutine);
                colorShiftRoutine = StartCoroutine(ColorShift());
                break;
            case 3:
                anim.SetTrigger("HitBodyL");
                break;
        }

        // Wait for animation to finish
        yield return new WaitForSeconds(0.25f);
        yield return new WaitUntil(() => !anim.GetCurrentAnimatorStateInfo(0).IsTag("Hit"));
        isStaggered = false;
    }


    /// <summary>
    /// Cast a protection shield on 2 nearby teammates.
    /// </summary>
    public void CastProtection()
    {
        // Cast the spell
        health -= 5;
        GameManager.instance.PlaySFX("PowerUp");
        Instantiate(castEffect, transform);

        // Find all enemies
        List<Transform> enemies = new List<Transform>();
        foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            if (enemy.name[0] != 'W')
                enemies.Add(enemy.transform);
        }

        // Find the closest 2 enemies
        Transform[] closestEnemies = new Transform[2] { null, null };
        if (enemies.Count > 2)
        {
            closestEnemies = enemies.ToArray().OrderBy(t => (t.position - transform.position).sqrMagnitude)
                           .Take(2)
                           .ToArray();
        }
        else
        {
            for (int i = 0; i < enemies.Count; i++)
                closestEnemies[i] = enemies[i];
        }

        // Place the shield on them
        if (closestEnemies[0] != null)
        {
            if (closestEnemies[0].GetComponent<Enemy>().GiveShield(shieldProtectionHealth))
                Instantiate(shieldEffect, closestEnemies[0]);
        }
        if (closestEnemies[1] != null)
        {
            if (closestEnemies[1].GetComponent<Enemy>().GiveShield(shieldProtectionHealth))
                Instantiate(shieldEffect, closestEnemies[1]);
        }
    }
}
