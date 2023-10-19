using UnityEngine;

public class TeachBonus : MonoBehaviour
{
    public void ActivateBonusLevel()
    {
        ProgressBar progressBar = null;
        foreach (GameObject button in GameObject.FindGameObjectsWithTag("Button"))
        {
            if ((progressBar = button.GetComponent<ProgressBar>()) != null)
                progressBar.AnimateBonusStage();
        }
    }
}
