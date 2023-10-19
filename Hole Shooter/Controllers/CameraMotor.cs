using System.Collections;
using UnityEngine;

public class CameraMotor : MonoBehaviour
{
    // References
    [SerializeField] private Transform target;

    // Static Data
    private const float SMOOTH_SPEED = 0.125f; // 0 - 1
    private const float Y_OFFSET_FACTOR = 3.25f;
    private const float Z_OFFSET_FACTOR = -2f;
    private Vector3 offsetOrigin = new Vector3(0, 8.69f, -4.69f);
    private Vector3 velocity = Vector3.zero;
    private Quaternion down = Quaternion.Euler(90, 0, 0);

    // Logistics
    public Vector3 offset;
    private bool isZoomedOut;
    [HideInInspector] public bool transitioning = false;


    private void Awake()
    {
        GameManager.instance.OnHoleTransition += StartTransition;
        GameManager.instance.OnUpgradeHoleSize += UpgradeSize;
        GameManager.instance.OnToggleHoleZoom += ToggleZoom;
    }

    private void OnDestroy()
    {
        StopAllCoroutines();
        GameManager.instance.OnHoleTransition -= StartTransition;
        GameManager.instance.OnUpgradeHoleSize -= UpgradeSize;
        GameManager.instance.OnToggleHoleZoom -= ToggleZoom;
    }


    /// <summary>
    /// Set the origin to whatever the current offset is.
    /// </summary>
    private void Start()
    {
        isZoomedOut = SaveData.current.gameSettings.isZoomedOut;

        offset = offsetOrigin + new Vector3(0, Y_OFFSET_FACTOR * SaveData.current.playerData.sizeLvl, Z_OFFSET_FACTOR * SaveData.current.playerData.sizeLvl);

        transform.position = target.position + offset;
        transitioning = false;
    }


    /// <summary>
    /// Every fixed frame, smoothly track the target.
    /// </summary>
    private void FixedUpdate()
    {
        if (!transitioning)
        {
            Vector3 desiredPos = isZoomedOut ? target.position + (offset * 1.5f) : target.position + offset;

            transform.position = Vector3.SmoothDamp(transform.position, desiredPos, ref velocity, SMOOTH_SPEED);
        }
    }


    /// <summary>
    /// Toggle whether the camera is zoomed out or not.
    /// </summary>
    private void ToggleZoom()
    {
        isZoomedOut = !isZoomedOut;
        SaveData.current.gameSettings.isZoomedOut = isZoomedOut;
    }


    /// <summary>
    /// Lerps the offset based on the player level.
    /// </summary>
    private void UpgradeSize()
    {
        StartCoroutine(LerpOffset());
    }
    private IEnumerator LerpOffset()
    {
        // Animation setup
        float time = 0;
        float duration = 0.5f;
        Vector3 startOffset = offset;
        Vector3 targetOffset = startOffset + new Vector3(0, Y_OFFSET_FACTOR, Z_OFFSET_FACTOR);

        // Animate over time (duration)
        while (time <= duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            t = Mathf.Sin(t * Mathf.PI * 0.5f);
            offset = Vector3.Lerp(startOffset, targetOffset, t);
            yield return null;
        }
        offset = targetOffset;
    }


    /// <summary>
    /// Animate the transition to the shooting phase.
    /// </summary>
    private void StartTransition()
    {
        transitioning = true;
        StartCoroutine(TransitionToShooting());
    }
    private IEnumerator TransitionToShooting()
    {
        float t = 0;
        float smooth = 0;
        float duration = 2.5f;
        Vector3 startPos = transform.position;
        Quaternion startRot = transform.rotation;
        Quaternion targetRot = down;
        // Move towards the hole
        while (transform.position.y > 1)
        {
            smooth = t / duration;
            transform.position = Vector3.Lerp(startPos, target.position, smooth);
            transform.rotation = Quaternion.Lerp(startRot, targetRot, smooth);
            t += Time.deltaTime;
            yield return null;
        }

        GameManager.instance.Fader(false, 1.5f);

        t = 0;
        Vector3 desiredPos = target.position + (Vector3.down * 10);
        startPos = transform.position;
        // Move into the hole and fade out
        while (transform.position.y > -9f)
        {
            transform.position = Vector3.Lerp(startPos, desiredPos, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
    }
}
