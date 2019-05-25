using System;
using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense.Game
{
    /// <summary>
    /// A calss to save level data
    /// </summary>
    /// 
    [Serializable]
    public class PlayInfoSaveData 
    {
        public int playerId;
        public string playerName;
        public int sex;
        public List<LevelSaveData> completedLevels;
        public PlayInfoSaveData(int playerId, string playerName , int sex, List<LevelSaveData> completedLevels)
        {
            this.playerId = playerId;
            this.playerName = playerName;
            this.sex = sex;
            this.completedLevels = completedLevels;
        }
    }
}