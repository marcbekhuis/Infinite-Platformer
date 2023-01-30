using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiDetection : Singleton<UiDetection>
{
    [SerializeField] private RectTransform checkFrom;
    [SerializeField] private Canvas canvas;
    [SerializeField] private bool debugging = false;

    public bool CheckOnUI(Vector3 position)
    {
        //Vector3 screenPosition = Camera.main.WorldToScreenPoint(position);
        List<RectTransform> rectTransforms = new List<RectTransform> { checkFrom };
        Debug.DrawLine(position, position + Vector3.up, Color.yellow, 10);

        for (int i = 0; i < rectTransforms.Count; i++)
        {
            RectTransform rectTransform = rectTransforms[i];
            if (rectTransform.TryGetComponent<Graphic>(out Graphic graphic))
            {
                if (graphic.raycastTarget)
                {
                    Vector3 pivotPosition = rectTransform.position - (Vector3)(rectTransform.rect.size * canvas.scaleFactor * rectTransform.pivot);
                    Vector3 upperPosition = Vector3.Max(rectTransform.position, pivotPosition);
                    Vector3 lowerPosition = Vector3.Min(rectTransform.position, pivotPosition);

                    if (position.x > lowerPosition.x && position.y > lowerPosition.y)
                        if (position.x < upperPosition.x && position.y < upperPosition.y)
                        {
                            Debug.LogError("Hit: " + rectTransform.name);
                            return true;
                        }
                }
            }

            foreach (RectTransform item in rectTransform)
            {
                if (!item.gameObject.activeSelf)
                    continue;
                rectTransforms.Add(item);
            }
        }
        return false;
    }

    private void OnDrawGizmos()
    {
        if (!debugging)
            return;

        List<RectTransform> rectTransforms = new List<RectTransform> { checkFrom };

        for (int i = 0; i < rectTransforms.Count; i++)
        {
            RectTransform rectTransform = rectTransforms[i];
            if(rectTransform.TryGetComponent<Graphic>(out Graphic graphic))
            {
                if (graphic.raycastTarget)
                {
                    Gizmos.color = Color.red;
                    Vector3 position = rectTransform.position + (Vector3)(rectTransform.rect.size * canvas.scaleFactor * (new Vector2(0.5f, 0.5f) - rectTransform.pivot));
                    Gizmos.DrawWireCube(position, rectTransform.rect.size * canvas.scaleFactor);
                }
            }

            foreach (RectTransform item in rectTransform)
            {
                if (!item.gameObject.activeSelf)
                    continue;
                rectTransforms.Add(item);
            }
        }
    }
}
