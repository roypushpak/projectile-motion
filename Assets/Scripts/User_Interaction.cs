using UnityEngine;

public class User_Interaction : MonoBehaviour
{
    private GameObject phenomenon;
    private GameObject ball;
    private Vector3 lastMousePosition;
    public float movementSpeed;
    public float mouseDragSpeed;

    [Header("------Audio Source------")]
    [SerializeField] private AudioSource throwSource;

    [Header("------Audio Clip------")]
    public AudioClip throwSound;

    private bool hasPlayedThrowSound = false;
    public Camera mainCamera;
    public float zoomSpeed = 10f;
    public float minFOV = 10f;
    public float maxFOV = 100f;

    void Start()
    {
        movementSpeed = 2.5f;
        mouseDragSpeed = 100f;

        phenomenon = GameObject.Find("Phenomenon");
        ball = GameObject.Find("Basketball");

        if (phenomenon == null)
            Debug.LogError("No GameObject named 'Phenomenon' found in the scene.");

        if (ball == null)
            Debug.LogError("No GameObject named 'Basketball' found in the scene.");

        if (throwSource == null)
        {
            throwSource = gameObject.AddComponent<AudioSource>();
        }

        if (mainCamera == null)
        {
            mainCamera = Camera.main;
        }
    }

    void Update()
    {
        if (ball != null)
        {
            HandleKeyboardMovement();
            HandleBallHalt();
        }

        if (phenomenon != null)
        {
            HandleMouseDragRotation();
        }

        HandleZoom();
    }

    private void HandleKeyboardMovement()
    {
        if (ball == null) return;

        Rigidbody rb = ball.GetComponent<Rigidbody>();
        if (rb == null) return;

        bool isMoving = false;

        //Movement controls using WASD, arrow keys, and Q/E for up/down
        Vector3 forceDirection = Vector3.zero;

        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow)) { forceDirection += transform.forward; isMoving = true; }
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow)) { forceDirection -= transform.forward; isMoving = true; }
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) { forceDirection -= transform.right; isMoving = true; }
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) { forceDirection += transform.right; isMoving = true; }
        if (Input.GetKey(KeyCode.Q)) { forceDirection += Vector3.up; isMoving = true; }
        if (Input.GetKey(KeyCode.E)) { forceDirection -= Vector3.up; isMoving = true; }

        //Apply force to the ball based on accumulated direction
        if (isMoving)
        {
            rb.AddForce(forceDirection * movementSpeed, ForceMode.Force);

            //Play throw sound once when movement starts
            if (!hasPlayedThrowSound)
            {
                throwSource.clip = throwSound;
                throwSource.Play();
                hasPlayedThrowSound = true;
            }
        }
        else
        {
            hasPlayedThrowSound = false;
        }

        //Check for reset rotation (pressing "R")
        if (Input.GetKeyDown(KeyCode.R) && phenomenon != null)
        {
            phenomenon.transform.rotation = Quaternion.identity;
        }
    }

    private void HandleMouseDragRotation()
    {
        if (Input.GetMouseButton(0))
        {
            Vector3 mouseDelta = Input.mousePosition - lastMousePosition;

            float xRotation = -mouseDelta.y * mouseDragSpeed * Time.deltaTime;
            float yRotation = mouseDelta.x * mouseDragSpeed * Time.deltaTime;

            phenomenon.transform.Rotate(xRotation, yRotation, 0, Space.World);
        }

        lastMousePosition = Input.mousePosition;
    }

    private void HandleZoom()
    {
        if (mainCamera == null) return;

        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0)
        {
            mainCamera.fieldOfView -= scroll * zoomSpeed;
            mainCamera.fieldOfView = Mathf.Clamp(mainCamera.fieldOfView, minFOV, maxFOV);
        }
    }

    private void HandleBallHalt()
    {
        if (Input.GetKeyDown(KeyCode.G) && ball != null)
        {
            Rigidbody rb = ball.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.velocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }
        }
    }
}
