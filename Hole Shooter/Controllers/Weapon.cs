using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // References
    [SerializeField] private Transform fpCam;
    [SerializeField] private Transform firePoint;
    [SerializeField] private BulletSO[] bullets;

    // Logistics
    public bool isShooting;
    private bool oneHandedMode;
    private bool outOfAmmo;
    private string SFX;

    // Data
    private float fireRate;
    private float variableFireRate;
    private float fireDelayTime;
    public int currentBullet;
    private Dictionary<int, int> arsenal;


    private void Awake()
    {
        GameManager.instance.OnSwapAmmo += SwitchAmmoType;
        GameManager.instance.OnStartShooting += StartHoldButton;
        GameManager.instance.OnStopShooting += ReleaseButton;
        GameManager.instance.OnToggleOneHanded += ToggleOneHanded;
        GameManager.instance.OnChangeFireRate += FireRateChanged;

        oneHandedMode = SaveData.current.gameSettings.oneHandedMode;
        if (oneHandedMode)
        {
            StartCoroutine(BeginAutoShooting());
        }
    }

    private void OnDestroy()
    {
        GameManager.instance.OnSwapAmmo -= SwitchAmmoType;
        GameManager.instance.OnStartShooting -= StartHoldButton;
        GameManager.instance.OnStopShooting -= ReleaseButton;
        GameManager.instance.OnToggleOneHanded -= ToggleOneHanded;
        GameManager.instance.OnChangeFireRate -= FireRateChanged;
    }


    /// <summary>
    /// Fill the arsenal with the given collection of bullets.
    /// </summary>
    public void PopulateArsenal(Dictionary<int, int> bullets)
    {
        // Initialization
        outOfAmmo = false;
        arsenal = new Dictionary<int, int>(bullets);

        // Assign default bullet to the lowest ID in the player's arsenal
        int id = 0;
        if (arsenal.Count > 0)
            id = arsenal.First().Key;
        // No bullets collected, just fail immediately
        else
        {
            fireRate = 0.33f;
            outOfAmmo = true;
        }

        // If the player collects nothing, fail immediately
        if (outOfAmmo)
            GameManager.instance.OutOfAmmo();
        else
            GameManager.instance.SwapToAmmo(id);
    }


    /// <summary>
    /// Empty the arsenal.
    /// </summary>
    public void ClearArsenal() { arsenal.Clear(); }


    /// <summary>
    /// Returns the dictionary of all the ammo in the player's arsenal.
    /// </summary>
    public Dictionary<int, int> GetAllAmmo() { return arsenal; }


    /// <summary>
    /// Swap to the given ammo type ID.
    /// </summary>
    public void SwitchAmmoType(int id)
    {
        currentBullet = id;

        switch (id)
        {
            case 0:
                SFX = "9mm";
                break;
            case 1:
                SFX = "556";
                break;
            case 2:
                SFX = "Arrow"; // Kunai
                break;
            case 3:
                SFX = "Grenade";
                break;
            case 4:
                SFX = "Arrow";
                break;
            case 5:
                SFX = "Grenade"; // Axe
                break;
            case 6:
                SFX = "12g";
                break;
            case 7:
                SFX = "Nuke";
                break;
        }

        fireRate = bullets[id].FireRate;
        GameManager.instance.FireRateChanged();
    }


    /// <summary>
    /// Attempt to shoot the given ammo type ID.
    /// </summary>
    private void TryShoot(int id)
    {
        // Verify the user can actually shoot something
        if (!arsenal.ContainsKey(id))
        {
            // If there is no more ammo
            if (arsenal.Count <= 0)
            {
                GameManager.instance.PlaySFX("NoAmmo");
                fireRate = 0.33f;
                variableFireRate = 100;
                outOfAmmo = true;
                GameManager.instance.OutOfAmmo();
                return;
            }

            id = arsenal.First().Key;
            GameManager.instance.SwapToAmmo(id);
        }

        // Check if we still have this type of ammo
        if (arsenal[id] > 0)
        {
            // Play firing sound
            GameManager.instance.PlaySFX(SFX);

            // Remove the bullet and update the UI
            arsenal[id]--;
            GameManager.instance.RefreshHotbar(id, arsenal[id]);

            // Create the bullet
            GameObject bulletObj = Instantiate(bullets[id].BulletPrefab, new Vector3(0, 20), Quaternion.identity);
            Bullet bullet = bulletObj.GetComponent<Bullet>();
            bullet.Activate(firePoint.position, fpCam.forward);

            // Spin the Axe and Grenade
            if (id == 5 || id == 3)
                bullet.AddSpin();
            // Make a Shotgun spread
            else if (id == 6)
                bullet.DuplicateAndSpread(firePoint.position, fpCam.forward);

            SaveData.current.statistics.shotsTemp++;

            // If that was the last bullet of this type
            if (arsenal[id] == 0)
            {
                arsenal.Remove(id);

                // If this was the last bullet overall
                if (arsenal.Count <= 0)
                {
                    GameManager.instance.PlaySFX("NoAmmo");
                    fireRate = 0.33f;
                    variableFireRate = 100;
                    outOfAmmo = true;
                    GameManager.instance.OutOfAmmo();
                    return;
                }
               
                fireRate = 0.2f;
            }
        }
    }


    /// <summary>
    /// Begin firing sequence.
    /// </summary>
    public void StartHoldButton() { isShooting = true; }


    /// <summary>
    /// Stop firing sequence.
    /// </summary>
    public void ReleaseButton() { isShooting = false; }


    public void ToggleOneHanded()
    {
        oneHandedMode = !oneHandedMode;
        isShooting = oneHandedMode;
    }


    /// <summary>
    /// Delay then begin shooting automatically for the user.
    /// </summary>
    public IEnumerator BeginAutoShooting()
    {
        yield return new WaitForSecondsRealtime(3f);
        if (!outOfAmmo && oneHandedMode)
            isShooting = true;
    }


    /// <summary>
    /// Change rate to the percent between 50% slower than the current firerate and current max firerate.
    /// </summary>
    public void FireRateChanged(float newRatePercent)
    {
        variableFireRate = Mathf.Lerp(fireRate * 1.5f, fireRate, newRatePercent);
    }


    /// <summary>
    /// Every fixed frame, attempt to shoot, given the player is still holding the fire button.
    /// </summary>
    private void FixedUpdate()
    {
        // Cooldown between shots
        if (fireDelayTime > 0)
            fireDelayTime -= Time.deltaTime;
        else
        {
            // Shoot and reset cooldown
            if (isShooting)
            {
                if (!outOfAmmo)
                    TryShoot(currentBullet);
                else
                    GameManager.instance.PlaySFX("NoAmmo");

                if (oneHandedMode)
                    fireDelayTime = variableFireRate;
                else
                    fireDelayTime = fireRate;
            }
        }
    }
}
