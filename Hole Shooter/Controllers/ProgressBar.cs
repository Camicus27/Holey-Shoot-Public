using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressBar : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI levelLabel;
    [SerializeField] private GameObject bossIcon;
    [SerializeField] private GameObject bossIntroParticleEffect;
    [SerializeField] private Image progressFillBar;

    private void Start()
    {
        UpdateLevelProgress();
    }


    /// <summary>
    /// Update bar with current data.
    /// </summary>
    private void UpdateLevelProgress()
    {
        progressFillBar.fillAmount = 0;

        if (SaveData.current.playerData.stage != SaveData.current.playerData.nextBossStage)
            levelLabel.text = SaveData.current.playerData.stage.ToString();
        else
        {
            levelLabel.gameObject.SetActive(false);
            
            if (!SaveData.current.playerData.hasSeenBossAnimation)
            {
                GameManager.instance.BlockUserInputs();

                SaveData.current.playerData.hasSeenBossAnimation = true;
                GameManager.instance.SaveFileToDisk();

                GameManager.instance.PlaySFX("BossUnlock");
                bossIntroParticleEffect.SetActive(true);

                StartCoroutine(WaitForBossIntro());
            }
            else
            {
                // Show the tutorial
                if (!SaveData.current.tutorialData.bossStageTutorialComplete)
                {
                    SaveData.current.tutorialData.bossStageTutorialComplete = true;

                    GameManager.instance.ShowUpcomingBossTutorial();
                    GameManager.instance.SaveFileToDisk();
                }
            }
            bossIcon.SetActive(true);
        }

        // Update the progress bar
        if (SaveData.current.playerData.stage > 0)
            StartCoroutine(UpdateLevelProgressRoutine());
    }
    private IEnumerator UpdateLevelProgressRoutine()
    {
        float progNeeded = SaveData.current.playerData.nextBonusStage - SaveData.current.playerData.prevBonusStage;
        float newProg = (SaveData.current.playerData.stage - SaveData.current.playerData.prevBonusStage) / progNeeded;
        if (newProg > 1)
            newProg = 1;

        float t = 0;
        float duration = 1.2f;
        while (t < duration)
        {
            progressFillBar.fillAmount = Mathf.Lerp(0, newProg, t / duration);
            t += Time.deltaTime;
            yield return null;
        }

        progressFillBar.fillAmount = newProg;

        if (newProg >= .99f && SaveData.current.playerData.stage != SaveData.current.playerData.nextBossStage)
        {
            GetComponent<Animator>().enabled = true;
            GameManager.instance.PlaySFX("BonusUnlock");
        }
    }


    private IEnumerator WaitForBossIntro()
    {
        yield return new WaitForSeconds(3.69f);

        // Show the tutorial
        if (!SaveData.current.tutorialData.bossStageTutorialComplete)
        {
            SaveData.current.tutorialData.bossStageTutorialComplete = true;

            GameManager.instance.ShowUpcomingBossTutorial();
            SaveData.current.tutorialData.UpdateTutorialCompletionProg();

            GameManager.instance.SaveFileToDisk();
        }

        GameManager.instance.UnblockUserInputs();
    }


    public void AnimateBonusStage()
    {
        Animator anim = GetComponent<Animator>();
        if (anim.enabled)
        {
            GameManager.instance.BlockUserInputs();
            GameManager.instance.PlaySFX("BonusSta");
            anim.SetTrigger("BONUS");
        }
        else
            GameManager.instance.PlaySFX("No");
    }


    public void PlayConfettiSFX()
    {
        GameManager.instance.PlaySFX("Confetti");
        GameManager.instance.PlaySFX("Zip");
    }


    public void PlayGlowSFX()
    {
        GameManager.instance.PlaySFX("BonusTra");
    }


    public void ActivateBonusStage()
    {
        GameManager.instance.PlayBonusStage();
    }
}