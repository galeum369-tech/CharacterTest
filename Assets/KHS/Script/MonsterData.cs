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
    Rhino       //보스/코뿔소
}

public enum MonsterPattern
{
    Chase,          // 추적
    Patrol,         // 순찰
    Jump,           // 점프 공격
    RangedShot,     // 투사체 발사
    Laser,          // 레이저(비홀더)
    Explode,        // 폭발
    MimicTrap,      // 기습
    AOE,            // 범위 공격
    Breath,         // 브레스(드래곤)
    Charge,         // 돌진(코뿔소)
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
    public float jumpForce;             //슬라임(일반몹인데 이거까지 필요한가 싶은 느낌 차라리 드래곤의 점프 후 범위 충격파가 더 나을듯)
    public float explosionRadius;       //슬라임 폭발이긴한데 폭발을 넣을 필요가 있나(차라리 미믹한테 죽을때 터지는게 더 나을듯한 아님 빼든지)
    public float breathDuration;        //드래곤
    public float chargeSpeed;           //코뿔소



    [Header("몬스터 드랍 테이블")]
    public DropItem[] DropTable;

}
