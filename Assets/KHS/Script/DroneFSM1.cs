using UnityEngine;

public class DroneFSM1 : MonoBehaviour
{
    public MonsterData data;

    // ===== 이동 관련 =====
    Vector3 spawnPoint;
    Vector3 currentMoveTarget; // 현재 이동하려는 목적지
    float moveTimer;

    // 높이 고정을 위한 변수
    float fixedY;

    // ===== 회전 관련 (부드러운 회전) =====
    Quaternion targetRotation; // 드론이 바라봐야 할 목표 회전값
    public float turnSpeed = 5f; // 회전 속도

    // ===== 상태 =====
    public enum DroneState { Patrol, Fire, Cooldown }
    [SerializeField] DroneState state;

    // ===== 공격 관련 =====
    public Transform firePoint;
    public LineRenderer laser;
    public LayerMask hitLayer;

    float stateTimer;
    bool isLaserActive = false;

    // 4방향 (월드 기준: 북, 남, 동, 서)
    Vector3[] cardinalDirections = { Vector3.forward, Vector3.back, Vector3.right, Vector3.left };

    void Start()
    {
        spawnPoint = transform.position;
        fixedY = spawnPoint.y; // 시작 높이 저장

        SetRandomMoveTarget();
        targetRotation = transform.rotation; // 초기 회전값 설정

        ChangeState(DroneState.Patrol);
    }

    void Update()
    {
        // 1. 상태별 로직 실행
        switch (state)
        {
            case DroneState.Patrol: UpdatePatrol(); break;
            case DroneState.Fire: UpdateFire(); break;
            case DroneState.Cooldown: UpdateCooldown(); break;
        }

        // 2. 부드러운 회전 (모든 상태 공통)
        // 현재 회전값에서 -> 목표 회전값으로 -> turnSpeed 속도로 보간
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
    }

    void ChangeState(DroneState newState)
    {
        state = newState;
        stateTimer = 0f;

        switch (newState)
        {
            case DroneState.Fire:
                // 공격 시작 시: 랜덤한 4방향 중 하나를 바라보게 설정
                SetRandomCardinalRotation();
                StartLaser();
                break;

            case DroneState.Cooldown:
                StopLaser();
                // 쿨타임 때는 다시 이동 방향을 바라보게 할 수도 있음 (선택사항)
                break;
        }
    }

    // ============================================================
    // STATE: PATROL (이동하며 기회를 엿봄)
    // ============================================================
    void UpdatePatrol()
    {
        // 이동 로직 수행
        MoveToTarget();

        // 이동 방향을 바라보게 함 (공격 중이 아닐 때 자연스럽게)
        LookAtMovementDirection();

        // 쿨타임 체크 -> 공격 전환
        stateTimer += Time.deltaTime;
        if (stateTimer >= data.laserCooldown)
        {
            ChangeState(DroneState.Fire);
        }
    }

    // ============================================================
    // STATE: FIRE (이동 + 공격 동시에)
    // ============================================================
    void UpdateFire()
    {
        // ★ 핵심: 공격 중에도 이동 함수를 호출하여 멈추지 않음
        MoveToTarget();

        // 레이저 그래픽 & 충돌 판정 업데이트
        UpdateLaser();
        RaycastLaserHit();

        // 발사 시간 체크
        stateTimer += Time.deltaTime;
        if (stateTimer >= data.laserFireTime)
        {
            ChangeState(DroneState.Cooldown);
        }
    }

    // ============================================================
    // STATE: COOLDOWN (공격 후 잠시 대기/이동)
    // ============================================================
    void UpdateCooldown()
    {
        MoveToTarget(); // 쿨타임 때도 계속 움직임
        LookAtMovementDirection();

        stateTimer += Time.deltaTime;
        if (stateTimer >= data.laserCooldown) // 데이터에 후딜레이 변수가 따로 없다면 쿨타임 재사용
        {
            ChangeState(DroneState.Patrol);
        }
    }

    // ============================================================
    // 유틸리티 함수들
    // ============================================================

    void MoveToTarget()
    {
        // 현재 위치에서 목표 지점까지 이동
        // Y축은 영향을 받지 않도록 transform.position의 y는 유지될 것임 (Target 설정에서 처리)
        transform.position = Vector3.MoveTowards(transform.position, currentMoveTarget, data.Speed * Time.deltaTime);

        // 목표 도달 체크
        if (Vector3.Distance(transform.position, currentMoveTarget) < 0.1f)
        {
            SetRandomMoveTarget();
        }
    }

    void SetRandomMoveTarget()
    {
        Vector2 random = Random.insideUnitCircle * data.moveRange;

        // ★ Y축은 고정(fixedY), X와 Z만 변경
        currentMoveTarget = new Vector3(spawnPoint.x + random.x, fixedY, spawnPoint.z + random.y);
    }

    // 이동하는 방향을 바라보게 설정 (Patrol 상태용)
    void LookAtMovementDirection()
    {
        Vector3 dir = (currentMoveTarget - transform.position).normalized;
        if (dir != Vector3.zero)
        {
            // Y축 회전만 반영하기 위해 LookRotation 사용
            targetRotation = Quaternion.LookRotation(dir);
        }
    }

    // 4방향 중 하나를 바라보게 설정 (Fire 상태용)
    void SetRandomCardinalRotation()
    {
        Vector3 randomDir = cardinalDirections[Random.Range(0, 4)];
        targetRotation = Quaternion.LookRotation(randomDir);
    }

    // ============================================================
    // 레이저 관련
    // ============================================================
    void StartLaser()
    {
        laser.enabled = true;
        isLaserActive = true;
    }

    void UpdateLaser()
    {
        if (!isLaserActive) return;

        // 드론이 움직이고 회전하므로 매 프레임 위치 갱신 필요
        laser.SetPosition(0, firePoint.position);

        // firePoint.forward는 드론의 회전(부드러운 회전 포함)을 따라감
        laser.SetPosition(1, firePoint.position + firePoint.forward * data.laserLength);
    }

    void StopLaser()
    {
        laser.enabled = false;
        isLaserActive = false;
    }

    void RaycastLaserHit()
    {
        // firePoint.forward 기준으로 레이 발사
        if (Physics.Raycast(firePoint.position, firePoint.forward, out RaycastHit hit, data.laserLength, hitLayer))
        {
            if (hit.collider.CompareTag("Player"))
            {
                // hit.collider.GetComponent<Player>().TakeDamage(data.laserDamage);
                Debug.Log("Player Hit!");
            }
        }
    }

    private void OnDrawGizmos()
    {
        // 이동 범위
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(spawnPoint, (data != null) ? data.moveRange : 5f);

        // 현재 목적지
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, currentMoveTarget);
        Gizmos.DrawSphere(currentMoveTarget, 0.3f);
    }
}