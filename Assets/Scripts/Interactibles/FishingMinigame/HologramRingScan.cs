
using UnityEngine;

public class HologramScanRing : MonoBehaviour
{
    [Header("Scanning Settings")]
    [SerializeField] private float bottomHeight = 0.5f;
    [SerializeField] private float topHeight = 4.0f;
    [SerializeField] private float scanSpeed = 0.7f;

    private float currentHeight;

    private void Start()
    {
        currentHeight = bottomHeight;
    }

    private void Update()
    {
        currentHeight += scanSpeed * Time.deltaTime;

        if (currentHeight > topHeight)
        {
            currentHeight = bottomHeight;
        }

        Vector3 position = transform.localPosition;
        position.y = currentHeight;
        transform.localPosition = position;
    }
}
