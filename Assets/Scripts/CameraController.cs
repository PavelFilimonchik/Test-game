using UnityEngine;

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
        float h = Input.GetAxisRaw("Horizontal"); // A/D или ←/→
        float v = Input.GetAxisRaw("Vertical");   // W/S или ↑/↓

        Vector3 move = new Vector3(h, v, 0) * MoveSpeed * Time.deltaTime;
        transform.position += move;
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        cam.orthographicSize -= scroll * ZoomSpeed;
        cam.orthographicSize = Mathf.Clamp(cam.orthographicSize, MinSize, MaxSize);
    }

    void ClampPosition()
    {
        float x = Mathf.Clamp(transform.position.x, 0f, 9f);
        float y = Mathf.Clamp(transform.position.y, 0f, 9f);
        transform.position = new Vector3(x, y, transform.position.z);
    }
}
