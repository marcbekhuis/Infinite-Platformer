using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class SafeAreaHelper : MonoBehaviour
{
    [SerializeField] private Canvas canvas;

    private RectTransform rectTransform; 

    private Rect oldSafeArea;

    private void Update()
    {
        if(canvas)
            if (oldSafeArea != Screen.safeArea)
                CalculateSafeArea();
    }

    private void UpdateSafeArea(Vector2 anchorMin, Vector2 anchorMax)
    {
        if (!rectTransform)
            rectTransform = GetComponent<RectTransform>();

        rectTransform.anchorMin = anchorMin;
        rectTransform.anchorMax = anchorMax;
    }

    private void CalculateSafeArea()
    {
        Rect safeArea = Screen.safeArea;
        Vector2 anchorMin = safeArea.position;
        Vector2 anchorMax = safeArea.position + safeArea.size;
        anchorMin.x /= canvas.pixelRect.width;
        anchorMin.y /= canvas.pixelRect.height;
        anchorMax.x /= canvas.pixelRect.width;
        anchorMax.y /= canvas.pixelRect.height;
        UpdateSafeArea(anchorMin, anchorMax);

        oldSafeArea = safeArea;
    }
}
