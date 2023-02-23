using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveableObject : MonoBehaviour
{
    private Vector3 resetPoint;

    private void Start()
    {
        Player.OnSafePoint += OnSafePoint;
        GameManager.instance.OnContinueGame += OnContinueGame;
    }

    private void OnDestroy()
    {
        Player.OnSafePoint -= OnSafePoint;
        GameManager.instance.OnContinueGame -= OnContinueGame;
    }

    private void OnSafePoint()
    {
        resetPoint = transform.localPosition;
    }

    private void OnContinueGame()
    {
        transform.localPosition = resetPoint;
    }
}
