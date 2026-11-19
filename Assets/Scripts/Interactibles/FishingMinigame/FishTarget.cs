
using UnityEngine;
using UnityEngine.EventSystems;

public class FishTarget : MonoBehaviour, IPointerClickHandler
{
    [Header("Fish Settings")]
    [SerializeField] private bool isPhishingFish = true;

    [Header("Game Manager")]
    [SerializeField] private FishingGameManager gameManager;

    public bool IsPhishingFish => isPhishingFish;

    public void Setup(bool phishing, FishingGameManager manager)
    {
        isPhishingFish = phishing;
        gameManager = manager;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (gameManager == null || !gameManager.IsGameActive)
            return;

        gameManager.CatchFish(isPhishingFish);

        Destroy(gameObject);
    }
}
