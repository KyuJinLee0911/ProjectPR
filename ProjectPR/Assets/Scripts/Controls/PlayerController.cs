using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor.Animations;

public class PlayerController : MonoBehaviour
{
    PlayerControl action;
    InputAction moveAction;
    InputAction jumpAction;
    Animator playerAnimator;
    [SerializeField]
    Transform modelTransform;
    new Rigidbody rigidbody;
    [SerializeField]
    float jumpPower;

    bool hasTouchedGround = true;
    [SerializeField]
    float raycastDist = 0.2f;
    [SerializeField]
    LayerMask groundLayer;

    private void Awake()
    {
        action = new PlayerControl();
        moveAction = action.Player.Move;
        jumpAction = action.Player.Jump;
        playerAnimator = gameObject.GetComponent<Animator>();
        rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        moveAction.Enable();
        moveAction.started += Started;
        moveAction.performed += Performed;
        moveAction.canceled += Canceled;
        jumpAction.Enable();

        jumpAction.performed += JumpPerformed;

    }

    private void FixedUpdate()
    {
        Vector2 keyboard_vector = moveAction.ReadValue<Vector2>();
        MOVE(keyboard_vector.x, keyboard_vector.y);

        hasTouchedGround = Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0), Vector3.down, raycastDist, groundLayer);
        Debug.Log(hasTouchedGround);
    }

    private void OnDisable()
    {
        moveAction.Disable();
        moveAction.started -= Started;
        moveAction.performed -= Performed;
        moveAction.canceled -= Canceled;
        jumpAction.Disable();
        jumpAction.performed -= JumpPerformed;
    }

    void Started(InputAction.CallbackContext context)
    {
        Debug.Log("started!");
    }

    void Performed(InputAction.CallbackContext context)
    {
        Debug.Log("performed!");
    }

    void Canceled(InputAction.CallbackContext context)
    {
        Debug.Log("canceled!");
    }

    void MOVE(float _x, float _z)
    {
        bool isMoving = _x != 0 || _z != 0 ? true : false;
        playerAnimator.SetBool("isMoving", isMoving);
        bool isForward = _z >= 0 ? true : false;
        playerAnimator.SetBool("isForward", isForward);

        float square = _x >= 0 ? _x * _x : -1 * (_x * _x);
        square = isForward ? square : -square;
        Quaternion targetRotation = Quaternion.Euler(0, transform.rotation.y + square * 90, 0);

        transform.position = new Vector3(transform.position.x + _x * 0.1f, transform.position.y, transform.position.z + _z * 0.1f);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.2f);
    }

    void JumpPerformed(InputAction.CallbackContext context)
    {
        if (!hasTouchedGround)
            return;
        playerAnimator.SetTrigger("Jump");
        Debug.Log("Jumped");
        rigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
    }
}
