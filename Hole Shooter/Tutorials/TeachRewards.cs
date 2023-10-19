using UnityEngine;

public class TeachRewards : MonoBehaviour
{
    public void OpenRewards()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<HoleUIManager>().OnRewardsButtonClicked();

        SaveData.current.tutorialData.rewardsTutorialComplete = true;
        GameManager.instance.SaveFileToDisk();
    }
}
