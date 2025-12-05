using Unity.Android.Gradle.Manifest;
using UnityEngine;



public enum Class
{
    Nomal,      //일반몹
    Boss        //보스몹
}
public enum Type
{
    Melee,      //근접계열
    Range       //원거리계열
}

public enum Race
{
    Slime,      //슬라임
    Golem,      //골렘
    Drone,      //드론
    Beholder,   //비홀더
    Mimic,      //미믹
    Dragon,     //보스/드래곤
    Turtle      //보스/대형거북이
}

public enum MonsterPattern
{
    Chase,          // 추적
    Patrol,         // 순찰
    Charge,         // 돌진
    Jump,           // 점프 공격
    RangedShot,     // 투사체 발사
    Laser,          // 레이저(비홀더)
    Explode,        // 폭발
    MimicTrap,      // 기습
    AOE,            // 범위 공격
    Breath,         // 브레스(드래곤)
    GroundShock     // 지진/충격파(거북)
}

public struct DropItem
{
    public int dummy;   // 나중에 제거하거나 itemID로 변경 가능
    public float chance;   // 드랍 확률
    public int minCount;
    public int maxCount;
}

[CreateAssetMenu(fileName = "New Monster", menuName = "Moster/Mosnter Data")]
public class MonsterData : ScriptableObject
{
    [Header("기본정보")]
    public int mobID; 
    public string mobName;
    public GameObject MobPrefab;

    [Header("몬스터 스텟")]
    public float HP;
    public float Speed;
    public float Attack;
    public float Defense;
    public float CoolTime;
    public float attackRange;
    public float detectionRange;
    [Tooltip("원거리 몹에만 적용")]
    public float minAttackRange;

    [Header("몬스터 클래스/타입")]
    public Class Class;
    public Type Type;
    public Race Race;

    [Header("몬스터 패턴")]
    public MonsterPattern[] Pattern;
    [Header("패턴 파라미터")]
    [Header("각 패턴에 해당하는 파라미터만 적용")]
    public float chargeSpeed;
    public float jumpForce;
    public float explosionRadius;
    public float breathDuration;
    public float groundShockRadius;

    [Header("몬스터 드랍 테이블")]
    public DropItem[] DropTable;

}
