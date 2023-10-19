using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SetCrosshair : MonoBehaviour
{
    // References
    [SerializeField] private Image crosshair;

    // Data
    [SerializeField] private Color color;
    [SerializeField] private List<Sprite> crosshairs;

    /// <summary>
    /// 0:Cross(Dot), 1:Cross, 2:Cross(X), 3:Bullseye, 4:Holo, 5:UDot, 6:Dot, 7:Chevron
    /// </summary>
    public int ID { get; set; }
    public Color Color { get { return color; } set { color = value; } }


    /// <summary>
    /// Sets the player's saved crosshair to the given crosshair.
    /// </summary>
    public void SetCrosshairs(SetCrosshair newCrosshair)
    {
        if (newCrosshair != null)
        {
            ID = newCrosshair.ID;
            crosshair.sprite = crosshairs[ID];
            color = newCrosshair.Color;
            crosshair.color = color;
        }
    }


    /// <summary>
    /// Change the color to the given color.
    /// </summary>
    public void SetColor(Color color)
    {
        this.color = color;
        crosshair.color = color;
    }


    /// <summary>
    /// Swap the crosshair sprite to the given sprite ID.
    /// </summary>
    public void ChangeCrosshair(int id)
    {
        ID = id;
        crosshair.sprite = crosshairs[id];
    }
}
