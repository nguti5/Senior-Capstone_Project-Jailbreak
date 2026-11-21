
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SceneTransitionManager : MonoBehaviour
{
    public static SceneTransitionManager Instance { get; private set; }

    [Header("UI References")]
    [SerializeField] private CanvasGroup transitionCanvasGroup;
    [SerializeField] private Image transitionBackground;
    [SerializeField] private TMP_Text transitionText;

    [Header("Transition Settings")]
    [SerializeField] private float fadeDuration = 0.6f;
    [SerializeField] private float messageDuration = 0.8f;

    [Header("Colors")]
    [SerializeField]
    private Color levelColor =
        new Color(0f, 0.12f, 0.13f, 1f);

    [SerializeField]
    private Color battleColor =
        new Color(0.3f, 0f, 0.04f, 1f);

    [Header("Battle Glitch Effect")]
    [SerializeField] private float glitchDuration = 1.5f;
    [SerializeField] private float glitchInterval = 0.09f;
    [SerializeField] private float textShakeAmount = 12f;
    [SerializeField, Range(0f, 1f)]
    private float flashIntensity = 0.7f;

    private bool isTransitioning;
    public bool IsTransitioning => isTransitioning;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        if (transitionCanvasGroup == null ||
            transitionBackground == null ||
            transitionText == null)
        {
            Debug.LogError("Scene transition UI references are missing!");
            enabled = false;
            return;
        }

        transitionCanvasGroup.alpha = 0f;
        transitionCanvasGroup.blocksRaycasts = false;
    }

    public void BeginTransition(string sceneName)
    {
        if (isTransitioning)
            return;

        StartCoroutine(TransitionRoutine(sceneName));
    }

    public void BeginBattleTransition(Action startBattle)
    {
        if (isTransitioning || startBattle == null)
            return;

        StartCoroutine(BattleTransitionRoutine(startBattle));
    }

    private IEnumerator TransitionRoutine(string sceneName)
    {
        isTransitioning = true;

        bool isBattle =
            SceneTransition.PendingTransitionType == TransitionType.Battle;

        transitionBackground.color =
            isBattle ? battleColor : levelColor;

        transitionText.text =
            isBattle ? "THREAT DETECTED" : "ACCESS GRANTED";

        transitionText.color =
            isBattle ? Color.red : Color.cyan;

        transitionCanvasGroup.blocksRaycasts = true;

        yield return Fade(0f, 1f);
        yield return new WaitForSecondsRealtime(messageDuration);

        SceneManager.LoadScene(sceneName);
        yield return null;

        yield return Fade(1f, 0f);

        transitionCanvasGroup.blocksRaycasts = false;
        isTransitioning = false;
    }

    private IEnumerator BattleTransitionRoutine(Action startBattle)
    {
        isTransitioning = true;

        transitionCanvasGroup.blocksRaycasts = true;

        transitionText.text = "THREAT DETECTED";
        transitionText.color = Color.red;

        RectTransform textRect =
            transitionText.rectTransform;

        Vector2 originalPosition =
            textRect.anchoredPosition;

        // Start with a short red fade.
        transitionBackground.color = battleColor;
        yield return Fade(0f, 1f);

        // Rapid red glitch sequence.
        float elapsed = 0f;

        while (elapsed < glitchDuration)
        {
            float brightness = UnityEngine.Random.Range(
                0.25f,
                0.25f + flashIntensity
            );

            transitionBackground.color = new Color(
                brightness,
                0f,
                brightness * 0.12f,
                1f
            );

            // Occasionally corrupt the warning text.
            int glitch = UnityEngine.Random.Range(0, 5);

            switch (glitch)
            {
                case 0:
                    transitionText.text = "THR3AT D3T3CTED";
                    break;
                case 1:
                    transitionText.text = "!! SYSTEM BREACH !!";
                    break;
                case 2:
                    transitionText.text = "THREAT_DETECTED";
                    break;
                default:
                    transitionText.text = "THREAT DETECTED";
                    break;
            }

            // Shake the text around its original position.
            textRect.anchoredPosition =
                originalPosition +
                new Vector2(
                    UnityEngine.Random.Range(
                        -textShakeAmount,
                        textShakeAmount
                    ),
                    UnityEngine.Random.Range(
                        -textShakeAmount,
                        textShakeAmount
                    )
                );

            yield return new WaitForSecondsRealtime(
                Mathf.Max(0.04f, glitchInterval)
            );

            elapsed += Mathf.Max(0.04f, glitchInterval);
        }

        // Stabilize before loading the battle.
        textRect.anchoredPosition = originalPosition;
        transitionText.text = "THREAT DETECTED";
        transitionBackground.color = battleColor;

        yield return new WaitForSecondsRealtime(0.3f);

        startBattle();

        // Wait for the additive battle scene to activate.
        float timeout = 10f;

        while (SceneManager.GetActiveScene().name != "BattleScene1"
               && timeout > 0f)
        {
            timeout -= Time.unscaledDeltaTime;
            yield return null;
        }

        if (timeout <= 0f)
            Debug.LogWarning("Battle scene activation timed out.");

        yield return Fade(1f, 0f);

        transitionCanvasGroup.blocksRaycasts = false;
        isTransitioning = false;
    }

    private IEnumerator Fade(float start, float end)
    {
        float elapsed = 0f;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.unscaledDeltaTime;

            float progress =
                Mathf.Clamp01(
                    elapsed / Mathf.Max(0.01f, fadeDuration)
                );

            transitionCanvasGroup.alpha =
                Mathf.Lerp(start, end, progress);

            yield return null;
        }

        transitionCanvasGroup.alpha = end;
    }
}
