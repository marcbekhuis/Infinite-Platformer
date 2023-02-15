using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GhostMovement : MonoBehaviour
{
    [SerializeField] private float speed;
    [SerializeField] private Transform target;

    private Vector3 startPosition;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.instance.OnMoveMap += OnMoveMap;
        GameManager.instance.OnResetGame += ResetGhost;
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

        transform.LookAt(target);
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnMoveMap(Vector3 offset)
    {
        transform.position += offset;
    }
}
