using System;
using UnityEngine;

public delegate void StateChangeEvent(System.Object sender, EnumObjectState newState, EnumObjectState oldState);

public enum EnumPropertyType : int
{
    RoleName = 1,   //角色名
    Sex,    //性别
    RoleID,    //Role ID
    Gold,    //宝石（元宝）
    Coin,    //金币（铜板）
    Level,    //等级
    Exp,    //当前经验
    AttackSpeed,    //攻击速度
    HP,    //当前HP
    HPMax,    //生命最大值
    Attack,    //普通攻击（点数）
    Water,    //水系攻击（点数）
    Fire,    //火系攻击（点数）
}

public enum EnumObjectState
{
    None,
    Initial,
    Loading,
    Ready,
    Closing,
}

public enum EnumRegisterMode
{
    NotRegister,
    AlreadyRegister,
}

public static class UIPathDefines
{
    public static string GAME_NAME = "ShrinkingPlanet";
    public const string UI_PREFAB = "UI/";    //UI预设
    public static float ScreenWidth;
    public static float ScreenHeight;
}
