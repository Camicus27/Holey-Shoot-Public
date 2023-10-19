using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// The Exploder charges the player and explodes upon reaching them.
/// Also explodes upon death dealing damage to anything near it.
/// </summary>
public class Exploder : Enemy
{
    [SerializeField] private GameObject explosionEffect;
    [SerializeField] private int radius;


    /// <summary>
    /// Run towards the target.
    /// </summary>
    protected override async void RunAtPlayer()
    {
        anim.SetFloat("Speed", Mathf.Lerp(0.69f, 1.15f, info.Speed));

        // While still alive and not yet at the target, run towards target
        while (!isDead && !hasWon && transform.position.z > 8.33f)
        {
            if (!isStaggered)
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
            transform.LookAt(new Vector3(target.x, 0, -5f));
            anim.SetFloat("PickAttack", PickRandomAnimation());
            anim.SetBool("isAttacking", true);
        }
    }


    public void ManualExplode()
    {
        Explode(false);
        Destroy(gameObject, .1f);
    }


    /// <summary>
    /// Remove body after a couple seconds.
    /// </summary>
    protected override IEnumerator Deactivate()
    {
        if (isDead && !hasWon)
        {
            GameManager.instance.EnemyKilled(info.CoinValue);
            yield return new WaitForSeconds(1.01f);
            Explode(true);
            yield return null;
            yield return null;
            yield return null;

            if (!hasWon)
                Destroy(gameObject);
        }
    }


    /// <summary>
    /// Get all enemies within a set radius and tell them to all take explosive damage.
    /// </summary>
    private void Explode(bool wasKilled)
    {
        // Spawn partical system
        GameManager.instance.PlaySFX("Explode");
        Instantiate(explosionEffect, transform.position - (Vector3.forward * 5), Quaternion.identity);

        // Get everything within the blast radius
        Collider[] colliders = Physics.OverlapSphere(transform.position - (Vector3.forward * 5), radius);

        // Deal damage to the player if nearby
        if ((transform.position - (Vector3.forward * 2)).z - radius <= 0)
        {
            if (wasKilled)
                DealSpecificDamage((int)(info.Damage * 0.33f));
            else
                DealDamage();
        }

        // Add explosive force to things within the radius
        foreach (Collider collider in colliders)
        {
            if (collider.name[0].Equals('T'))
                collider.GetComponentInParent<Enemy>().TakeExplosiveDamage(info.Damage, transform.position);
        }
    }
}
