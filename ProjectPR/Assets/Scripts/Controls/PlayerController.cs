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

    private Transform cameraTransform;

    float moveSpeed = 0.15f;

    private void Awake()
    {
        cameraTransform = Camera.main.transform;
        action = new PlayerControl();
        moveAction = action.Player.Move;
        jumpAction = action.Player.Jump;
        playerAnimator = gameObject.GetComponent<Animator>();
        rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        moveAction.Enable();
        jumpAction.Enable();

        jumpAction.performed += JumpPerformed;

    }

    private void FixedUpdate()
    {
        Vector2 keyboard_vector = moveAction.ReadValue<Vector2>();
        MOVE(keyboard_vector.x, keyboard_vector.y);

        // Raycast를 통해 땅에 닿았는지 체크.
        // Raycast의 시작지점을 0.1만큼 띄운 이유는 발 밑에서 바로 레이를 나가게 하면 가끔 울퉁불퉁한 지형에서 땅을 밟고 있음에도 밟고있지 않다고 판정이 날 때가 있었음
        // 따라서 0.1만큼 띄우고 Ray의 길이를 0.2로 해서 울퉁불퉁한 지형에서도 안정적으로 땅 밟고 있는 것을 체크할 수 있도록 함
        hasTouchedGround = Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0), Vector3.down, raycastDist, groundLayer);
    }

    private void OnDisable()
    {
        moveAction.Disable();
        jumpAction.Disable();
        jumpAction.performed -= JumpPerformed;
    }


    void MOVE(float _x, float _z)
    {
        bool isMoving = _x != 0 || _z != 0 ? true : false;
        playerAnimator.SetBool("isMoving", isMoving);
        bool isForward = _z >= 0 ? true : false;
        playerAnimator.SetBool("isForward", isForward);

        float square = _x >= 0 ? _x * _x : -1 * (_x * _x);
        square = isForward ? square : -square;
        Quaternion targetRotation = Quaternion.Euler(0, transform.rotation.y + square * 90 + cameraTransform.rotation.eulerAngles.y, 0);
        Debug.Log(cameraTransform.rotation.y);

        transform.position += transform.forward * _z * moveSpeed + transform.right * _x * moveSpeed;
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
