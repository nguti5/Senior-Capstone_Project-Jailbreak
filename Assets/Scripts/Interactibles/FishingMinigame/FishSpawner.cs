
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FishSpawner : MonoBehaviour
{
    [Header("Fish Prefab")]
    [SerializeField] private GameObject fishPrefab;

    [Header("Game Manager")]
    [SerializeField] private FishingGameManager gameManager;

    [Header("Spawning Settings")]
    [SerializeField] private float spawnInterval = 3f;
    [SerializeField] private float minSwimSpeed = 80f;
    [SerializeField] private float maxSwimSpeed = 180f;

    [Header("Spawn Area")]
    [SerializeField] private float rightSpawnPosition = 800f;
    [SerializeField] private float minY = -180f;
    [SerializeField] private float maxY = 180f;

    [Header("Fish Colors")]
    [SerializeField]
    private Color[] fishColors =
    {
        new Color(0.3f, 0.8f, 1f),
        new Color(0.4f, 1f, 0.5f),
        new Color(0.8f, 0.5f, 1f),
        new Color(1f, 0.6f, 0.3f),
        new Color(1f, 0.4f, 0.7f),
        new Color(1f, 0.9f, 0.3f)
    };

    private float spawnTimer;
    private bool wasGameActive;

    private readonly string[] phishingDomains =
    {
        "paypa1.com",
        "amaz0n.com",
        "micros0ft-login.com",
        "netfix.com",
        "g00gle-security.com",
        "faceboook.com",
        "apple-trust.com",
        "irs-payments.com"
    };

    private readonly string[] legitimateDomains =
    {
        "paypal.com",
        "amazon.com",
        "microsoft.com",
        "netflix.com",
        "google.com",
        "facebook.com",
        "apple.com",
        "irs.gov"
    };

    private void OnEnable()
    {
        spawnTimer = 0f;
        wasGameActive = false;
    }

    private void Update()
    {
        if (gameManager == null || !gameManager.IsGameActive)
        {
            if (wasGameActive)
                ClearFish();

            wasGameActive = false;
            spawnTimer = 0f;
            return;
        }

        wasGameActive = true;
        spawnTimer += Time.deltaTime;

        if (spawnTimer >= spawnInterval)
        {
            spawnTimer = 0f;
            SpawnFish();
        }
    }

    private void SpawnFish()
    {
        if (fishPrefab == null)
        {
            Debug.LogError("Fish prefab is missing!");
            return;
        }

        bool isPhishing = Random.value < 0.5f;

        string[] domains = isPhishing
            ? phishingDomains
            : legitimateDomains;

        string selectedDomain =
            domains[Random.Range(0, domains.Length)];

        GameObject newFish = Instantiate(fishPrefab, transform);
        newFish.SetActive(true);

        RectTransform fishRect =
            newFish.GetComponent<RectTransform>();

        if (fishRect != null)
        {
            fishRect.anchoredPosition = new Vector2(
                rightSpawnPosition,
                Random.Range(minY, maxY)
            );
        }

        FishTarget target = newFish.GetComponent<FishTarget>();

        if (target != null)
            target.Setup(isPhishing, gameManager);

        TMP_Text domainText =
            newFish.GetComponentInChildren<TMP_Text>();

        if (domainText != null)
            domainText.text = selectedDomain;

        FishMovement movement =
            newFish.GetComponent<FishMovement>();

        if (movement != null)
        {
            movement.SetSwimSpeed(
                Random.Range(minSwimSpeed, maxSwimSpeed)
            );

            movement.Setup(gameManager);
        }

        Image fishImage = newFish.GetComponent<Image>();

        if (fishImage != null && fishColors.Length > 0)
        {
            fishImage.color =
                fishColors[Random.Range(0, fishColors.Length)];
        }
    }

    private void ClearFish()
    {
        foreach (FishMovement fish in
                 GetComponentsInChildren<FishMovement>())
        {
            Destroy(fish.gameObject);
        }
    }
}
