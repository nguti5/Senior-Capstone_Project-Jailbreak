
using UnityEngine;
using UnityEngine.InputSystem;

public class MinigameMouseCrosshair : MonoBehaviour
{
    private RectTransform crosshairRect;
    private RectTransform parentRect;
    private Canvas canvas;

    private void Awake()
    {
        crosshairRect = GetComponent<RectTransform>();
        parentRect = transform.parent as RectTransform;
        canvas = GetComponentInParent<Canvas>();
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = false;
    }

    private void LateUpdate()
    {
        // Keep the normal mouse cursor hidden.
        Cursor.visible = false;

        if (crosshairRect == null || parentRect == null)
            return;

        if (Mouse.current == null)
            return;

        Vector2 mousePosition =
            Mouse.current.position.ReadValue();

        Camera uiCamera = null;

        if (canvas != null &&
            canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            uiCamera = canvas.worldCamera;
        }

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(
            parentRect,
            mousePosition,
            uiCamera,
            out Vector2 localPosition))
        {
            crosshairRect.localPosition = localPosition;
        }
    }

    private void OnDisable()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }
}
