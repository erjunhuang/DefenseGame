using GameModel;
using System.Collections.Generic;
using TargetDefense.Level;
using TargetDefense.Targets.Data;

public class GameData
{   
    //全局数据
    public static string Token = "";
    public static PPlayerInfo PlayInfo = new PPlayerInfo();//玩家信息


    //配置表
    public static Dictionary<long, Monster> monsters = new Dictionary<long, Monster>();//所有生物的信息
    public static Dictionary<long, Level> levels = new Dictionary<long, Level>();//所有场景的信息
    public static Dictionary<long, Skill> skills = new Dictionary<long, Skill>();//所有场景的信息

    //游戏数据
    public static LevelInfo levelInfo = new LevelInfo();//场景信息
    public static GameInfo gameInfo = new GameInfo();//游戏信息
    

}