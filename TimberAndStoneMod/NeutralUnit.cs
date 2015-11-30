using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod
{
    public class NeutralUnit
    {
        public const String GOBLIN_INFANTRY = "Goblin Infantry";
        public const String GOBLIN_ARCHER = "Goblin Archer";
        public const String SKELETON_INFANTRY = "Skeleton Infantry";
        public const String SKELETON_ARCHER = "Skeleton Archer";
        public const String HUMAN_INFANTRY = "Human Infantry";
        public const String HUMAN_ARCHER = "Human Archer";

        public static readonly List<String> NameList = new List<String>() 
        {
            GOBLIN_INFANTRY, GOBLIN_ARCHER, SKELETON_INFANTRY, SKELETON_ARCHER, HUMAN_INFANTRY, HUMAN_ARCHER
        };

        public static readonly NeutralUnit GoblinInfantry = new NeutralUnit(GOBLIN_INFANTRY);
        public static readonly NeutralUnit GoblinArcher = new NeutralUnit(GOBLIN_ARCHER);
        public static readonly NeutralUnit SkeletonInfantry = new NeutralUnit(SKELETON_INFANTRY);
        public static readonly NeutralUnit SkeletonArcher = new NeutralUnit(SKELETON_ARCHER);
        public static readonly NeutralUnit HumanInfantry = new NeutralUnit(HUMAN_INFANTRY);
        public static readonly NeutralUnit HumanArcher = new NeutralUnit(HUMAN_ARCHER);

        public static readonly NeutralUnit[] List = new NeutralUnit[] 
        {
            HumanInfantry, HumanArcher, GoblinInfantry, GoblinArcher, SkeletonInfantry, SkeletonArcher
        };

        public static NeutralUnit Random { get { return List[UnityEngine.Random.Range(0, List.Count())]; } }


        private String name;

        private NeutralUnit(String name)
        {
            this.name = name;
        }

        public String Name { get { return this.name; } }
    }
}
