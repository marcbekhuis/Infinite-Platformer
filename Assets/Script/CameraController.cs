using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : Singleton<CameraController>
{
    [SerializeField] private AnimationCurve speedX;
    [SerializeField] private AnimationCurve speedY;
    [SerializeField] private AnimationCurve speedZ;

    private Vector3 destination;
    private float animationTime = 0;

    private void FixedUpdate()
    {
        Vector3 difference = destination - transform.position;
        Vector3 direction = difference.normalized;
        Vector3 movement = new Vector3(Clamp(direction.x * speedX.Evaluate(animationTime) * Time.deltaTime, difference.x),
            Clamp(direction.y * speedY.Evaluate(animationTime) * Time.deltaTime, difference.y),
            Clamp(direction.z * speedZ.Evaluate(animationTime) * Time.deltaTime, difference.z));
        transform.Translate(movement);
        animationTime += Time.deltaTime;
    }

    public void SetDestination(Vector3 position)
    {
        if (destination == position)
            return;
        destination = position;
        animationTime = 0;
    }

    private float Clamp(float value, float clamp)
    {
        float posClamp = Mathf.Abs(clamp);
        if (value > posClamp)
            return posClamp;
        if (value < -posClamp)
            return -posClamp;
        return value;
    }
}
