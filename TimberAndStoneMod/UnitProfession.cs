using System;
using System.Collections.Generic;
using System.Linq;
using Timber_and_Stone;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod
{
    public class UnitProfession
    {
        public const int MAX_LEVEL = 20;

        public static readonly UnitProfession Archer = new UnitProfession("Archer");
        public static readonly UnitProfession Blacksmith = new UnitProfession("Blacksmith");
        public static readonly UnitProfession Builder = new UnitProfession("Builder");
        public static readonly UnitProfession Carpenter = new UnitProfession("Carpenter");
        public static readonly UnitProfession Engineer = new UnitProfession("Engineer");
        public static readonly UnitProfession Farmer = new UnitProfession("Farmer");
        public static readonly UnitProfession Fisherman = new UnitProfession("Fisherman");
        public static readonly UnitProfession Forager = new UnitProfession("Forager");
        public static readonly UnitProfession Herder = new UnitProfession("Herder");
        public static readonly UnitProfession Infantry = new UnitProfession("Infantry");
        public static readonly UnitProfession Miner = new UnitProfession("Miner");
        public static readonly UnitProfession StoneMason = new UnitProfession("Stone Mason");
        public static readonly UnitProfession Tailor = new UnitProfession("Tailor");
        public static readonly UnitProfession Trader = new UnitProfession("Trader");
        public static readonly UnitProfession WoodChopper = new UnitProfession("Wood Chopper");

        public static readonly UnitProfession[] List = new UnitProfession[]{
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



        private static Dictionary<APlayableEntity, Dictionary<AProfession, int>> originalProfessions
            = new Dictionary<APlayableEntity, Dictionary<AProfession, int>>();

        private static Dictionary<AProfession, int> professions;
        public static void setAllProfessionsMax(APlayableEntity entity)
        {
            professions = null;

            if (!originalProfessions.ContainsKey(entity))
            {
                professions = new Dictionary<AProfession, int>();
            }

            foreach (KeyValuePair<Type, AProfession> key in entity.professions)
            {
                if (professions != null)
                {
                    professions.Add(key.Value, key.Value.getLevel());
                }

                key.Value.setLevel(AProfession.maxLevel);
            }

            if (professions != null)
            {
                originalProfessions.Add(entity, professions);
            }
        }

        private static Dictionary<AProfession, int> existingProfessions;
        private static int existingLevel;
        public static void restoreProfessions(APlayableEntity entity)
        {
            if (originalProfessions.TryGetValue(entity, out existingProfessions))
            {
                foreach (KeyValuePair<Type, AProfession> key in entity.professions)
                {
                    if (existingProfessions.TryGetValue(key.Value, out existingLevel))
                    {
                        key.Value.setLevel(existingLevel);
                    }
                }

                originalProfessions.Remove(entity);
            }
        }
    }
}
