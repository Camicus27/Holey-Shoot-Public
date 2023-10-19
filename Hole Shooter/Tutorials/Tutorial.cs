using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Tutorial : MonoBehaviour
{
    private bool cont;
    public bool skip;
    private WaitForSeconds waitOneSecond = new WaitForSeconds(1);
    private WaitForSeconds waitHalfSecond = new WaitForSeconds(.5f);
    private WaitUntil waitForContinue;

    [SerializeField] private GameObject skipButton;

    // Hole Phase
    [SerializeField] private GameObject teachMovement;
    [SerializeField] private GameObject shadeOverlay;
    [SerializeField] private GameObject encourageCollection;
    [SerializeField] private GameObject teachSize;
    [SerializeField] private GameObject prepareToFight;

    // Shooting Phase
    [SerializeField] private GameObject teachLooking;
    [SerializeField] private GameObject teachShooting;
    [SerializeField] private GameObject teachSwapping;
    [SerializeField] private GameObject encourageShooting;

    // Post Tutorial
    [SerializeField] private GameObject teachUpgradeTimer;
    [SerializeField] private GameObject teachUpgradeSize;
    [SerializeField] private GameObject teachUpgradeAmmo;


    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        GameManager.instance.OnTutorialContinue += ContinueButtonClicked;
        GameManager.instance.OnStopShooterPhase += EndTutorial;

        GameManager.instance.StopPlayerMovement();
        waitForContinue = new WaitUntil(CanContinue);
        cont = false;

        //skipButton.SetActive(true);

        StartCoroutine(PlayTutorialHole());
    }

    private void OnDestroy()
    {
        GameManager.instance.OnTutorialContinue -= ContinueButtonClicked;
        GameManager.instance.OnStopShooterPhase -= EndTutorial;
    }


    /// <summary>
    /// Coroutine for the hole phase tutorial.
    /// </summary>
    private IEnumerator PlayTutorialHole()
    {
        // Teach player to drag the screen
        shadeOverlay.SetActive(true);
        teachMovement.SetActive(true);
        yield return waitOneSecond;
        GameManager.instance.ResumePlayerMovement();

        // Wait for player to move
        yield return waitForContinue; // Enter
        cont = false;
        shadeOverlay.SetActive(false);
        Destroy(teachMovement);
        encourageCollection.SetActive(true);
        yield return waitForContinue; // Exit
        cont = false;

        // Wait for player to reach halfway point
        yield return waitForContinue; // Enter
        cont = false;
        Destroy(encourageCollection);

        // Tell player they need to grow to collect bigger and better ammo
        GameManager.instance.StopPlayerMovement();
        shadeOverlay.SetActive(true);
        teachSize.SetActive(true);
        yield return waitOneSecond;
        yield return waitHalfSecond;
        GameManager.instance.ResumePlayerMovement();

        // Wait for player to move
        yield return waitForContinue; // Exit
        shadeOverlay.SetActive(false);
        Destroy(teachSize);

        yield return new WaitUntil(AllCollected);

        //skipButton.SetActive(false);

        // Tell the player to get ready to fight
        GameManager.instance.StopPlayerMovement();
        Destroy(GameObject.FindGameObjectWithTag("Joystick"));
        shadeOverlay.SetActive(true);
        prepareToFight.SetActive(true);
        GameManager.instance.TransitionToShootingPhase();
        yield return new WaitForSeconds(2.5f);
        GameManager.instance.Fader(false, 0.69f);
        yield return waitOneSecond;
        Destroy(shadeOverlay);
        Destroy(prepareToFight);

        // Load the shooting scene
        SceneManager.LoadScene("Shooting_Tutorial", LoadSceneMode.Single);

        yield return null;

        // Setup for shooter phase tutorial
        Dictionary<int, int> inv = new Dictionary<int, int>
        {
            // Setup 200 9mms and 7 556
            { 0, 200 },
            { 1, 7 }
        };
        GameObject.FindGameObjectWithTag("Weapon").GetComponent<Weapon>().PopulateArsenal(inv);
        GameManager.instance.SetupAmmo(inv);

        StartCoroutine(PlayTutorialShooter());
    }

    
    /// <summary>
    /// Coroutine for the shooter phase tutorial.
    /// </summary>
    private IEnumerator PlayTutorialShooter()
    {
        cont = false;
        yield return null;
        yield return null;

        // Tell the player how to look around
        teachLooking.SetActive(true);
        GameManager.instance.Fader(true, 0.69f);
        yield return waitOneSecond;
        yield return waitHalfSecond;
        yield return waitForContinue;
        Destroy(teachLooking);
        cont = false;
        yield return waitHalfSecond;

        // Tell the player how to shoot
        GameManager.instance.EndTouching();
        teachShooting.SetActive(true);
        yield return waitOneSecond;
        yield return waitForContinue;
        Destroy(teachShooting);
        cont = false;
        yield return waitHalfSecond;

        // Tell the player how to swap ammo
        GameManager.instance.EndTouching();
        teachSwapping.SetActive(true);
        yield return waitOneSecond;
        yield return waitHalfSecond;
        yield return waitForContinue;
        Destroy(teachSwapping);
        cont = false;
        yield return waitOneSecond;

        // Tell the player to shoot enemies
        GameObject.FindGameObjectWithTag("Spawner").GetComponent<EnemySpawner>().SpawnEnemiesTutorial();
        encourageShooting.SetActive(true);
    }


    private void EndTutorial()
    {
        StartCoroutine(EndTutorialRoutine());
    }
    private IEnumerator EndTutorialRoutine()
    {
        Destroy(encourageShooting);

        // Complete the tutorial
        GameManager.instance.EndTouching();
        GameManager.instance.BlockUserInputs();
        GameManager.instance.FadeCurrentMusicOut(1f);

        // Show the congrats screen
        yield return null;
        yield return null;
        GameManager.instance.ToggleCurrencyLabels();
        yield return waitOneSecond;
        GameManager.instance.UnblockUserInputs();

        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == "Hole");

        // End tutorial!
        Destroy(gameObject);
    }

    
    /* Button Events */
    public void ContinueButtonClicked()
    {
        cont = true;
    }
    public void SkipButtonClicked()
    {
        skip = true;
        StopAllCoroutines();
    }

    /* State Checks */

    // Continue button basically
    private bool CanContinue() { return cont; }

    // Checks if everything has been collected
    private bool AllCollected() { return GameObject.FindWithTag("1") is null && GameObject.FindWithTag("0") is null; }
}
