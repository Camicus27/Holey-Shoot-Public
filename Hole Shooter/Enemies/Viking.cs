using System.Collections;
using UnityEngine;

/// <summary>
/// The Viking becomes stronger over time.
/// </summary>
public class Viking : Enemy
{
    [SerializeField] private int powerUpsCount;
    [SerializeField] private Transform armature;
    [SerializeField] private GameObject powerEffect;

    protected override void Start()
    {
        StartCoroutine(PowerUpRoutine());

        base.Start();
    }


    /// <summary>
    /// Stop and spawn a minion every minionSpawnCooldown seconds and repeat minionSpawnCount times.
    /// </summary>
    private IEnumerator PowerUpRoutine()
    {
        // Wait a few seconds
        WaitForSeconds waitASec = new WaitForSeconds(6f);
        WaitForSeconds waitQuarterSec = new WaitForSeconds(.25f);

        yield return waitASec;

        for (int i = 0; !hasWon && !isDead && i < powerUpsCount; i++)
        {
            // Wait if already staggered by something
            yield return new WaitUntil(() => !isStaggered);

            isStaggered = true;
            // This is a special use case to trigger the specific animation
            StartCoroutine(IndicateDamage(3));

            // Wait for powering up animation to finish
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
    /// Increase size and health.
    /// </summary>
    public void PowerUp()
    {
        health += 50;
        GameManager.instance.PlaySFX("PowerUp");
        Instantiate(powerEffect, transform.position, Quaternion.identity);

        StartCoroutine(IncreaseSize());
    }
    private IEnumerator IncreaseSize()
    {
        float t = 0;
        float duration = 2.15f;
        Vector3 originalScale = armature.localScale;
        Vector3 newScale = originalScale + (Vector3.one * 25);

        while (t < duration && !isDead)
        {
            armature.localScale = Vector3.Lerp(originalScale, newScale, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
    }
}
