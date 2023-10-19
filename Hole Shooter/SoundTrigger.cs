using UnityEngine;

public class SoundTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        int id = int.Parse(other.tag);

        PlaySound(id);
    }


    /// <summary>
    /// Play the appropriate audio clip for the given bullet type.
    /// </summary>
    /// <param name="id">Bullet ID</param>
    private void PlaySound(int id)
    {
        switch (id)
        {
            case 0:
                GameManager.instance.PlaySFX("9mm_Coll");
                break;
            case 1:
                GameManager.instance.PlaySFX("556_Coll");
                break;
            case 2:
                GameManager.instance.PlaySFX("Kunai_Coll");
                break;
            case 3:
                GameManager.instance.PlaySFX("Grenade_Coll");
                break;
            case 4:
                GameManager.instance.PlaySFX("Arrow_Coll");
                break;
            case 5:
                GameManager.instance.PlaySFX("Axe_Coll");
                break;
            case 6:
                GameManager.instance.PlaySFX("12g_Coll");
                break;
            case 7:
                GameManager.instance.PlaySFX("Nuke_Coll");
                break;
            default:
                GameManager.instance.PlaySFX("9mm_Coll");
                break;
        }
    }
}
