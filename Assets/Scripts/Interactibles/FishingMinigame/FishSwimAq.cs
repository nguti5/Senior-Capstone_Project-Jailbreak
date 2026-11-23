
using UnityEngine;

public class HolographicFishSwim : MonoBehaviour
{
    [Header("Swimming Settings")]
    [SerializeField] private float swimRadius = 1.0f;
    [SerializeField] private float swimSpeed = 35f;
    [SerializeField] private float startingAngle = 0f;

    [Header("Floating Settings")]
    [SerializeField] private float swimHeight = 1.8f;
    [SerializeField] private float bobAmount = 0.15f;
    [SerializeField] private float bobSpeed = 2f;

    private float angle;
    private float bobOffset;
    private Vector3 center;

    private void Start()
    {
        angle = startingAngle;
        bobOffset = Random.Range(0f, Mathf.PI * 2f);

        // Aquarium center in world space
        center = transform.parent.position;
    }

    private void Update()
    {
        angle += swimSpeed * Time.deltaTime;

        float radians = angle * Mathf.Deg2Rad;

        float x = Mathf.Cos(radians) * swimRadius;
        float z = Mathf.Sin(radians) * swimRadius;

        float y = swimHeight +
                  Mathf.Sin(Time.time * bobSpeed + bobOffset) * bobAmount;

        Vector3 targetPosition = center + new Vector3(x, y, z);

        // Tangent direction of the circular path
        Vector3 swimDirection = new Vector3(
            -Mathf.Sin(radians),
            0f,
            Mathf.Cos(radians)
        );

        transform.position = targetPosition;

        // Our fish model faces local +X
        transform.rotation = Quaternion.LookRotation(swimDirection) *
                             Quaternion.Euler(0f, -90f, 0f);
    }
}
