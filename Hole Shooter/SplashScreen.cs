using UnityEngine;

public class SplashScreen : MonoBehaviour
{
    public void PlayJingle()
    {
        GameManager.instance.PlaySFX("C27");
    }

    public void PlayWoosh()
    {
        GameManager.instance.PlaySFX("Woosh");
    }
}
