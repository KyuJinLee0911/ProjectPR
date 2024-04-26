using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEditor.Animations;
using UnityEditor.Search;
using RPGCharacterAnims.Actions;

public class PlayerController : MonoBehaviour
{
    Player player;
    CreatureState playerState;
    PlayerControl action;
    InputAction moveAction;
    InputAction jumpAction;
    InputAction attackAction;
    InputAction sprintAction;
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

    [SerializeField]
    float moveSpeed = 0.15f;
    public float MoveSpeed { get => moveSpeed; set => moveSpeed = value; }
    float moveSpeedPreset = 0.15f;

    Quaternion targetRotation;
    bool isMoving = false;
    bool isForward = true;
    bool isSprinting = false;

    private void Awake()
    {
        PlayerReset();

    }

    private void OnEnable()
    {
        EnablePlayerAction();
    }

    private void FixedUpdate()
    {
        // 앞을 보고 전력질주 중에도 뒤로가는 버튼(s)을 누르면 속도가 줄어들게 함
        if(!isForward)
            MoveSpeed = moveSpeedPreset;
        playerAnimator.SetFloat("moveSpeed", MoveSpeed);
        Vector2 keyboard_vector = moveAction.ReadValue<Vector2>();
        if (isMoving)
            MOVE(keyboard_vector.x, keyboard_vector.y);

        // Raycast를 통해 땅에 닿았는지 체크.
        // Raycast의 시작지점을 0.1만큼 띄운 이유는 발 밑에서 바로 레이를 나가게 하면 가끔 울퉁불퉁한 지형에서 땅을 밟고 있음에도 밟고있지 않다고 판정이 날 때가 있었음
        // 따라서 0.1만큼 띄우고 Ray의 길이를 0.2로 해서 울퉁불퉁한 지형에서도 안정적으로 땅 밟고 있는 것을 체크할 수 있도록 함
        hasTouchedGround = Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0), Vector3.down, raycastDist, groundLayer);
    }

    private void OnDisable()
    {
        DisablePlayerAction();
    }

    private void PlayerReset()
    {
        cameraTransform = Camera.main.transform;
        action = new PlayerControl();
        player = gameObject.GetComponent<Player>();
        moveAction = action.Player.Move;
        jumpAction = action.Player.Jump;
        attackAction = action.Player.Attack;
        sprintAction = action.Player.Sprint;
        playerAnimator = gameObject.GetComponent<Animator>();
        rigidbody = gameObject.GetComponent<Rigidbody>();
    }

    private void EnablePlayerAction()
    {
        moveAction.Enable();
        moveAction.performed += MovePerformed; // 움직이고 있을때 isMoving을 true로 바꿔줌
        moveAction.canceled += MoveCanceled; // 움직임이 끝날 때 isMoving을 false로, isForward를 true로 바꿔줌

        jumpAction.Enable();
        jumpAction.performed += JumpPerformed;
        targetRotation = transform.rotation;

        attackAction.Enable();
        attackAction.performed += AttackPerformed;
        attackAction.canceled += AttackCanceled;

        sprintAction.Enable();
        sprintAction.performed += SprintCheck;
        sprintAction.canceled += SprintCheck;
    }

    private void DisablePlayerAction()
    {
        moveAction.performed -= MovePerformed;
        moveAction.canceled -= MoveCanceled;
        moveAction.Disable();

        jumpAction.performed -= JumpPerformed;
        jumpAction.Disable();

        attackAction.performed -= AttackPerformed;
        attackAction.canceled -= AttackCanceled;
        attackAction.Disable();

        sprintAction.performed -= SprintCheck;
        sprintAction.performed -= SprintCheck;
        sprintAction.Disable();
    }

    void MovePerformed(InputAction.CallbackContext context)
    {
        float _x, _z;
        Vector2 keyboardVector = context.ReadValue<Vector2>();
        _x = keyboardVector.x;
        _z = keyboardVector.y;

        // 움직이는지 아닌지에 따라 isMoving 변수 결정. 그 뒤 플레이어 애니메이션 바꿈
        isMoving = _x != 0 || _z != 0 ? true : false;
        playerAnimator.SetBool("isMoving", isMoving);
    }

    void MoveCanceled(InputAction.CallbackContext context)
    {
        MovePerformed(context);
        // 애니메이터의 isForward 초기화
        // 초기화를 해주는 이유 => 초기화를 하지 않으면 isForward가 false인 상태가 되는데, 이 이후 앞으로 가려고 하면 isForward = false, isMoving = true인 상태가
        // 아주 잠깐 지속되면서 뒤로가기 모션이 잠깐 나왔다가 앞으로가기 모션이 나옴
        // -> 애니메이션에 딜레이가 생김
        playerAnimator.SetBool("isForward", true);
    }



    void MOVE(float _x, float _z)
    {
        // 앞으로 가는지 뒤로 가는지에 따라 애니메이션 결정
        isForward = _z >= 0 ? true : false;
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
        Vector3 moveDir = _z >= 0 ? transform.forward : transform.forward * -1;
        transform.position = transform.position + moveDir * MoveSpeed;

    }

    void JumpPerformed(InputAction.CallbackContext context)
    {
        if (!hasTouchedGround)
            return;
        playerAnimator.SetTrigger("Jump");
        Debug.Log("Jumped");
        rigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
    }

    void AttackPerformed(InputAction.CallbackContext context)
    {
        Debug.Log("Attack");
        targetRotation = Quaternion.Euler(0, transform.rotation.y + cameraTransform.rotation.eulerAngles.y, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.2f);
        playerState = CreatureState.CS_READY;
        playerAnimator.SetBool("battleReady", true);
        player.Attack();
        Debug.Log(playerState);
    }

    void AttackCanceled(InputAction.CallbackContext context)
    {
        StartCoroutine(CancelAttack());
    }

    IEnumerator CancelAttack()
    {
        yield return new WaitForSeconds(5);

        playerState = CreatureState.CS_IDLE;
        playerAnimator.SetBool("battleReady", false);
        Debug.Log(playerState);
    }

    void SprintCheck(InputAction.CallbackContext context)
    {
        isSprinting = !isSprinting;

        if (!isForward)
        {
            MoveSpeed = moveSpeedPreset;
            return;
        }

        moveSpeed = isSprinting ? moveSpeed * 1.6f : moveSpeedPreset;
    }
}
