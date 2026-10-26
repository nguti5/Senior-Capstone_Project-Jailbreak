
using System.Collections;
using UnityEngine;

public class PhishingButtonInteraction : MonoBehaviour, IInteractible
{
    [SerializeField] private Transform redButton;
    [SerializeField] private float pressDepth = 0.08f;
    [SerializeField] private float pressDuration = 0.12f;

    [Header("Warning Lights")]
    [SerializeField] private Light warningLightLeft;
    [SerializeField] private Light warningLightRight;
    [SerializeField] private float flashInterval = 0.2f;
    [SerializeField] private int flashCount = 8;

    private Vector3 originalPosition;
    private bool isAnimating;
    private bool isFlashing;

    private void Start()
    {
        if (redButton != null)
            originalPosition = redButton.localPosition;

        SetLights(false, false);
    }

    public void Interact(GameObject interactor)
    {
        if (isAnimating || isFlashing || redButton == null)
            return;

        StartCoroutine(AnimateButton());
        StartCoroutine(FlashWarningLights());
    }

    private IEnumerator AnimateButton()
    {
        isAnimating = true;

        Vector3 pressedPosition =
            originalPosition + Vector3.down * pressDepth;

        yield return MoveButton(originalPosition, pressedPosition);
        yield return new WaitForSeconds(0.15f);
        yield return MoveButton(pressedPosition, originalPosition);

        Debug.Log("PHISHING BUTTON PRESSED! Suspicious link activated.");

        isAnimating = false;
    }

    private IEnumerator MoveButton(Vector3 start, Vector3 end)
    {
        float elapsed = 0f;

        while (elapsed < pressDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / pressDuration);

            redButton.localPosition = Vector3.Lerp(start, end, t);
            yield return null;
        }

        redButton.localPosition = end;
    }

    private IEnumerator FlashWarningLights()
    {
        isFlashing = true;

        for (int i = 0; i < flashCount; i++)
        {
            SetLights(true, false);
            yield return new WaitForSeconds(flashInterval);

            SetLights(false, true);
            yield return new WaitForSeconds(flashInterval);
        }

        SetLights(false, false);
        isFlashing = false;
    }

    private void SetLights(bool left, bool right)
    {
        if (warningLightLeft != null)
            warningLightLeft.enabled = left;

        if (warningLightRight != null)
            warningLightRight.enabled = right;
    }

    private void OnDisable()
    {
        SetLights(false, false);
    }
}
