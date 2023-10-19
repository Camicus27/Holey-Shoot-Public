using UnityEngine;

[CreateAssetMenu(fileName = "NewTreasure", menuName = "ScriptableObjects/Treasure")]
public class TreasureSO : ScriptableObject
{
    public bool IsGems;
    public int Value;
    public float Points;
}
