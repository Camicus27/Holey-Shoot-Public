using UnityEngine;

public class TeachUpgrades : MonoBehaviour
{
    public void UpgradeSize()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<HoleUIManager>().TryUpgradeSize();
    }

    public void UpgradeTime()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<HoleUIManager>().TryUpgradeTimer();
    }

    public void OpenUpgrades()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<HoleUIManager>().OnUpgradesButtonClicked();

        SaveData.current.tutorialData.upgradesTutorialComplete = true;
        GameManager.instance.SaveFileToDisk();
    }
}
