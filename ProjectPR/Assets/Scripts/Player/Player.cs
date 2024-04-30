using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class Player : MonoBehaviour, IDamageable, IUnitStats
{
    [SerializeField]
    private PlayerData playerData;

    [SerializeField]
    PlayerData[] playerDatas;

    private CreatureType creatureType = CreatureType.CT_PLAYER;
    public PlayerClass playerClass = PlayerClass.PC_BEGINNER;
    public CreatureState playerState = CreatureState.CS_IDLE;


    public float Health { get => playerData.Hp; set { } }
    public float maxHp;
    public float currentHp;
    public int Defence { get => playerData.Defence; set { } }
    public float Stamina { get { return playerData.Stamina; } set => playerData.Stamina = value; }
    public int Damage { get => playerData.Damage; set { } }
    public float maxStamina;
    public float currentStamina;
    public float jumpReqStamina = 5;
    public float sprintReqStamina = 3;

    [SerializeField]
    private int lv = 1;
    public int Level { get => lv; set => lv = value; }

    [SerializeField]
    private float exp = 0.00f;
    public float Exp { get => exp; set => exp = value; }

    [SerializeField]
    private int str = 1;
    public int Strength { get => str; set => str = value; }

    [SerializeField]
    private int dex = 1;
    public int Dexerity { get => dex; set => dex = value; }

    [SerializeField]
    private int inteligence = 1;
    public int Inteligence { get => inteligence; set => inteligence = value; }

    Animator animator;

    public Skill[] skills;


    //----------------------------컨트롤 관련-------------------------------
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
    bool isReadyToAttack = false;
    [SerializeField]
    bool isSprinting = false;
    Vector2 keyboard_vector;

    void Start()
    {
        animator = gameObject.GetComponent<Animator>();
        rigidbody = gameObject.GetComponent<Rigidbody>();
        cameraTransform = Camera.main.transform;
        InitializePlayerStat(playerData);
    }

    private void FixedUpdate()
    {
        Debug.Log($"{Stamina}");
        // 앞을 보고 전력질주 중에도 뒤로가는 버튼(s)을 누르면 속도가 줄어들게 함
        if (!isForward)
            MoveSpeed = moveSpeedPreset;
        animator.SetFloat("moveSpeed", MoveSpeed);

        if (isMoving)
            MOVE(keyboard_vector.x, keyboard_vector.y);

        // Raycast를 통해 땅에 닿았는지 체크.
        // Raycast의 시작지점을 0.1만큼 띄운 이유는 발 밑에서 바로 레이를 나가게 하면 가끔 울퉁불퉁한 지형에서 땅을 밟고 있음에도 밟고있지 않다고 판정이 날 때가 있었음
        // 따라서 0.1만큼 띄우고 Ray의 길이를 0.2로 해서 울퉁불퉁한 지형에서도 안정적으로 땅 밟고 있는 것을 체크할 수 있도록 함
        hasTouchedGround = Physics.Raycast(transform.position + new Vector3(0, 0.1f, 0), Vector3.down, raycastDist, groundLayer);

        if (isSprinting)
        {
            currentStamina -= sprintReqStamina * Time.deltaTime;
            if (currentStamina < 0)
            {
                currentStamina = 0;
                OnSprint();
            }

        }
        else
        {
            currentStamina += Time.deltaTime;
            if (currentStamina > Stamina)
                currentStamina = Stamina;
        }

        if (!isMoving && isSprinting)
        {
            OnSprint();
        }

    }

    void OnMove(InputValue value)
    {

        keyboard_vector = value.Get<Vector2>();
        float _x, _z;

        _x = keyboard_vector.x;
        _z = keyboard_vector.y;

        // 움직이는지 아닌지에 따라 isMoving 변수 결정. 그 뒤 플레이어 애니메이션 바꿈
        isMoving = _x != 0 || _z != 0 ? true : false;
        animator.SetBool("isMoving", isMoving);
        // 멈췄을 때 애니메이터의 isForward 초기화
        // 초기화를 해주는 이유 => 초기화를 하지 않으면 isForward가 false인 상태가 되는데, 이 이후 앞으로 가려고 하면 isForward = false, isMoving = true인 상태가
        // 아주 잠깐 지속되면서 뒤로가기 모션이 잠깐 나왔다가 앞으로가기 모션이 나옴
        // -> 애니메이션에 딜레이가 생김
        if (!isMoving)
            animator.SetBool("isForward", true);
    }

    void MOVE(float _x, float _z)
    {
        // 앞으로 가는지 뒤로 가는지에 따라 애니메이션 결정
        isForward = _z >= 0 ? true : false;
        animator.SetBool("isForward", isForward);

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

    void OnJump()
    {
        if (!hasTouchedGround)
            return;
        
        if(currentStamina < jumpReqStamina)
            return;

        animator.SetTrigger("Jump");
        Debug.Log("Jumped");
        rigidbody.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
        currentStamina -= jumpReqStamina;
    }

    void OnAttack()
    {
        Debug.Log("Attack");
        targetRotation = Quaternion.Euler(0, transform.rotation.y + cameraTransform.rotation.eulerAngles.y, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 0.2f);
        playerState = CreatureState.CS_READY;
        isReadyToAttack = true;
        animator.SetBool("battleReady", true);
        Attack();
        Debug.Log(playerState);
        if (isReadyToAttack)
            StartCoroutine(CancelAttack());
    }

    IEnumerator CancelAttack()
    {
        yield return new WaitForSeconds(5);

        playerState = CreatureState.CS_IDLE;
        animator.SetBool("battleReady", false);
        isReadyToAttack = false;
        Debug.Log(playerState);
    }

    void OnSprint()
    {
        isSprinting = !isSprinting;

        if (!isForward)
        {
            MoveSpeed = moveSpeedPreset;
            return;
        }

        moveSpeed = isSprinting ? moveSpeed * 1.6f : moveSpeedPreset;
    }

    public void SelectClass(int classNum)
    {
        if (playerClass != PlayerClass.PC_BEGINNER)
            return;

        playerData = playerDatas[classNum];
        InitializePlayerStat(playerData);
        

        Debug.Log($"Class : {playerData.ClassName}, HP : {Health}, Stamina : {Stamina}, Defence : {Defence}, Damage : {Damage}");

    }

    public void InitializePlayerStat(PlayerData data)
    {
        maxStamina = data.Stamina;
        currentStamina = data.Stamina;
        maxHp = data.Hp;
        currentHp = data.Hp;
    }

    public void ChangeStats(int statNum)
    {
        switch (statNum)
        {
            case 0:
                Strength++;
                break;
            case 1:
                Dexerity++;
                break;
            case 2:
                Inteligence++;
                break;
        }

        Debug.Log($"str : {Strength}, dex : {Dexerity}, int : {Inteligence}");
    }

    public void Die()
    {
        playerState = CreatureState.CS_DEAD;
    }

    public void GetDamaged()
    {

    }

    public void RestoreHealth()
    {

    }

    public void Attack()
    {
        animator.SetTrigger("Attack");
    }

    


}
