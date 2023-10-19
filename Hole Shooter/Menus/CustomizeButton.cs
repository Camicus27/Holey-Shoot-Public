using UnityEngine;

public class CustomizeButton : MonoBehaviour
{
    private void Start()
    {
        GameManager.instance.OnTransactionOccurred += RefreshSelf;

        RefreshSelf();
    }


    private void OnDestroy()
    {
        GameManager.instance.OnTransactionOccurred -= RefreshSelf;
    }


    private void RefreshSelf()
    {
        if (SaveData.current.playerData.purchasedSkinIDs.Count < 4 && (SaveData.current.playerData.coins >= 5000 ||
                                                                      SaveData.current.playerData.gems >= 3))
        {
            GetComponent<Animator>().SetBool("Available", true);
            return;
        }

        var cheapest = SaveData.current.playerData.GetCheapestSkins(GameObject.FindGameObjectWithTag("Skins").GetComponent<SkinManager>().allSkins);

        if (SaveData.current.playerData.coins >= cheapest.coin ||
            SaveData.current.playerData.gems >= cheapest.gem)
            GetComponent<Animator>().SetBool("Available", true);
        else
            GetComponent<Animator>().SetBool("Available", false);
    }
}
