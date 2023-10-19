using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SettingsMenu : MonoBehaviour
{
    private Coroutine anyRoutine = null;

    [SerializeField] private Image musicButton;
    [SerializeField] private Image soundButton;
    [SerializeField] private Image leftHandButton;
    [SerializeField] private Image oneHandButton;
    [SerializeField] private Sprite toggleOn;
    [SerializeField] private Sprite toggleOff;
    [SerializeField] private Slider sensSlider;
    [SerializeField] private TextMeshProUGUI sensLabel;


    private void Start()
    {
        // Setup camera sensitivity
        float sens = SaveData.current.gameSettings.cameraSens / 4.69f;
        sensSlider.value = sens;
        sensLabel.text = sens.ToString("0.0");

        // Setup music
        if (SaveData.current.gameSettings.isMusicOn)
            musicButton.sprite = toggleOn;

        // Setup sfx
        if (SaveData.current.gameSettings.isSFXOn)
            soundButton.sprite = toggleOn;

        // Setup one handed
        if (SaveData.current.gameSettings.oneHandedMode)
            oneHandButton.sprite = toggleOn;

        // Setup left handed
        if (SaveData.current.gameSettings.isRightHanded)
            leftHandButton.sprite = toggleOff;

    }

    private void OnDestroy()
    {
        if (anyRoutine != null)
            StopCoroutine(anyRoutine);
    }

    public void CloseSettings()
    {
        GameManager.instance.SaveFileToDisk();
        GameManager.instance.CloseSettingsMenu();
        GetComponent<Animator>().SetTrigger("SlideOut");
    }


    /// <summary>
    /// Toggles the music on/off.
    /// </summary>
    public void ToggleMusic()
    {
        GameManager.instance.ButtonPressSound();

        if (GameManager.instance.ToggleMusic())
        {
            musicButton.sprite = toggleOn;
            GameManager.instance.PlayMusic("Main");
            SaveData.current.gameSettings.isMusicOn = true;
        }
        else
        {
            musicButton.sprite = toggleOff;
            SaveData.current.gameSettings.isMusicOn = false;
        }
    }


    /// <summary>
    /// Toggles the sounds on/off.
    /// </summary>
    public void ToggleSounds()
    {
        GameManager.instance.ButtonPressSound();

        if (GameManager.instance.ToggleSFX())
        {
            soundButton.sprite = toggleOn;
            SaveData.current.gameSettings.isSFXOn = true;
        }
        else
        {
            soundButton.sprite = toggleOff;
            SaveData.current.gameSettings.isSFXOn = false;
        }
    }


    /// <summary>
    /// Toggles the left handed mode on/off.
    /// </summary>
    public void ToggleLeftHanded()
    {
        GameManager.instance.ButtonPressSound();

        if (SaveData.current.gameSettings.isRightHanded)
        {
            leftHandButton.sprite = toggleOn;
            SaveData.current.gameSettings.isRightHanded = false;
        }
        else
        {
            leftHandButton.sprite = toggleOff;
            SaveData.current.gameSettings.isRightHanded = true;
        }

        GameManager.instance.ToggleLeftHanded();
    }


    /// <summary>
    /// Toggles the one handed mode on/off.
    /// </summary>
    public void ToggleOneHanded()
    {
        GameManager.instance.ButtonPressSound();

        if (SaveData.current.gameSettings.oneHandedMode)
        {
            oneHandButton.sprite = toggleOff;
            SaveData.current.gameSettings.oneHandedMode = false;
        }
        else
        {
            oneHandButton.sprite = toggleOn;
            SaveData.current.gameSettings.oneHandedMode = true;
        }

        GameManager.instance.ToggleOneHanded();
    }


    /// <summary>
    /// Change the shooter phase look sensitivity.
    /// </summary>
    public void ChangeCameraSens()
    {
        SaveData.current.gameSettings.cameraSens = 4.69f * sensSlider.value;
        sensLabel.text = sensSlider.value.ToString("0.0");
    }
}