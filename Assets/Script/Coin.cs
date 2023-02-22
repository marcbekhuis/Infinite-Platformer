using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 50;

    private void Update()
    {
        this.transform.Rotate(new Vector3(0,rotationSpeed *Time.deltaTime,0));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.TryGetComponent<Player>(out Player player))
        {
            PointManager.instance.points++;
            Destroy(this.gameObject);
        }
    }
}
