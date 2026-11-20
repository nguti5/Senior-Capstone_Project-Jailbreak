using UnityEngine;

public class SpinningFan : MonoBehaviour
{
    [SerializeField] private Vector3 axis = Vector3.up;      // the direction the blades spin around
    [SerializeField] private float degreesPerSecond = 360f;

    private void Update()
    {
        transform.Rotate(axis, degreesPerSecond * Time.deltaTime, Space.Self);
    }
}