namespace GameModel
{
	public class Skill
    {
		public long Id { get; set; }
		public string Name;
		public string Desc;
		public int[] levelIds;
		public int Level;
		public int Type;
		public int series;
		public int SpeedUp;
		public int MasterConds;
		public int CastConds;
		public int NeedLead;
		public int LeadTime;
		public int ShowProgress;
		public int CastTime;
		public int LastTime;
		public int EffectInterval;
		public int Range;
		public int RangeParams;
		public int FlyerSpeed;
		public int TargetType;
		public int TargetsCount;
		public int ChangePos;
		public int[] Physical;
		public int BaseValue;
		public int ExtraValue;
		public int CoolDown;
		public int[] Buff;
		public int Threat;
		public string IconPack;
		public string IconName;
		public int[] EarthQuake;
		public int[] FlashLight;
		public string Action;
		public int[] MyEffect;
		public int[] MiddleEffect;
		public int[] StruckEffect;
		public int[] RangeEffect;
		public int[] CastEffect;
		public int[] WeaponEffect;
		public string MySound;
		public string MiddleSound;
		public string StruckSound;
		public string RangeSound;
	}
}
