using UnityEngine;

[CreateAssetMenu(fileName = "NewEnemy", menuName = "ScriptableObjects/Enemy")]
public class EnemySO : ScriptableObject
{
    [Header("Settings")]
    [field: Range(0.1f, 1f)] public float Speed;
    public float RunSpeed { get { return Mathf.Lerp(4.5f, 16f, Speed); } }
    public int Damage;
    public int Health;
    public int CoinValue;
}
