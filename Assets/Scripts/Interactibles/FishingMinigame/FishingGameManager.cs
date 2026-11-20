
using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class FishingGameManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text livesText;
    [SerializeField] private TMP_Text startText;

    [Header("Game Settings")]
    [SerializeField] private int startingLives = 3;
    [SerializeField] private int winningScore = 10;

    [Header("Level Rewards")]
    [SerializeField] private GameObject ceilingArrow;
    [Header("Victory Notification")]
    [SerializeField] private GameObject ceilingHintPopup;
    private int score;
    private int lives;
    private bool gameStarted;
    private bool gameWon;

    public bool IsGameActive => gameStarted && lives > 0 && !gameWon;

    private void OnEnable()
    {
        score = 0;
        lives = startingLives;
        gameStarted = false;
        gameWon = false;

        UpdateUI();

        if (startText != null)
        {
            startText.text = "PRESS SPACE TO START FISHING";
            startText.gameObject.SetActive(true);
        }
    }

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (!gameStarted && !gameWon &&
            Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            StartFishing();
        }
    }

    private void StartFishing()
    {
        score = 0;
        lives = startingLives;
        gameStarted = true;
        gameWon = false;

        UpdateUI();

        if (startText != null)
            startText.gameObject.SetActive(false);

        Debug.Log("PhisherMen started!");
    }

    public void CatchFish(bool isPhishingFish)
    {
        if (!IsGameActive)
            return;

        if (isPhishingFish)
        {
            score++;

            Debug.Log("Correct! Score: " + score);

            if (score >= winningScore)
            {
                WinGame();
            }
        }
        else
        {
            LoseLife();
        }

        UpdateUI();
    }
    private void Awake()
    {
        if (ceilingArrow != null)
            ceilingArrow.SetActive(false);
    }

    public void MissedPhishingFish()
    {
        if (!IsGameActive)
            return;

        Debug.Log("A phishing fish escaped!");
        LoseLife();
    }

    private void LoseLife()
    {
        lives--;

        UpdateUI();

        Debug.Log("Lives remaining: " + lives);

        if (lives <= 0)
        {
            gameStarted = false;

            if (startText != null)
            {
                startText.text =
                    "GAME OVER!\nPRESS SPACE TO RESTART";

                startText.gameObject.SetActive(true);
            }

            Debug.Log("GAME OVER!");
        }
    }

    private void WinGame()
    {
        gameWon = true;
        gameStarted = false;

        if (startText != null)
        {
            startText.text =
                "YOU WIN!\nPHISHING THREATS ELIMINATED!\nPRESS X TO EXIT";

            startText.gameObject.SetActive(true);
        }

        if (ceilingArrow != null)
        {
            ceilingArrow.SetActive(true);
            Debug.Log("Ceiling arrow activated!");
        }
        if (ceilingHintPopup != null)
        {
            ceilingHintPopup.SetActive(true);
            StartCoroutine(HideCeilingHintAfterDelay());
        }
        Debug.Log("PHISHERMEN COMPLETED! Final Score: " + score);
    }

    private void UpdateUI()
    {
        if (scoreText != null)
            scoreText.text = "SCORE: " + score;

        if (livesText != null)
            livesText.text = "LIVES: " + lives;
    }
    private IEnumerator HideCeilingHintAfterDelay()
    {
        yield return new WaitForSecondsRealtime(7f);

        if (ceilingHintPopup != null)
        {
            ceilingHintPopup.SetActive(false);
        }
    }
}
