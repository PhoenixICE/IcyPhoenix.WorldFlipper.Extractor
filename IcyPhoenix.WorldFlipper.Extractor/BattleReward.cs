using System.Collections.Generic;

namespace IcyPhoenix.WorldFlipper.Extractor
{
    public class BattleReward
    {
        public string Stamina { get; set; }
        public string RankBScore { get; set; }
        public string RankAScore { get; set; }
        public string RankSScore { get; set; }
        public string RankSSScore { get; set; }
        public string RankCItemCount { get; set; }
        public string RankBItemCount { get; set; }
        public string RankAItemCount { get; set; }
        public string RankSItemCount { get; set; }
        public string RankSSItemCount { get; set; }
        public string BattleRewardRP { get; set; }
        public string BattleRewardExp { get; set; }
        public string BattleRewardMana { get; set; }
        public string BattleRewardPoolExp { get; set; }
        public string BattleEnemyLevel { get; set; }
        public string StageName { get; set; }
        public List<Drop> Drops { get; set; } = new List<Drop>();
    }
}