using GameModel;
using System.Collections.Generic;
using TargetDefense.Level;
using TargetDefense.Targets.Data;

public class GameData
{   
    //全局数据
    public static string Token = "";
    public static PPlayerInfo PlayInfo = new PPlayerInfo();//玩家信息


    //游戏数据
    public static LevelInfo levelInfo = new LevelInfo();//场景信息
    public static GameInfo gameInfo = new GameInfo();//游戏信息
}