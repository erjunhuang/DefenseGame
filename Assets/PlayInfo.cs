using GameModel;

public class PlayInfo
{
    public MonsterCfg monster;
    public string iconSpritPath;

    /// <summary>
    /// 普攻cd 秒
    /// </summary>
    public float cd_attack = 1.5f;

    /// <summary>
    /// 技能cd 秒,释放后会cd时间会20%递加
    /// </summary>
    public float cd_skill = 0;

    public long armyID;

    public PlayInfo(MonsterCfg monster) {
        this.monster = monster;
        armyID = monster.Id;
    }
}