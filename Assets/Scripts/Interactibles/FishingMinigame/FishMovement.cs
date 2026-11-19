
using UnityEngine;

public class FishMovement : MonoBehaviour
{
    [Header("Swimming Settings")]
    [SerializeField] private float swimSpeed = 150f;

    [Header("Screen Boundaries")]
    [SerializeField] private float leftBoundary = -800f;
    [SerializeField] private float rightBoundary = 800f;

    private RectTransform fishRect;
    private FishTarget fishTarget;
    private FishingGameManager gameManager;

    private void Awake()
    {
        fishRect = GetComponent<RectTransform>();
        fishTarget = GetComponent<FishTarget>();
    }

    public void Setup(FishingGameManager manager)
    {
        gameManager = manager;
    }

    private void Update()
    {
        if (fishRect == null || gameManager == null)
            return;

        if (!gameManager.IsGameActive)
            return;

        // Swim from right to left.
        fishRect.anchoredPosition +=
            Vector2.left * swimSpeed * Time.deltaTime;

        // Check if fish has left the screen.
        if (fishRect.anchoredPosition.x < leftBoundary)
        {
            // Lose a life if a phishing fish escapes.
            if (fishTarget != null && fishTarget.IsPhishingFish)
            {
                gameManager.MissedPhishingFish();
            }

            Destroy(gameObject);
        }
    }

    public void SetSwimSpeed(float speed)
    {
        swimSpeed = speed;
    }
}
