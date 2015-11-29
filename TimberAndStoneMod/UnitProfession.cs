using System;
using System.Linq;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod
{
    public class UnitProfession
    {
        public const int MAX_LEVEL = 20;

        public static UnitProfession Archer = new UnitProfession("Archer");
        public static UnitProfession Blacksmith = new UnitProfession("Blacksmith");
        public static UnitProfession Builder = new UnitProfession("Builder");
        public static UnitProfession Carpenter = new UnitProfession("Carpenter");
        public static UnitProfession Engineer = new UnitProfession("Engineer");
        public static UnitProfession Farmer = new UnitProfession("Farmer");
        public static UnitProfession Fisherman = new UnitProfession("Fisherman");
        public static UnitProfession Forager = new UnitProfession("Forager");
        public static UnitProfession Herder = new UnitProfession("Herder");
        public static UnitProfession Infantry = new UnitProfession("Infantry");
        public static UnitProfession Miner = new UnitProfession("Miner");
        public static UnitProfession StoneMason = new UnitProfession("Stone Mason");
        public static UnitProfession Tailor = new UnitProfession("Tailor");
        public static UnitProfession Trader = new UnitProfession("Trader");
        public static UnitProfession WoodChopper = new UnitProfession("Wood Chopper");

        public static UnitProfession[] List = new UnitProfession[]{
            Archer, Blacksmith, Builder, Carpenter, Engineer, Farmer, Fisherman, 
            Forager, Herder, Infantry, Miner, StoneMason, Tailor, Trader, WoodChopper
        };

        public static UnitProfession Random { get { return List[UnityEngine.Random.Range(0, List.Count())]; } }

        private String name;

        private UnitProfession(String name)
        {
            this.name = name;
        }

        public String Name { get { return this.name; } }
    }
}
