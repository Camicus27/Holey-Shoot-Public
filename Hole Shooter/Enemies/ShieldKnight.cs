using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;

/// <summary>
/// The Shield Knight defends itself with a shield which blocks damage but can be broken.
/// </summary>
public class ShieldKnight : Enemy
{
    [SerializeField] private RuntimeAnimatorController basicEnemyController;
    [SerializeField] private GameObject shield;
    [SerializeField] private GameObject damagedShield;
    [SerializeField] private GameObject shardEffect;
    [SerializeField] private int shldHealth;
    private Coroutine shieldColorShiftRoutine = null;
    private int halfShieldHealth;
    private bool shieldDamaged = false;

    protected override void Start()
    {
        halfShieldHealth = (int)(shldHealth / 1.85f);
        
        base.Start();
    }


    public override int TakeDamage(int dmg, int hitLoc)
    {
        SaveData.current.statistics.hitsTemp++;

        // If there is a shield active
        if (hasShield)
        {
            shieldHealth -= dmg;

            if (shieldHealth <= 0)
            {
                // Find the shield particle system and destroy it
                foreach (ParticleSystem child in GetComponentsInChildren<ParticleSystem>(true))
                {
                    if (child.name[0] == 'W')
                    {
                        Destroy(child.gameObject);
                        break;
                    }
                }

                hasShield = false;
            }

            return 1;
        }

        // If the shield was hit
        if (hitLoc == 3 && shldHealth > 0)
        {
            shldHealth -= dmg;

            // If the shield breaks, stumble, then behave like normal
            if (shldHealth <= 0)
            {
                Instantiate(shardEffect, transform.position, Quaternion.identity);
                shield.SetActive(false);
                damagedShield.SetActive(false);
                SaveData.current.statistics.damageTemp += dmg + shldHealth;

                anim.runtimeAnimatorController = basicEnemyController;
                // 'Spawning' is the shield break stagger animation
                anim.Play("Spawning");
            }
            else
            {
                // Perform a color shift on the skin
                if (shieldColorShiftRoutine != null)
                    StopCoroutine(shieldColorShiftRoutine);
                shieldColorShiftRoutine = StartCoroutine(ShieldColorShift());

                // If the shield is below half health, show some damage
                if (!shieldDamaged && shldHealth <= halfShieldHealth)
                {
                    shieldDamaged = true;
                    damagedShield.SetActive(true);
                    shield.SetActive(false);

                    isStaggered = true;
                    StartCoroutine(IndicateDamage(1));
                }

                SaveData.current.statistics.damageTemp += dmg;
            }

            return 1;
        }


        // If the actual knight was hit
        health -= dmg;

        // Perform a color shift on the skin
        if (colorShiftRoutine != null)
            StopCoroutine(colorShiftRoutine);
        colorShiftRoutine = StartCoroutine(ColorShift());

        // Check if dead
        if (health <= 0)
        {
            // Update state
            isDead = true;
            bodyUpperCollider.enabled = false;
            bodyLowerCollider.enabled = false;
            headCollider.enabled = false;
            SaveData.current.statistics.damageTemp += dmg + health;

            if (hitLoc == 0)
                anim.SetTrigger("DeathHead");
            else
            {
                anim.SetFloat("PickDeath", PickRandomAnimation());
                anim.SetTrigger("DeathBody");
            }

            StartCoroutine(Deactivate());
        }
        else
        {
            SaveData.current.statistics.damageTemp += dmg;

            if (!isStaggered)
            {
                isStaggered = true;
                if (shldHealth > 0)
                    StartCoroutine(IndicateDamage(1));
                else
                    StartCoroutine(IndicateDamage(hitLoc));
            }
        }
        return health;
    }


    /// <summary>
    /// Flashes the skin a certain color to indicate they've been hit.
    /// </summary>
    private IEnumerator ShieldColorShift()
    {
        Renderer shieldRender;
        if (shieldDamaged)
            shieldRender = damagedShield.GetComponentInChildren<Renderer>(true);
        else
            shieldRender = shield.GetComponentInChildren<Renderer>(true);

        shieldRender.material.color = damageColor;
        float t = 0;
        float duration = .33f;

        while (t < duration)
        {
            shieldRender.material.color = Color.Lerp(damageColor, original, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
    }
}
