using UnityEngine;
using System.Collections;

namespace QGame.Core.FightEnegin
{

    /// <summary>
    /// 战场状态(开场介绍 布阵 开战镜头 战斗中 方阵前进 胜利 失败 结算)
    /// </summary>
    public enum eBattleState : byte { Intro, Embattle, StartCamera, Battling, Move, Victory, Defeat, Count }

    /// <summary>
    /// 方阵状态 Suppress技能压制 Stun晕眩 Stand站立
    /// </summary>
    public enum eAttackState : byte { Idle, SelectTarget, Move, MoveToAttackPosition, Charge, Ready, Fighting, Done, Suppress, Dead, Victory, Stun }

    /// <summary>
    /// 攻击阶段
    /// </summary>
    public enum AttackPhase : byte { Intro, Attack, Before, Fly, After, Done }


    /// <summary>
    /// 格子状态
    /// </summary>
    public enum eTileMode : byte { Hidden, Normal, Highlighted, ValidSelection, InvalidSelection }

    /// <summary>
    /// 介绍文字状态
    /// </summary>
    public enum eIntroStates : byte { Waiting, Starting, Displaying, Closing, Done }

    /// <summary>
    /// 阵型
    /// </summary>
    public enum eFormation : byte { Triangle, Circle, Hexagon }

    /// <summary>
    /// 小兵状态 空闲 补位 死亡 攻击 Force为受力状态
    /// </summary>
    public enum eSingleCharState : byte { Idle, Cover, Run, Charge, Ready, FightReady, RandomAttack, Attack, Skill, BeAttack, Dead, Victory, Defeat, Force }//, Superskill

    /// <summary>
    /// 飞行弹道类型(抛物线 直线)
    /// </summary>
    public enum eBallistic : int { Parabola, Line }



    /// <summary>
    /// heal2 回血  control3 控制  guwu 4 鼓  huluan 5 混乱  pojia 7 破甲  jiansu 9 减速
    /// </summary>
    public enum eBuffType : int { damage = 1, heal = 2, control = 3, guwu = 4, huluan = 5, jajia = 6, pojia = 7, jasu = 8, jiansu = 9, jatili = 10,
        jiantili = 11, fanjian = 12, shalou = 13 ,stand = 14 };
    /// 被动技能类型 1=商业 2=农业 4=工业 8=加攻 16=减攻 32=加防 64=减防 128=加速 256=减速 512=击杀 1024=控制
    /// business = 1, farming = 2, industry = 4, attackUp = 6, attackDown = 16, defenceUp = 32, defenceDown = 64, actionUp = 128, actionDown = 256, kill = 512, control = 1024

    public enum eNormalHurtType : byte { normal, little, big, miss, skill }

    /// <summary>
    /// 技能发动概率类型 Force = 武力, Agile = 政治, Wisdom = 智力, Charm = 魅力
    /// </summary>
    public enum ePassiveProbabilityType : byte { Force = 3, Agile = 4, Wisdom = 5, Charm = 7 }

    public enum eScoreFlashType : byte { FriendHurt = 1, Heal = 2, SkillHurt = 3, ContiuneHurt = 4, ContiuneHeal = 5, Common =6, SkillName = 7, EnemyHurt = 8,SkillUp=9,SkillDown=10,SkillTag=11}

    /// <summary>
    /// 格子状态 默认  选择  可走 我方 敌方 
    /// </summary>
    public enum eTileState { Default, GreenSelect, RedSelect, Friendly, Hostile }

    /// <summary>
    /// SelectRandomOne 选择所有方阵中的随机一个，友方随机概率为10% //3=选择己方目标（新）9=全体随机（新）
    /// </summary>
    public enum eTargetSelectType { SelectFriend = 3, SelectEnemy = 4, SelectRandomOne = 9, SelectFuHuo = 10 };// SelectRandomFriend, SelectRandomEnemy,

    /// <summary>
    /// 技能加载的方向，orignal原始方向，self与自身方向一致
    /// </summary>
    public enum eSkillDirection { orignal = 0, self = 1 }

    public enum eFightObjectType { player=0,juma,xianjing,jianta,zhangai}
    /// <summary>
    /// 技能镜头动画 none无  MainCamera 对主像机进行动画 AttackerCamera 将动画绑定在攻击者身上
    /// </summary>
    public enum eCameraAnimatorType { None = 0,MainCamera,AttackerCamera}
    /// <summary>
    /// 战斗UI类型
    /// </summary>
    public enum eFightUIType { pve = 0, pvp = 1,embattle=2 }
}