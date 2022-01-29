using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Water : MonoBehaviour
{
    private BoxCollider collider;
    private float surfaceHeight = 0;

    private void Start()
    {
        collider = GetComponent<BoxCollider>();
        surfaceHeight = transform.localScale.y * collider.size.y * 0.5f + transform.position.y;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
        {
            float heightSpeed = surfaceHeight - rigidbody.worldCenterOfMass.y;
            heightSpeed = Mathf.Clamp(heightSpeed, 0, float.MaxValue) * Time.deltaTime * 25;
            float velocityBreak = Mathf.Clamp(-rigidbody.velocity.y, 0, float.MaxValue) * Time.deltaTime * 5;
            rigidbody.AddForce(Vector3.up * (heightSpeed + velocityBreak), ForceMode.Impulse);
        }
        else if (other.transform.parent.TryGetComponent<Rigidbody>(out rigidbody))
        {
            float heightSpeed = surfaceHeight - rigidbody.worldCenterOfMass.y;
            heightSpeed = Mathf.Clamp(heightSpeed, 0, float.MaxValue) * Time.deltaTime * 25;
            float velocityBreak = Mathf.Clamp(-rigidbody.velocity.y, 0, float.MaxValue) * Time.deltaTime * 5;
            rigidbody.AddForce(Vector3.up * (heightSpeed + velocityBreak), ForceMode.Impulse);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.parent.TryGetComponent<Player>(out Player player))
            player.InWater = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.transform.parent.TryGetComponent<Player>(out Player player))
            player.InWater = false;
    }
}
