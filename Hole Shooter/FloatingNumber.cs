using System.Threading.Tasks;
using TMPro;
using UnityEngine;

public class FloatingNumber : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI text;
    private float duration;
    private float value;


    public void SetupAndRun(float val, float dur)
    {
        duration = dur * 2;
        value = val;
        transform.localScale = Vector3.one * dur;
        text.text = "+" + value.ToString("0");
        FloatAway();
    }


    /// <summary>
    /// Routine for floating away from parent and disappearing.
    /// </summary>
    private async void FloatAway()
    {
        float t = 0;
        Color col = text.color;
        Vector3 startPos = transform.position;
        Vector3 endPos = startPos + new Vector3(Random.Range(-2.25f, 2.25f), Random.Range(-1f, -2.5f));

        float quarterDur = duration * .25f;
        while (t < quarterDur)
        {
            transform.position = Vector3.Lerp(startPos, endPos, t / duration);

            t += Time.deltaTime;
            await Task.Yield();
        }
        while (t < duration)
        {
            col.a = Mathf.Lerp(col.a, 0, t / duration);
            text.color = col;

            transform.position = Vector3.Lerp(startPos, endPos, t / duration);

            t += Time.deltaTime;
            await Task.Yield();
        }

        Destroy(gameObject);
    }
}
