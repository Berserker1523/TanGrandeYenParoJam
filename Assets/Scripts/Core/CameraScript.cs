using UnityEngine;

public class CameraScript : MonoBehaviour
{
    //Movement
    [SerializeField] private float borderMoveSpeed = 1.2f;
    [SerializeField] private float screenOffset = .05f;
    // ZOOM
    [SerializeField] private float zoomSpeed = 4f;
    [SerializeField] private Vector2 zoomLimits;

    private Camera myCam;

    private float zoom;
    private Vector3 speed = new Vector3();

    private void Start()
    {
        myCam = GetComponent<Camera>();
    }

    void Update()
    {
        // Zoom code 
        zoom = Input.GetAxis("Mouse ScrollWheel");
        myCam.orthographicSize -= zoom * zoomSpeed;
        myCam.orthographicSize = Mathf.Clamp(myCam.orthographicSize,
            zoomLimits.x, zoomLimits.y);

        speed = Vector3.zero;

        // Camera movement per border
        if (Input.mousePosition.x < Screen.width * screenOffset)
            speed.x -= borderMoveSpeed;
        else if (Input.mousePosition.x > Screen.width - (Screen.width * screenOffset))
            speed.x += borderMoveSpeed;

        if (Input.mousePosition.y < Screen.height * screenOffset)
            speed.z -= borderMoveSpeed;
        else if (Input.mousePosition.y > Screen.height - (Screen.height * screenOffset))
            speed.z += borderMoveSpeed;

        transform.position += speed * Time.deltaTime;
    }
}