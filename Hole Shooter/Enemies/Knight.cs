using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// The Knight cannot be staggered.
/// </summary>
public class Knight : Enemy
{
    protected override async void RunAtPlayer()
    {
        anim.SetFloat("Speed", Mathf.Lerp(0.69f, 1.15f, info.Speed));

        // While still alive and not yet at the target, run towards target
        while (!isDead && !hasWon && transform.position.z > 0.33f)
        {
            transform.LookAt(target);
            movement = (target - transform.position).normalized;
            transform.position += movement * (runningSpeed * Time.deltaTime);
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
}
