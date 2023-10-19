using UnityEngine;

[CreateAssetMenu(fileName = "NewBullet", menuName = "ScriptableObjects/Bullet")]
public class BulletSO : ScriptableObject
{
    public int ID;

    [Header("References")]
    public GameObject BulletPrefab;
    public GameObject ExplosionEffect;

    [Header("Settings")]
    public float SizeXP;
    public int Damage;
    public int Lifetime;
    public int MuzzleVelocity;
    public float FireRate;
    public int SpecialAttribute;

    [Header("Properties")]
    public bool IsStickable;
    public bool IsExplosive;
    public bool IsPiercing;
}
