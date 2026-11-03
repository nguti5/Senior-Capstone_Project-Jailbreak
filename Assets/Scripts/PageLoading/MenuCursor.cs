using UnityEngine;

public class MenuCursor : MonoBehaviour
{
    void OnEnable() => Apply();

    void Update()
    {
        if (Cursor.lockState != CursorLockMode.None || !Cursor.visible)
            Apply();
    }

    static void Apply()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}