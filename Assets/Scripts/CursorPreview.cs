using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CursorPreview : MonoBehaviour
{
    [Header("Software Cursor Setup")]
    public Image customCursorImage;

    private void Start()
    {
        // Hide the actual Windows/Mac hardware cursor
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
    }

    private void LateUpdate()
    {
        // 2. We check if a mouse actually exists, then grab its position the modern way
        if (customCursorImage != null && Mouse.current != null)
        {
            customCursorImage.transform.position = Mouse.current.position.ReadValue();
        }
    }
}