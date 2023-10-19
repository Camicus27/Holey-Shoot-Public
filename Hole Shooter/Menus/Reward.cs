using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Reward : MonoBehaviour
{
    private Coroutine cannotPerformActionRoutine = null;

    [SerializeField] private Animator anim;
    [SerializeField] private Image check;
    [SerializeField] private Image locker;

    public void SetBoolean(string name, bool val) { anim.SetBool(name, val); }

    public bool GetBoolean(string name) { return anim.GetBool(name); }

    private void OnDestroy()
    {
        if (cannotPerformActionRoutine != null)
            StopCoroutine(cannotPerformActionRoutine);
    }

    /// <summary>
    /// Collect today's reward.
    /// </summary>
    public bool TryCollectReward()
    {
        if (GetBoolean("isAvail"))
        {
            SetBoolean("isAvail", false);
            SetBoolean("isCollected", true);

            return true;
        }
        else if (GetBoolean("isCollected"))
            CannotPerformAction(true);
        else
            CannotPerformAction(false);

        return false;
    }

    /// <summary>
    /// Animate a small indication that the player can't do this action.
    /// It pops the image object and turns it red for a sec.
    /// </summary>
    /// <param name="isColl">Is this reward collected or not</param>
    public void CannotPerformAction(bool isColl)
    {
        if (cannotPerformActionRoutine != null)
            StopCoroutine(cannotPerformActionRoutine);

        if (isColl)
            cannotPerformActionRoutine = StartCoroutine(CannotPerformActionRoutine(check));
        else
            cannotPerformActionRoutine = StartCoroutine(CannotPerformActionRoutine(locker));
    }
    private IEnumerator CannotPerformActionRoutine(Image imageObj)
    {
        Color startColor = Color.white;
        Color midColor = Color.red;
        Vector3 startScale = Vector3.one;
        Vector3 midScale = startScale * 1.33f;

        float duration = .25f;
        float t = 0;
        while (t < duration)
        {
            imageObj.color = Color.Lerp(startColor, midColor, t / duration);
            imageObj.transform.localScale = Vector3.Lerp(startScale, midScale, t / duration);
            t += Time.deltaTime;
            yield return null;
        }
        t = 0;
        while (t < duration)
        {
            imageObj.color = Color.Lerp(midColor, startColor, t / duration);
            imageObj.transform.localScale = Vector3.Lerp(midScale, startScale, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        imageObj.color = startColor;
        imageObj.transform.localScale = startScale;
    }
}
