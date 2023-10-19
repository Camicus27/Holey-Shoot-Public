using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class RewardsMenu : MonoBehaviour
{
    private Coroutine liveRewardCountdownRoutine = null;
    private int currDay;
    private TimeSpan currSpan;

    [SerializeField] private List<Reward> dayCells;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI rewardCooldown;
    [SerializeField] private TextMeshProUGUI streakCounter;

    [Header("Reward Animations")]
    [SerializeField] private AssetReferenceGameObject coinReward;
    [SerializeField] private AssetReferenceGameObject gemReward;


    private void Start()
    {
        transform.SetSiblingIndex(transform.parent.childCount - 2);

        Setup();
    }

    private void OnDestroy()
    {
        if (liveRewardCountdownRoutine != null)
            StopCoroutine(liveRewardCountdownRoutine);
    }

    public void CloseRewards()
    {
        GameManager.instance.SaveFileToDisk();
        GameManager.instance.CloseRewardMenu();
        GetComponent<Animator>().SetTrigger("SlideOut");
    }


    /// <summary>
    /// Sets up the cells of days that have already been collected and
    /// updates the time span.
    /// </summary>
    private void Setup()
    {
        // Loop through all collected days and make them collected
        int i;
        for (i = 0; i < SaveData.current.playerData.currRewardDay; i++)
            dayCells[i].SetBoolean("isCollected", true);

        currDay = i;
        currSpan = DateTime.UtcNow - GameManager.instance.StringToDate(SaveData.current.playerData.dailyRewardTimeStamp);

        dayCells[currDay].SetBoolean("isNext", true);

        LiveRewardCountdown();
    }


    /// <summary>
    /// Collect today's reward.
    /// </summary>
    public void TryCollectReward(int day)
    {
        if (dayCells[day].TryCollectReward())
        {
            GameManager.instance.PlaySFX("Celebrate");

            // Check for a daily streak
            if ((DateTime.UtcNow - GameManager.instance.StringToDate(SaveData.current.playerData.dailyRewardTimeStamp)).TotalDays < 2)
                SaveData.current.playerData.dailyRewardStreak++;
            else
                SaveData.current.playerData.dailyRewardStreak = 1;

            // Receive reward!
            GameManager.instance.ReceiveCoins(2000 + (SaveData.current.playerData.currRewardDay * 100) + ((SaveData.current.playerData.dailyRewardStreak - 1) * 250), .33f);
            Addressables.InstantiateAsync(coinReward).Completed += (reward) => {
                ParticleSystem system = reward.Result.GetComponent<ParticleSystem>();
                var coinEmiss = system.emission;
                coinEmiss.rateOverTime = 155;
                var coinMain = system.main;
                coinMain.maxParticles = 150;

                GameManager.instance.PlaySFX("Confetti");
                GameManager.instance.PlaySFX("Zip");
                system.Play(true);
            };

            // Increment day and stamp the time
            SaveData.current.playerData.currRewardDay++;
            SaveData.current.playerData.dailyRewardTimeStamp = DateTime.UtcNow.ToString("MM/dd/yyyy HH:mm:ss");

            // Refresh UI
            Setup();
            LiveRewardCountdown();
        }
        else
        {
            GameManager.instance.PlaySFX("No");
        }
    }


    /// <summary>
    /// Update the countdown every second.
    /// </summary>
    private void LiveRewardCountdown()
    {
        if (liveRewardCountdownRoutine != null)
            StopCoroutine(liveRewardCountdownRoutine);

        liveRewardCountdownRoutine = StartCoroutine(LiveRewardCountdownRoutine());
    }
    private IEnumerator LiveRewardCountdownRoutine()
    {
        WaitForSecondsRealtime oneSec = new WaitForSecondsRealtime(1);
        while (gameObject.activeSelf)
        {
            UpdateRewards();
            yield return oneSec;
        }
    }


    /// <summary>
    /// Refresh rewards countdown and update daily visuals.
    /// </summary>
    private void UpdateRewards()
    {
        // Set the current day to available (if 18 hours has passed)
        currSpan = currSpan.Add(TimeSpan.FromSeconds(1));
        if (currSpan.TotalHours >= 18)
        {
            dayCells[currDay].SetBoolean("isAvail", true);
            rewardCooldown.text = "REWARD AVAILABLE!";
        }
        else
        {
            rewardCooldown.text = "NEXT REWARD IN\n" +
                (17 - currSpan.Hours).ToString("00") + ":" +
                (59 - currSpan.Minutes).ToString("00") + ":" +
                (59 - currSpan.Seconds).ToString("00");
        }

        if (currSpan.TotalDays < 2)
            streakCounter.text = "DAILY\nSTREAK: " + SaveData.current.playerData.dailyRewardStreak;
        else
            streakCounter.text = "DAILY\nSTREAK: 0";
    }
}
