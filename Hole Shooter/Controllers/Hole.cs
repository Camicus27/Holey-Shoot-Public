using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hole : MonoBehaviour
{
    // References
    public MeshCollider meshCollider;
    public Collider groundCollider;
    [SerializeField] private Image progressBar;
    [SerializeField] private Transform gravityPoint;
    [SerializeField] private TextMeshPro timerText;

    // Logistics
    public bool canMove;
    public bool isMoving;
    public Vector3 movement;

    // Data
    private float points;

    // Defaults
    private readonly Vector3 defaultScale = new Vector3(1.69f, 1.69f, 1.69f);
    private Vector3 scaleFactor = new Vector3(0.8f, 0f, 0.8f);
    private const float INITIAL_SPEED = 0.05f;


    private void Awake()
    {
        GameManager.instance.OnUpgradeHoleSize += UpgradeSize;

        GameManager.instance.OnJoystickMoved += Move;
        GameManager.instance.OnStartMoving += StartMoving;
        GameManager.instance.OnStopMoving += StopMoving;
        GameManager.instance.OnStopMovement += StopMovement;
        GameManager.instance.OnResumeMovement += ResumeMovement;

        GameManager.instance.OnItemCollected += AddItem;

        GameManager.instance.OnHardReset += HardReset;
        GameManager.instance.OnSoftReset += SoftReset;
    }

    private void OnDestroy()
    {
        
        GameManager.instance.OnUpgradeHoleSize -= UpgradeSize;

        GameManager.instance.OnJoystickMoved -= Move;
        GameManager.instance.OnStartMoving -= StartMoving;
        GameManager.instance.OnStopMoving -= StopMoving;
        GameManager.instance.OnStopMovement -= StopMovement;
        GameManager.instance.OnResumeMovement -= ResumeMovement;

        GameManager.instance.OnItemCollected -= AddItem;

        GameManager.instance.OnHardReset -= HardReset;
        GameManager.instance.OnSoftReset -= SoftReset;
    }


    public void Start()
    {
        if (SaveData.current.tutorialData.tutorialComplete)
            GameManager.instance.EquipSkin(SaveData.current.playerData.currentSkinID);
        else
            BasicReset();
    }


    /// <summary>
    /// Resets temporary progression as well.
    /// </summary>
    private void HardReset()
    {
        SaveData.current.playerData.speed = INITIAL_SPEED;

        BasicReset();
    }


    /// <summary>
    /// Does NOT reset temporary progression.
    /// </summary>
    private void SoftReset()
    {
        SaveData.current.playerData.speed = INITIAL_SPEED + SaveData.current.playerData.sizeLvl * 0.015f;

        BasicReset();

        transform.localScale += scaleFactor * SaveData.current.playerData.sizeLvl;
    }


    /// <summary>
    /// Resets the player's position, scale, speed, points, and inventory counts.
    /// </summary>
    private void BasicReset()
    {
        // Stop movement
        canMove = false;
        movement = Vector3.zero;

        // Reset points
        points = 0;
        progressBar.fillAmount = points;

        // Reset position and scale
        transform.position = Vector3.zero;
        transform.localScale = defaultScale;
    }


    /// <summary>
    /// Disable the player's ability to move.
    /// </summary>
    private void StopMovement()
    {
        canMove = false;
        movement = Vector3.zero;
    }


    /// <summary>
    /// Enable the player's ability to move.
    /// </summary>
    private void ResumeMovement()
    {
        canMove = true;
    }


    /// <summary>
    /// Gets input from the player to move the hole.
    /// </summary>
    private void Move(float x, float y)
    {
        movement.x = x;
        movement.z = y;
    }

    private void StartMoving() { isMoving = true; }
    private void StopMoving() { isMoving = false; }

    private void FixedUpdate()
    {
        if (canMove && isMoving)
        {
            transform.position += movement * SaveData.current.playerData.speed;
        }
    }


    /// <summary>
    /// Upgrades the size of the hole.
    /// </summary>
    private void UpgradeSize()
    {
        SaveData.current.playerData.speed += .015f;
        StartCoroutine(ScaleHole());
    }


    /// <summary>
    /// Add the given item to the player's inventory and current total points.
    /// </summary>
    public void AddItem(float pointVal)
    {
        points += pointVal;

        if (points < 1)
            progressBar.fillAmount += pointVal;
        else
        {
            GameManager.instance.PlaySFX("UpSize");

            // Adjust progress bar
            points -= 1f;
            progressBar.fillAmount = points;

            GameManager.instance.HoleSizeUpgraded();
        }
    }


    /// <summary>
    /// Scales the hole and improves the speed when the player levels up.
    /// </summary>
    public IEnumerator ScaleHole()
    {
        // Animation setup
        float time = 0;
        float duration = 0.5f;
        Vector3 startScale = transform.localScale;
        Vector3 targetScale = startScale + scaleFactor;

        // Animate over time (duration)
        while (time <= duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            t = Mathf.Sin(t * Mathf.PI * 0.5f);
            transform.localScale = Vector3.Lerp(startScale, targetScale, t);
            yield return null;
        }
        transform.localScale = targetScale;
    }


    /// <summary>
    /// When an edible is near the hole, activate it's interaction with the hole.
    /// </summary>
    /// <param name="other">Colliding object</param>
    private void OnTriggerEnter(Collider other)
    {
        other.attachedRigidbody.sleepThreshold = 0;

        // Swap collision to be with the hole's mesh instead of the ground's mesh
        Physics.IgnoreCollision(other, meshCollider, false);
        Physics.IgnoreCollision(other, groundCollider, true);

        Vector3 dir = gravityPoint.position - other.transform.position;
        other.attachedRigidbody.AddForce(dir.normalized * (269 / dir.sqrMagnitude));
    }


    /// <summary>
    /// When an edible is no longer near the hole, deactivate it's interaction with the hole.
    /// </summary>
    /// <param name="other"></param>
    private void OnTriggerExit(Collider other)
    {
        if (!other.attachedRigidbody.IsSleeping())
        {
            other.attachedRigidbody.sleepThreshold = 0.005f;

            // Swap collision to be with the ground's mesh instead of the hole's mesh
            Physics.IgnoreCollision(other, groundCollider, false);
            Physics.IgnoreCollision(other, meshCollider, true);
        }
    }
}
