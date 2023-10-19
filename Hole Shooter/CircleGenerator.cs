using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleGenerator : MonoBehaviour
{
    public int points;
    public float radius;
    public GameObject pointPrefab;
    public float yPos;


    [ContextMenu("Generate Circle!")]
    void DrawCirclePoints()
    {
        float num = Mathf.PI / (points / 2);
        for (int i = 0; i < points; i++)
        {
            float angle = i * num;
            float newX = radius * Mathf.Cos(angle);
            float newY = radius * Mathf.Sin(angle);

            Instantiate(pointPrefab, new Vector3(newX, yPos, newY), Quaternion.identity);
        }
    }
}
