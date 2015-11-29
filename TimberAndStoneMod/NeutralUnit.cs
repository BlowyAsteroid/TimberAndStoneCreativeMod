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

        public static NeutralUnit GoblinInfantry = new NeutralUnit(GOBLIN_INFANTRY);
        public static NeutralUnit GoblinArcher = new NeutralUnit(GOBLIN_ARCHER);
        public static NeutralUnit SkeletonInfantry = new NeutralUnit(SKELETON_INFANTRY);
        public static NeutralUnit SkeletonArcher = new NeutralUnit(SKELETON_ARCHER);
        public static NeutralUnit HumanInfantry = new NeutralUnit(HUMAN_INFANTRY);
        public static NeutralUnit HumanArcher = new NeutralUnit(HUMAN_ARCHER);

        public static NeutralUnit[] List = new NeutralUnit[] 
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
