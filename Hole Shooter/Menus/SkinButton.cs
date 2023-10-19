using System.Collections;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class SkinButton : MonoBehaviour
{
    [SerializeField] private int skinID;

    [Header("References")]
    [SerializeField] private GameObject lockCover;
    [SerializeField] private GameObject activeHighlight;
    [SerializeField] private TextMeshProUGUI price;
    private SkinManager skins;


    private void Awake()
    {
        GameManager.instance.OnEquipSkin += EquipSkin;
        GameManager.instance.OnTransactionOccurred += RefreshSelf;

        skins = GameObject.FindGameObjectWithTag("Skins").GetComponent<SkinManager>();

        RefreshSelf();

        if (SaveData.current.playerData.currentSkinID == skinID)
            activeHighlight.SetActive(true);

        price.text = skins.allSkins[skinID].Price.ToString();
    }

    private void OnDestroy()
    {
        GameManager.instance.OnEquipSkin -= EquipSkin;
        GameManager.instance.OnTransactionOccurred -= RefreshSelf;
    }


    private async void RefreshSelf()
    {
        if (SaveData.current.playerData.purchasedSkinIDs.Contains(skinID))
            Destroy(lockCover);
        else
        {
            Animator anim = lockCover.GetComponent<Animator>();
            if ((skins.allSkins[skinID].DoesCostGems && SaveData.current.playerData.gems >= skins.allSkins[skinID].Price) ||
                (!skins.allSkins[skinID].DoesCostGems && SaveData.current.playerData.coins >= skins.allSkins[skinID].Price))
                anim.enabled = true;
            else if (anim.enabled)
            {
                anim.SetTrigger("clear");
                while (!anim.GetCurrentAnimatorStateInfo(0).IsName("SKINNotAffordable"))
                    await Task.Delay(5);

                anim.enabled = false;
            }
        }
    }


    public void TrySkinAction()
    {
        // Check if this skin hasn't been purchased yet
        if (!SaveData.current.playerData.purchasedSkinIDs.Contains(skinID))
        {
            // Attempt to purchase the skin
            if ((skins.allSkins[skinID].DoesCostGems && GameManager.instance.TryPayGems(skins.allSkins[skinID].Price)) ||
                (!skins.allSkins[skinID].DoesCostGems && GameManager.instance.TryPayCoins(skins.allSkins[skinID].Price)))
            {
                GameManager.instance.PlaySFX("Celebrate");

                // Add skin to purchased list
                SaveData.current.playerData.AddNewSkin(skinID);

                // Equip the newly purchased skin
                GameManager.instance.EquipSkin(skinID);
                GameManager.instance.PlaySFX("Yes");

                GameManager.instance.TransactionOccurred();
                GameManager.instance.SaveFileToDisk();
            }
            else
            {
                GameManager.instance.PlaySFX("No");
                StopAllCoroutines();
                StartCoroutine(CannotPerformAction());
            }
        }
        else
        {
            // Unequip if this is the current skin, otherwise equip it
            if (SaveData.current.playerData.currentSkinID == skinID)
            {
                GameManager.instance.PlaySFX("No");
                activeHighlight.SetActive(false);
                GameManager.instance.UnequipSkin();
            }
            else
            {
                GameManager.instance.PlaySFX("Yes");
                SaveData.current.playerData.currentSkinID = skinID;
                GameManager.instance.EquipSkin(skinID);
            }

            GameManager.instance.SaveFileToDisk();
        }
    }


    private void EquipSkin(int skinID)
    {
        if (this.skinID == skinID)
            activeHighlight.SetActive(true);
        else
            activeHighlight.SetActive(false);
    }


    /// <summary>
    /// Animate a small indication that the player can't do this action.
    /// It pops the desired text object and turns it red for a sec.
    /// </summary>
    private IEnumerator CannotPerformAction()
    {
        Color startColor = Color.white;
        Color midColor = Color.red;
        Vector3 startScale = Vector3.one;
        Vector3 midScale = startScale * 1.33f;

        float duration = .25f;
        float t = 0;
        while (t < duration)
        {
            price.color = Color.Lerp(startColor, midColor, t / duration);
            price.transform.localScale = Vector3.Lerp(startScale, midScale, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        t = 0;
        while (t < duration)
        {
            price.color = Color.Lerp(midColor, startColor, t / duration);
            price.transform.localScale = Vector3.Lerp(midScale, startScale, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        price.color = startColor;
        price.transform.localScale = startScale;
    }
}
