
using UnityEngine;

public class HolographicFishTail : MonoBehaviour
{
    [Header("Tail Animation")]
    [SerializeField] private Transform fishTail;
    [SerializeField] private float tailSpeed = 8f;
    [SerializeField] private float tailSwing = 25f;

    private Quaternion originalRotation;
    private float timeOffset;

    private void Start()
    {
        if (fishTail == null)
        {
            Debug.LogWarning("Fish Tail has not been assigned!");
            enabled = false;
            return;
        }

        originalRotation = fishTail.localRotation;
        timeOffset = Random.Range(0f, Mathf.PI * 2f);
    }

    private void Update()
    {
        float swing = Mathf.Sin(
            Time.time * tailSpeed + timeOffset
        ) * tailSwing;

        fishTail.localRotation =
            originalRotation * Quaternion.Euler(0f, swing, 0f);
    }
}
