using System;
using UnityEngine;

public class RewardsButton : MonoBehaviour
{
    private Animator anim;


    private void Start()
    {
        GameManager.instance.OnCloseRewardMenu += RefreshSelf;

        anim = gameObject.GetComponent<Animator>();
        RefreshSelf();
    }


    private void OnDestroy()
    {
        GameManager.instance.OnCloseRewardMenu -= RefreshSelf;
    }


    private void RefreshSelf()
    {
        TimeSpan dailyRewSpan = DateTime.UtcNow - GameManager.instance.StringToDate(SaveData.current.playerData.dailyRewardTimeStamp);

        if (dailyRewSpan.TotalHours >= 18)
            anim.SetBool("Available", true);
        else
            anim.SetBool("Available", false);
    }
}
