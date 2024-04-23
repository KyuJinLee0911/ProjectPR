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

    Quaternion targetRotation;
    bool isMoving = false;

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
        moveAction.performed += MoveCheck; // 움직이고 있을때 isMoving을 true로 바꿔줌
        moveAction.canceled += MoveCheck; // 움직임이 끝날 때 isMoving을 false로 바꿔줌
        
        jumpAction.Enable();
        jumpAction.performed += JumpPerformed;
        targetRotation = transform.rotation;
    }

    private void FixedUpdate()
    {
        Vector2 keyboard_vector = moveAction.ReadValue<Vector2>();
        if(isMoving)
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

    void MoveCheck(InputAction.CallbackContext context)
    {
        float _x, _z;
        Vector2 keyboardVector = context.ReadValue<Vector2>();
        _x = keyboardVector.x;
        _z = keyboardVector.y;

        // 움직이는지 아닌지에 따라 isMoving 변수 결정. 그 뒤 플레이어 애니메이션 바꿈
        isMoving = _x != 0 || _z != 0 ? true : false;
        playerAnimator.SetBool("isMoving", isMoving);
    }

    

    void MOVE(float _x, float _z)
    {
        // 앞으로 가는지 뒤로 가는지에 따라 애니메이션 결정
        bool isForward = _z >= 0 ? true : false;
        playerAnimator.SetBool("isForward", isForward);  

        // 왜 제곱수로 하는지?
        // => 대각선으로 움직일때 45도의 각도로 회전시켜주기 위해
        float square = _x >= 0 ? _x * _x : -1 * (_x * _x);
        // 뒤로 갈때는 좌우 바라보는 방향을 반대로
        square = _z >= 0 ? square : -square;

        // 따로 x축 입력이 없으면 카메라가 바라보는 방향으로 회전, x축 입력이 있으면 카메라가 바라보는 방향 + 입력만큼의 각도로 회전
        targetRotation = Quaternion.Euler(0, transform.rotation.y + square * 90 + cameraTransform.rotation.eulerAngles.y, 0); 
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.2f);

        // y축 입력이 양수일 경우 캐릭터가 바라보는 방향으로 진행, 음수일 경우 반대 방향으로 진행
        Vector3 moveDir = _z >=0 ? transform.forward : transform.forward * -1;
        transform.position = transform.position + moveDir * moveSpeed;
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
