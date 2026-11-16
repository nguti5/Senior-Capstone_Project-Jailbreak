using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float Walkspeed = 5.5f;
    [SerializeField] private float RunSpeed = 9.0f;
    [SerializeField] private float GravityForce = 20.0f;
    [SerializeField] private float LookSensitivity = 0.2f;
    [SerializeField] private float PlayerCameraClamp = 90.0f;

    private Camera mainCamera;
    private CharacterController characterController;
    private InputAction moveInput;
    private InputAction runInput;

    private AudioSource footsteps;
    private AudioClip walkingClip;
    private AudioClip runningClip;

    private float verticalSpeed;
    private float lookAngle;

    private void Start()
    {
        mainCamera = GetComponentInChildren<Camera>();
        characterController = GetComponent<CharacterController>();
        moveInput = InputSystem.actions.FindAction("Move");
        runInput = InputSystem.actions.FindAction("Sprint");

        walkingClip = Resources.Load<AudioClip>("Audio/walking");
        runningClip = Resources.Load<AudioClip>("Audio/running");

        footsteps = gameObject.AddComponent<AudioSource>();
        footsteps.playOnAwake = false;
        footsteps.loop = true;
        footsteps.spatialBlend = 0f;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Time.timeScale == 0f)
        {
            UpdateFootsteps(null);
            return;
        }

        Vector2 moveInputValue = moveInput.ReadValue<Vector2>();
        bool sprinting = runInput.IsPressed();
        bool moving = HandleMovement(moveInputValue, sprinting ? RunSpeed : Walkspeed);

        AudioClip movementClip = sprinting ? runningClip : walkingClip;
        UpdateFootsteps(moving && characterController.isGrounded ? movementClip : null);

        if (Mouse.current != null)
            HandleMouse(Mouse.current.delta.ReadValue());
    }

    private void OnDisable()
    {
        if (footsteps != null)
            footsteps.Stop();
    }

    private bool HandleMovement(Vector2 input, float speed)
    {
        float frameTime = Time.deltaTime;
        verticalSpeed = characterController.isGrounded ? 0f : verticalSpeed;
        verticalSpeed -= GravityForce * frameTime;

        Vector3 movement =
            (transform.forward * input.y + transform.right * input.x) * speed;
        movement.y = verticalSpeed;

        Vector3 previousPosition = transform.position;
        characterController.Move(movement * frameTime);

        Vector3 displacement = transform.position - previousPosition;
        displacement.y = 0f;
        return input.sqrMagnitude > 0f && displacement.sqrMagnitude > 0.00000001f;
    }

    private void UpdateFootsteps(AudioClip clip)
    {
        if (clip == null)
        {
            if (footsteps.isPlaying)
                footsteps.Stop();
            return;
        }

        if (footsteps.clip == clip && footsteps.isPlaying)
            return;

        footsteps.Stop();
        footsteps.clip = clip;
        footsteps.Play();
    }

    private void HandleMouse(Vector2 mouseDelta)
    {
        lookAngle -= mouseDelta.y * LookSensitivity;
        lookAngle = Mathf.Clamp(lookAngle, -PlayerCameraClamp, PlayerCameraClamp);
        mainCamera.transform.localRotation = Quaternion.Euler(lookAngle, 0, 0);
        transform.rotation *= Quaternion.Euler(0, mouseDelta.x * LookSensitivity, 0);
    }
}