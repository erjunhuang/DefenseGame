using QGame.Core.Config;
 namespace GameModel
{
	public class MapCfg :ConfigBase
	{
		public string Name;
		public string XmlName;
		public string ImageName;
		public string Music;
		public int Type;
		public int Section;
		public int Difflevel;
		public int OpenLevel;
		public int PreCondition;
		public int CarryHero;
		public int MapNum;
		public int WaveNum;
		public int[] CreateMonster;
		public int MapLife;
		public int NeedGold;
		public int NeedEnergy;
		public int NeedItem;
		public int NeedDia;
		public int BuyTimes;
		public int[] Cost;
		public int CloseTime;
		public string KillTarget;
		public int[] OutPut;
		public int[] Reward;
		public int[] FirstReward;
		public int ReviveItem;
		public int[] PackageSkill;
		public int ShowResurgence;
		public int ClearanceInterface;
	}
}
