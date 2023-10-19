using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomizeMenu : MonoBehaviour
{
    private Coroutine anyRoutine = null;

    [Header("Skins Pages")]
    [SerializeField] private List<GameObject> skinPages;
    private int activeSkinPageIndex;

    [Header("Crosshair Customization")]
    [SerializeField] private Image crosshair;
    [SerializeField] private Slider redSlider;
    [SerializeField] private Slider greenSlider;
    [SerializeField] private Slider blueSlider;
    [SerializeField] private Slider alphaSlider;
    [SerializeField] private List<Sprite> crosshairs;
    private bool canChangeColor;


    private void Start()
    {
        transform.SetSiblingIndex(transform.parent.childCount - 2);

        // Setup crosshair
        crosshair.sprite = crosshairs[SaveData.current.gameSettings.crosshairID];
        crosshair.color = SaveData.current.gameSettings.GetColor();
        canChangeColor = false;
        redSlider.value = crosshair.color.r;
        greenSlider.value = crosshair.color.g;
        blueSlider.value = crosshair.color.b;
        alphaSlider.value = crosshair.color.a;
        canChangeColor = true;

        // Default to basic skins page
        activeSkinPageIndex = 0;
        skinPages[0].SetActive(true);
        for (int i = 1; i < skinPages.Count; i++)
            skinPages[i].SetActive(false);
    }

    private void OnDestroy()
    {
        if (anyRoutine != null)
            StopCoroutine(anyRoutine);
    }

    public void CloseCustomize()
    {
        SaveData.current.gameSettings.SetColor(crosshair.color);

        GameManager.instance.SaveFileToDisk();
        GameManager.instance.CloseCustomizationMenu();
        GetComponent<Animator>().SetTrigger("SlideOut");
    }


    /// <summary>
    /// Change the color of the crosshair based on the sliders.
    /// </summary>
    public void ChangeColor()
    {
        if (canChangeColor)
            crosshair.color = new Color(redSlider.value, greenSlider.value, blueSlider.value, alphaSlider.value);
    }


    /// <summary>
    /// Change the crosshair to the given crosshair ID.
    /// </summary>
    public void ChangeCrosshair(int id)
    {
        GameManager.instance.ButtonPressSound();
        crosshair.sprite = crosshairs[id];
        SaveData.current.gameSettings.crosshairID = id;
    }


    /// <summary>
    /// Change to the selected page of skins.
    /// </summary>
    /// <param name="index">Index of the page</param>
    public void SwapToSkinPage(int index)
    {
        GameManager.instance.ButtonPressSound();
        skinPages[activeSkinPageIndex].SetActive(false);
        activeSkinPageIndex = index;
        skinPages[activeSkinPageIndex].SetActive(true);
    }
}