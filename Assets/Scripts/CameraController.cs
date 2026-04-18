using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    public float MoveSpeed   = 10f;
    public float RotateSpeed = 120f;
    public float ZoomSpeed   = 4f;
    public float MinDistance = 4f;
    public float MaxDistance = 35f;

    private float   distance   = 20f;
    private float   pitch      = 50f;
    private float   yaw        = 45f;
    private Vector3 focusPoint = new Vector3(16f, 0f, 16f);
    private Vector2 prevMouse;

    void Start() => ApplyTransform();

    void Update()
    {
        MoveCamera();
        RotateCamera();
        ZoomCamera();
        focusPoint.x = Mathf.Clamp(focusPoint.x, 0f, 31f);
        focusPoint.z = Mathf.Clamp(focusPoint.z, 0f, 31f);
        focusPoint.y = 0f;
        ApplyTransform();
    }

    void MoveCamera()
    {
        var kb = Keyboard.current;
        if (kb == null) return;

        float h = 0f, v = 0f;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed)  h = -1f;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) h =  1f;
        if (kb.sKey.isPressed || kb.downArrowKey.isPressed)  v = -1f;
        if (kb.wKey.isPressed || kb.upArrowKey.isPressed)    v =  1f;

        float rad   = yaw * Mathf.Deg2Rad;
        Vector3 fwd   = new Vector3( Mathf.Sin(rad), 0f,  Mathf.Cos(rad));
        Vector3 right = new Vector3( Mathf.Cos(rad), 0f, -Mathf.Sin(rad));
        focusPoint += (fwd * v + right * h) * MoveSpeed * Time.deltaTime;
    }

    void RotateCamera()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        if (mouse.rightButton.wasPressedThisFrame)
            prevMouse = mouse.position.ReadValue();

        if (mouse.rightButton.isPressed)
        {
            Vector2 delta = (Vector2)mouse.position.ReadValue() - prevMouse;
            prevMouse = mouse.position.ReadValue();
            yaw   += delta.x * RotateSpeed * Time.deltaTime;
            pitch -= delta.y * RotateSpeed * Time.deltaTime;
            pitch  = Mathf.Clamp(pitch, 15f, 80f);
        }
    }

    void ZoomCamera()
    {
        var mouse = Mouse.current;
        if (mouse == null) return;

        float scroll = mouse.scroll.ReadValue().y;
        distance -= scroll * ZoomSpeed * 0.05f;
        distance  = Mathf.Clamp(distance, MinDistance, MaxDistance);
    }

    void ApplyTransform()
    {
        float pRad = pitch * Mathf.Deg2Rad;
        float yRad = yaw   * Mathf.Deg2Rad;

        Vector3 offset = new Vector3(
            distance * Mathf.Cos(pRad) * Mathf.Sin(yRad),
            distance * Mathf.Sin(pRad),
            distance * Mathf.Cos(pRad) * Mathf.Cos(yRad)
        );

        transform.position = focusPoint + offset;
        transform.LookAt(focusPoint);
    }
}
