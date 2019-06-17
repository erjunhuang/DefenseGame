using QGame.Core.Config;
 namespace GameModel
{
	public class UpdateCfg :ConfigBase
	{
		public string Name;
		public int NextID;
		public int NeedLv;
		public int NeedGold;
		public int[] SKillTree;
		public int[] SkillSeries;
		public int[] Relation;
		public int[] levelIds;
	}
}
