using UnityEngine;
using UnityEngine.AddressableAssets;

[CreateAssetMenu(fileName = "NewSkin", menuName = "ScriptableObjects/Skin")]
public class SkinSO : ScriptableObject
{
    [Header("References")]
    public AssetLabelReference BG;
    public AssetLabelReference FG;
    public AssetLabelReference Anim;

    [Header("Properties")]
    public int ID;
    public int Price;
    public bool DoesCostGems;
}
