using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Splines;

[ExecuteInEditMode]
public class CoinsSpline : MonoBehaviour
{
    [SerializeField] private bool update = true;
    [SerializeField] private float distanceBetweenCoins = 5;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private SplineContainer splineContainer;

    private float oldLength = 0;
    private float oldDistanceBetween;

    // Update is called once per frame
    void Update()
    {
        float length = splineContainer.Spline.GetLength();

        if (length != oldLength || oldDistanceBetween != distanceBetweenCoins || update) {

            foreach (Transform child in this.transform)
            {
                DestroyImmediate(child.gameObject);
            }

            int amount = Mathf.FloorToInt(length / distanceBetweenCoins);
            float coinDistance = length / amount;

            for (int i = 0; i < amount; i++)
            {
                Vector3 position = splineContainer.EvaluatePosition(1 / length * (coinDistance * i));
                Instantiate(coinPrefab, position, new Quaternion(), this.transform);
            }

            oldLength = length;
            oldDistanceBetween = distanceBetweenCoins;
            update = false;
        }
    }
}
