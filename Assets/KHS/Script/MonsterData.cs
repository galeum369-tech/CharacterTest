using UnityEngine;



public enum Type
{
    Melee,
    Ranged,
    Boss
}
[CreateAssetMenu(fileName = "New Monster", menuName = "Moster/Mosnter Data")]
public class MonsterData : ScriptableObject
{
    public string mobName;
    public int mobID;
    public Type type;
    public GameObject MobPrefab;
    public int HP;
    public int Speed;
    public int Attack;
    public int Cool;
    public int attackRange;




}
