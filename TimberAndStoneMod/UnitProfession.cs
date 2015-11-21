using System;
using System.Linq;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod
{
    public class UnitProfession
    {
        public const int MAX_LEVEL = 20;

        public static UnitProfession Archer = new UnitProfession("archer");
        public static UnitProfession Blacksmith = new UnitProfession("blacksmith");
        public static UnitProfession Builder = new UnitProfession("builder");
        public static UnitProfession Carpenter = new UnitProfession("carpenter");
        public static UnitProfession Farmer = new UnitProfession("farmer");
        public static UnitProfession Fisherman = new UnitProfession("fisherman");
        public static UnitProfession Forager = new UnitProfession("forager");
        public static UnitProfession Herder = new UnitProfession("herder");
        public static UnitProfession Infantry = new UnitProfession("infantry");
        public static UnitProfession Miner = new UnitProfession("miner");
        public static UnitProfession StoneMason = new UnitProfession("stone mason");
        public static UnitProfession Tailor = new UnitProfession("tailor");
        public static UnitProfession Trader = new UnitProfession("trader");
        public static UnitProfession WoodChopper = new UnitProfession("wood chopper");

        public static UnitProfession[] List = new UnitProfession[]{
            Archer, Blacksmith, Builder, Carpenter, Farmer, Fisherman, 
            Forager, Herder, Infantry, Miner, StoneMason, Tailor, Trader, WoodChopper
        };

        public static UnitProfession Random { get { return List[UnityEngine.Random.Range(0, List.Count())]; } }

        private String name;

        private UnitProfession(String name)
        {
            this.name = name;
        }

        public String getName() { return this.name; }
    }
}
