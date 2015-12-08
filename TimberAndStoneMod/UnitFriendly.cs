using System;
using System.Collections.Generic;
using System.Linq;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod
{
    public class UnitFriendly
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

        public static readonly UnitFriendly GoblinInfantry = new UnitFriendly(GOBLIN_INFANTRY);
        public static readonly UnitFriendly GoblinArcher = new UnitFriendly(GOBLIN_ARCHER);
        public static readonly UnitFriendly SkeletonInfantry = new UnitFriendly(SKELETON_INFANTRY);
        public static readonly UnitFriendly SkeletonArcher = new UnitFriendly(SKELETON_ARCHER);
        public static readonly UnitFriendly HumanInfantry = new UnitFriendly(HUMAN_INFANTRY);
        public static readonly UnitFriendly HumanArcher = new UnitFriendly(HUMAN_ARCHER);

        public static readonly UnitFriendly[] List = new UnitFriendly[] 
        {
            HumanInfantry, HumanArcher, GoblinInfantry, GoblinArcher, SkeletonInfantry, SkeletonArcher
        };

        public static UnitFriendly Random { get { return List[UnityEngine.Random.Range(0, List.Count())]; } }


        private String name;

        private UnitFriendly(String name)
        {
            this.name = name;
        }

        public String Name { get { return this.name; } }
    }
}
