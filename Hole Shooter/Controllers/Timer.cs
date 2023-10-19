using System.Collections;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshPro timer;

    private bool timerRunning = false;
    private float timeLimit = 20f;
    private const float INITIAL_TIME = 20.0f;

    private void Awake()
    {
        GameManager.instance.OnJoystickMoved += StartTimer;
        GameManager.instance.OnStopTimerEarly += StopTimerEarly;

        GameManager.instance.OnUpgradeHoleTimer += UpgradeTimer;

        GameManager.instance.OnHardReset += HardReset;
        GameManager.instance.OnSoftReset += SoftReset;
    }

    private void OnDestroy()
    {
        GameManager.instance.OnJoystickMoved -= StartTimer;
        GameManager.instance.OnStopTimerEarly -= StopTimerEarly;

        GameManager.instance.OnUpgradeHoleTimer -= UpgradeTimer;

        GameManager.instance.OnHardReset -= HardReset;
        GameManager.instance.OnSoftReset -= SoftReset;
    }


    private void HardReset()
    {
        timeLimit = INITIAL_TIME;
        timer.text = timeLimit.ToString("0.0");
    }


    private void SoftReset()
    {
        timeLimit = INITIAL_TIME + SaveData.current.playerData.timerLvl * 2;
        timer.text = timeLimit.ToString("0.0");
    }


    private void StartTimer(float x, float y)
    {
        // Only call start timer once, so remove event listener
        GameManager.instance.OnJoystickMoved -= StartTimer;

        GameManager.instance.StartHolePhase();

        StartCoroutine(TimerCountdown());
    }
    private IEnumerator TimerCountdown()
    {
        timerRunning = true;

        bool tickThree = false;
        bool tickTwo = false;
        bool tickOne = false;

        while (timerRunning && timeLimit > 0.03f)
        {
            timeLimit -= Time.deltaTime;
            timer.text = timeLimit.ToString("0.0");

            if (!tickThree && timeLimit <= 3)
            {
                tickThree = true;
                GameManager.instance.PlaySFX("Strum");
            }
            else if (!tickTwo && timeLimit <= 2)
            {
                tickTwo = true;
                GameManager.instance.PlaySFX("Strum");
            }
            else if (!tickOne && timeLimit <= 1)
            {
                tickOne = true;
                GameManager.instance.PlaySFX("Strum");
            }
            yield return null;
        }

        if (timerRunning)
            timer.text = "0.0";

        timerRunning = false;

        GameManager.instance.StopPlayerMovement();
        GameManager.instance.StopTimer();
    }


    private void StopTimerEarly()
    {
        timerRunning = false;
    }


    private void UpgradeTimer()
    {
        timeLimit += 2;
        timer.text = timeLimit.ToString("0.0");
    }
}
