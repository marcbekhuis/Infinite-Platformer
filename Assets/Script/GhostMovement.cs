using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Transform target;
    [SerializeField] private Vector3 targetOffset;

    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.OnMoveMap += OnMoveMap;
        GameManager.instance.OnResetGame += ResetGhost;
        GameManager.instance.OnContinueGame += OnContinueGame;
        startPosition = transform.position;
    }

    private void ResetGhost()
    {
        transform.position = startPosition;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.instance.gameEnded || GameManager.instance.gamePaused)
            return;

        transform.LookAt(target.position + targetOffset);
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnMoveMap(Vector3 offset)
    {
        transform.position += offset;
    }

    private void OnContinueGame()
    {
        transform.position = new Vector3(startPosition.x, GameManager.instance.lastSaveHeight, startPosition.z);
    }
}
