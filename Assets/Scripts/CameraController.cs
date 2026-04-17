using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public float MoveSpeed = 10f;
    public float ZoomSpeed = 5f;
    public float MinSize = 3f;
    public float MaxSize = 10f;

    private Camera cam;

    void Start()
    {
        cam = GetComponent<Camera>();
    }

    void Update()
    {
        HandleMovement();
        HandleZoom();
        ClampPosition();
    }

    void HandleMovement()
    {
        float h = 0f;
        float v = 0f;

        var kb = Keyboard.current;
        if (kb == null) return;

        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed)  h = -1f;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) h =  1f;
        if (kb.sKey.isPressed || kb.downArrowKey.isPressed)  v = -1f;
        if (kb.wKey.isPressed || kb.upArrowKey.isPressed)    v =  1f;

        transform.position += new Vector3(h, v, 0) * MoveSpeed * Time.deltaTime;
    }

    void HandleZoom()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        float scroll = mouse.scroll.ReadValue().y;
        cam.orthographicSize -= scroll * ZoomSpeed * 0.05f;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, MinSize, MaxSize);
    }

    void ClampPosition()
    {
        float x = Mathf.Clamp(transform.position.x, 0f, 9f);
        float y = Mathf.Clamp(transform.position.y, 0f, 9f);
        transform.position = new Vector3(x, y, transform.position.z);
    }
}
