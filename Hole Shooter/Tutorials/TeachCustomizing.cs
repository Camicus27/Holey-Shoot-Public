using UnityEngine;

public class TeachCustomizing : MonoBehaviour
{
    public void OpenCustomizer()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<HoleUIManager>().OnCustomizeButtonClicked();
    }
}
