using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    Transform characterTransform;

    PlayerControl playerControl;
    InputAction inputAction;

    public float xAxis, yAxis;

    [SerializeField]
    private float horizontalSensitive = 3.0f;
    [SerializeField]
    private float verticalSensitive = 3.0f;
    [SerializeField]
    private float distance;
    [SerializeField]
    private float maxRot = 80.0f;
    [SerializeField]
    private float minRot = -10.0f;
    [SerializeField]
    private float smoothTime = 0.12f;

    private Vector3 targetRotation;
    private Vector3 currentVelocity;

    // Start is called before the first frame update
    void Awake()
    {
        playerControl = new PlayerControl();
        distance = Vector3.Distance(transform.position, characterTransform.position);
        inputAction = playerControl.Player.Look;
    }

    private void OnEnable()
    {
        inputAction.Enable();
        inputAction.performed += Look;
    }

    private void OnDisable()
    {
        inputAction.Disable();
        inputAction.performed -= Look;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = characterTransform.position - transform.forward * distance + Vector3.up;
    }

    void Look(InputAction.CallbackContext context)
    {
        Vector2 mousePos = context.ReadValue<Vector2>();
        yAxis += mousePos.x * horizontalSensitive;
        xAxis -= mousePos.y * verticalSensitive;

        xAxis = Mathf.Clamp(xAxis, minRot, maxRot);
        
        targetRotation = Vector3.SmoothDamp(targetRotation, new Vector3(xAxis, yAxis), ref currentVelocity, smoothTime);
        transform.eulerAngles = targetRotation;
    }
}
