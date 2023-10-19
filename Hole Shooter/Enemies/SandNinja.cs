using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// The Sand Ninja is able to quickly disappear and reappear to evade fire.
/// </summary>
public class SandNinja : Enemy
{
    [SerializeField] private GameObject dustEffect;
    private bool hasDoneEvade1;
    private bool hasDoneEvade2;

    /// <summary>
    /// Run towards the target.
    /// </summary>
    protected override async void RunAtPlayer()
    {
        anim.SetFloat("Speed", Mathf.Lerp(0.69f, 1.15f, info.Speed));

        // While still alive and not yet at the target, run towards target
        while (!isDead && !hasWon && transform.position.z > 0.33f)
        {
            if (!isStaggered)
            {
                transform.LookAt(target);
                movement = (target - transform.position).normalized;
                transform.position += movement * (runningSpeed * Time.deltaTime);

                if (!hasDoneEvade1 && transform.position.z < 75f)
                {
                    hasDoneEvade1 = true;
                    StartCoroutine(EvadeRoutine());
                }
                else if (!hasDoneEvade2 && transform.position.z < 42f)
                {
                    hasDoneEvade2 = true;
                    StartCoroutine(EvadeRoutine());
                }
            }
            await Task.Yield();
        }

        // If at the target and not dead, attack target
        if (!isDead && !hasWon)
        {
            transform.position = target;
            transform.LookAt(new Vector3(target.x, 0, -5f));
            anim.SetFloat("PickAttack", PickRandomAnimation());
            anim.SetBool("isAttacking", true);
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
                break;
            case 1:
            case 2:
                anim.SetFloat("PickHitBodyUp", PickRandomAnimation());
                anim.SetTrigger("HitBodyUp");
                break;
        }

        // Wait for animation to finish
        yield return new WaitForSeconds(0.25f);
        yield return new WaitUntil(() => !anim.GetCurrentAnimatorStateInfo(0).IsTag("Hit"));
        isStaggered = false;
    }


    private IEnumerator EvadeRoutine()
    {
        yield return new WaitUntil(() => !isStaggered);
        isStaggered = true;

        // Wait for animation to finish
        anim.SetTrigger("HitBodyL");
        yield return new WaitForSeconds(0.769f);

        // Move to a new position
        Instantiate(dustEffect, transform.position, Quaternion.identity);
        transform.position = new Vector3(Random.Range(transform.position.x - 6f, transform.position.x + 6f),
                                         -10,
                                         Random.Range(transform.position.z + 6f, transform.position.z + 12f));
        yield return new WaitUntil(() => !anim.GetCurrentAnimatorStateInfo(0).IsTag("Hit"));
        yield return new WaitForSeconds(2f);

        // Pop back up!
        anim.SetFloat("PickSpawn", PickRandomAnimation());
        Instantiate(dustEffect, new Vector3(transform.position.x, 0f, transform.position.z), Quaternion.identity);
        anim.Play("Spawning");
        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);

        yield return new WaitForSeconds(0.25f);
        yield return new WaitUntil(() => !anim.GetCurrentAnimatorStateInfo(0).IsTag("Spawn"));
        isStaggered = false;
    }
}
