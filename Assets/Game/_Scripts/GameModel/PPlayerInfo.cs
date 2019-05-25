using System;
using System.Collections.Generic;

public class PPlayerInfo
{
    /// <summary> 角色ID </summary>
    public int PlayerId { get; set; }

    /// <summary> 角色名称 </summary>
    public string Name { get; set; }

    /// <summary> 性别 </summary>
    public int Gender { get; set; }

    /// <summary> 头像ID </summary>
    public int AvatarId { get; set; }

    /// <summary> 级别 </summary>
    public int Level { get; set; }

    /// <summary> 经验 </summary>
    public int Exp { get; set; }
    /// <summary> 钻石 1 </summary>
    public int Diamond { get; set; }

    /// <summary> 功能开放标识 </summary>
    public Int64 OpenFuncMask { get; set; }

    /// <summary> 剧情记录 </summary>
    public int PlotVal { get; set; }

    ///// <summary> 装备 </summary>
    //public List<EquipInfo> Equips { get; set; }

    ///// <summary> 历练属性 </summary>
    //public List<KeyValue> Attribs { get; set; }

    ///// <summary> 宝石列表 </summary>
    //public List<GemInfo> Gems { get; set; }


}