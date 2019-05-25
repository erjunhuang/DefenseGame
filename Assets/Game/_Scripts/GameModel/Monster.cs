namespace GameModel
{
	public class Monster
	{
		public long Id { get; set; }
		public string Name;
		public int ResId;
		public int Type;
		public int Level;
		public int[] levelIds;
		public int[] AiList;
		public int[] SkillList;
		public int MoveSpeed;
		public int MaxHealth;
		public int PhyAttackMin;
		public int PhyAttackMax;
		public int MagicAttackMin;
		public int MagicAttackMax;
		public int PhyDefense;
		public int MagicDefense;
		public int Crit;
		public int CritDefense;
		public int CritMultiple;
		public int ReduceCritHp;
		public int AttackAdd;
		public int DefPuncture ;
		public int Vampire;
		public int LevelLife;
		public int Sell;
		public int Cost;
		public int LootDropped;
		public int[] Spoils;
		public int ReviveTime;
		public string CreateEffect;
		public string DeathEffect;
		public string HitEffect;
		public int RetentionTime;
		public int Scaling;
		public string PhysicalHoldout;
		public string Icon;
		public int EffectRadius;
		public string Description;
		public string UpgradeDescription;
	}
}
