using System.Collections.Generic;
using UnityEngine;

namespace TargetDefense.Targets.Data
{
    /// <summary>
    /// Data container for settings per tower level
    /// </summary>
    public class TargetrLevelData  
    {
        public long Id { get; set; }

        public string resourcePath;
        /// <summary>
        /// A description of the tower for displaying on the UI
        /// </summary>
        public string description;

        /// <summary>
        /// A description of the tower for displaying on the UI
        /// </summary>
        public string upgradeDescription;

        public int damage;
        /// <summary>
        /// The cost to upgrade to this level
        /// </summary>
        public int cost;

        /// <summary>
        /// The sell cost of the tower
        /// </summary>
        public int sell;

        /// <summary>
        /// The max health
        /// </summary>
        public int maxHealth;

        /// <summary>
        /// The starting health
        /// </summary>
        public int startingHealth;

        /// <summary>
        /// The tower icon
        /// </summary>
        public string icon;
    }
}