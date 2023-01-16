using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject gameManager;

    private Camera mainCamera;

    [SerializeField] private float minHeight;
    [SerializeField] private float maxHeight;
    [SerializeField] private float scrollSpeed;
    [SerializeField] private float zoomSpeed;

    private Vector3 previousMousePosition;
    private Vector3 cameraCoverage;

    // Start is called before the first frame update
    void Start()
    {
        if (gameManager == null) { Debug.Log("Game = null"); }
        mainCamera = GetComponent<Camera>();
        GameController gm = gameManager.GetComponent<GameController>();
        (int width, int height) = gm.GetMapSize();
        Vector2 corner = gm.GetMapCorner();

        Vector3 center = new Vector3(corner.x + width / 2 * 0.74f, corner.y + height / 2 * 0.86328125f + 0.431640625f * (height / 2 % 2) - 0.7f, -10);
        transform.position = center;
        AdjustViewportToWorldScaling();
    }

    // Update is called once per frame
    void Update()
    {
        float delta_x;
        float delta_y;
        if (Input.GetMouseButton(2))
        {
            var relMousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            delta_x = cameraCoverage.x * (previousMousePosition.x - relMousePos.x);
            delta_y = cameraCoverage.y * (previousMousePosition.y - relMousePos.y);
        }
        else
        {
            delta_x = Input.GetAxis("Horizontal") * scrollSpeed;
            delta_y = Input.GetAxis("Vertical") * scrollSpeed;
        }
        var delta_z = -Input.mouseScrollDelta.y * zoomSpeed;
        if (delta_z != 0) 
        {
            AdjustViewportToWorldScaling();
        }
        transform.position = new Vector3(delta_x + transform.position.x, transform.position.y + delta_y, transform.position.z);
        mainCamera.orthographicSize = Mathf.Clamp(mainCamera.orthographicSize + delta_z, minHeight, maxHeight);

        previousMousePosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
    }

    void AdjustViewportToWorldScaling()
    {
        var bottomLeft = Camera.main.ViewportToWorldPoint(Vector3.zero);
        var topRight = Camera.main.ViewportToWorldPoint(Vector3.one);
        cameraCoverage = topRight - bottomLeft;
    }
}
