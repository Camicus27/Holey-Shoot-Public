using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    protected Animator anim;
    [SerializeField] protected EnemySO info;
    [SerializeField] protected BoxCollider bodyUpperCollider;
    [SerializeField] protected BoxCollider bodyLowerCollider;
    [SerializeField] protected BoxCollider headCollider;

    [Header("Damage Effect")]
    [SerializeField] protected Renderer skinRender;
    [SerializeField] protected int skinIndex;
    protected Coroutine colorShiftRoutine = null;
    protected Color original;
    public Color damageColor;

    // Logistics
    protected Vector3 target;
    protected Vector3 movement;
    protected bool isDead = false;
    protected bool hasWon = false;
    protected bool isStaggered = false;
    protected bool hasShield = false;
    protected int shieldHealth = 0;

    // Data
    protected int health;
    protected float runningSpeed;


    protected void Awake()
    {
        GameManager.instance.OnStopShooterPhase += Win;
    }

    protected void OnDestroy()
    {
        GameManager.instance.OnStopShooterPhase -= Win;
        StopAllCoroutines();
    }


    /// <summary>
    /// Setup target to run at and start running.
    /// </summary>
    protected virtual void Start()
    {
        target = new Vector3(Random.Range(-3.069f, 3.069f), 0, 0.25f);
        anim = GetComponent<Animator>();

        bodyUpperCollider.enabled = true;
        bodyLowerCollider.enabled = true;
        headCollider.enabled = true;
        original = skinRender.materials[skinIndex].color;
        health = info.Health;
        runningSpeed = info.RunSpeed;

        SpawnIn();
    }


    /// <summary>
    /// Pop into scene and play spawn animation.
    /// </summary>
    protected virtual async void SpawnIn()
    {
        anim.SetFloat("PickSpawn", PickRandomAnimation());

        transform.position = new Vector3(transform.position.x, 0f, transform.position.z);

        while (anim.GetCurrentAnimatorStateInfo(0).IsTag("Spawn"))
            await Task.Yield();

        RunAtPlayer();
    }


    /// <summary>
    /// Run towards the target.
    /// </summary>
    protected virtual async void RunAtPlayer()
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
    /// Callback for animators to deal damage to the player.
    /// </summary>
    public void DealDamage()
    {
        GameManager.instance.IndicateAHit(info.Damage);
    }


    /// <summary>
    /// Special use case for dealing decreased or increased damage to the player.
    /// </summary>
    protected void DealSpecificDamage(int damage)
    {
        GameManager.instance.IndicateAHit(damage);
    }


    /// <summary>
    /// Remove the given damage from health.
    /// </summary>
    /// <param name="dmg">How much damage to take</param>
    /// <param name="hitLoc">0: Headshot, 1: Upper Body, 2: Lower Body</param>
    /// <returns>Remaining health after taking damage</returns>
    public virtual int TakeDamage(int dmg, int hitLoc)
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
                StartCoroutine(IndicateDamage(hitLoc));
            }
        }
        return health;
    }


    /// <summary>
    /// Remove the given explosive damage from health.
    /// </summary>
    /// <param name="dmg">How much damage to take</param>
    /// <param name="src">Position of the explosion source</param>
    /// <returns>Remaining health after taking damage</returns>
    public virtual int TakeExplosiveDamage(int dmg, Vector3 src)
    {
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

            transform.LookAt(src);
            transform.position += new Vector3(0, 0.1f, 0);
            transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
            anim.SetTrigger("DeathExplo");
            anim.SetFloat("Speed", Random.Range(.85f, 1.15f));

            StartCoroutine(Deactivate());

            // If there is an active wizard shield particle system, destroy it
            if (hasShield)
            {
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
        }
        else
        {
            SaveData.current.statistics.damageTemp += dmg;

            if (!isStaggered)
            {
                isStaggered = true;
                StartCoroutine(IndicateDamage(1));
            }
        }
        return health;
    }


    /// <summary>
    /// Animates a small stagger when hit.
    /// </summary>
    /// <param name="hitLoc">0: Headshot, 1: Upper Body, 2: Lower Body</param>
    protected virtual IEnumerator IndicateDamage(int hitLoc)
    {
        // Set the animation
        switch (hitLoc)
        {
            case 0:
                anim.SetFloat("PickHitHead", PickRandomAnimation());
                anim.SetTrigger("HitHead");
                break;
            case 1:
                anim.SetFloat("PickHitBodyUp", PickRandomAnimation());
                anim.SetTrigger("HitBodyUp");
                break;
            case 2:
                anim.SetFloat("PickHitBodyL", PickRandomAnimation());
                anim.SetTrigger("HitBodyL");
                break;
        }

        // Perform a color shift on the skin
        if (colorShiftRoutine != null)
            StopCoroutine(colorShiftRoutine);
        colorShiftRoutine = StartCoroutine(ColorShift());

        // Wait for animation to finish
        yield return new WaitForSeconds(0.25f);
        yield return new WaitUntil(() => !anim.GetCurrentAnimatorStateInfo(0).IsTag("Hit"));
        isStaggered = false;
    }


    public bool GiveShield(int shieldHealth)
    {
        if (!hasShield)
        {
            hasShield = true;
            this.shieldHealth = shieldHealth;
            return true;
        }
        return false;
    }


    /// <summary>
    /// Enemies win, celebrate!
    /// </summary>
    public void Win()
    {
        if (!isDead && !GameManager.instance.levelCompleted)
        {
            hasWon = true;

            anim.SetFloat("PickDance", PickRandomDance());
            anim.SetTrigger("Dance");
        }
    }


    /// <summary>
    /// Remove body after a couple seconds.
    /// </summary>
    protected virtual IEnumerator Deactivate()
    {
        if (isDead && !hasWon)
        {
            yield return new WaitForSeconds(.1f);
            GameManager.instance.EnemyKilled(info.CoinValue);
            yield return new WaitForSeconds(2.9f);

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
    /// Flashes the skin a certain color to indicate they've been hit.
    /// </summary>
    protected virtual IEnumerator ColorShift()
    {
        skinRender.materials[skinIndex].color = damageColor;
        float t = 0;
        float duration = .33f;

        while (!isDead && t < duration)
        {
            skinRender.materials[skinIndex].color = Color.Lerp(damageColor, original, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        skinRender.materials[skinIndex].color = original;
    }


    /// <summary>
    /// Randomly pick an animation variant to play.
    /// </summary>
    protected float PickRandomAnimation()
    {
        if (Random.value < .5f)
            return 0f;
        else
            return 1f;
    }


    /// <summary>
    /// Randomly pick a dance animation to play.
    /// </summary>
    protected float PickRandomDance()
    {
        float val = Random.value;

        if (val < .2f)
            return 0f;
        else if (val < .4f)
            return 0.25f;
        else if (val < .6f)
            return 0.5f;
        else if (val < .8f)
            return 0.75f;
        else
            return 1f;
    }
}
