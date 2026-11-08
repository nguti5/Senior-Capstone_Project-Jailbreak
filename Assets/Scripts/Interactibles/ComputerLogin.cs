
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class ComputerLogin : MonoBehaviour
{
    [Header("Login Settings")]
    [SerializeField] private string correctPassword = "Peanut";

    [Header("UI References")]
    [SerializeField] private TMP_InputField passwordInput;
    [SerializeField] private GameObject errorText;

    public void AttemptLogin()
    {
        if (passwordInput == null)
        {
            Debug.LogError("Password input field is missing!");
            return;
        }

        string enteredPassword = passwordInput.text;

        if (enteredPassword == correctPassword)
        {
            Debug.Log("Correct password! Tutorial completed. Loading Level 1...");

            if (errorText != null)
                errorText.SetActive(false);

            // Restore normal cursor state before transitioning
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;

            // Load Level 1
            SceneManager.LoadScene("Battle-Tutorial");
        }
        else
        {
            Debug.Log("Incorrect password.");

            // Clear the password without triggering On Value Changed
            passwordInput.SetTextWithoutNotify("");

            // Display the error message
            if (errorText != null)
                errorText.SetActive(true);

            // Allow the player to try again
            passwordInput.Select();
            passwordInput.ActivateInputField();
        }
    }
}
