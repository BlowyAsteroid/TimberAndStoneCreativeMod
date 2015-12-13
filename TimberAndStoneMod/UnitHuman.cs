using System;
using System.Collections.Generic;
using System.Linq;
using Timber_and_Stone;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod
{
    public sealed class UnitHuman
    {
        public const int MAX_LEVEL = 20;

        public static readonly UnitHuman Archer = new UnitHuman("Archer");
        public static readonly UnitHuman Blacksmith = new UnitHuman("Blacksmith");
        public static readonly UnitHuman Builder = new UnitHuman("Builder");
        public static readonly UnitHuman Carpenter = new UnitHuman("Carpenter");
        public static readonly UnitHuman Engineer = new UnitHuman("Engineer");
        public static readonly UnitHuman Farmer = new UnitHuman("Farmer");
        public static readonly UnitHuman Fisherman = new UnitHuman("Fisherman");
        public static readonly UnitHuman Forager = new UnitHuman("Forager");
        public static readonly UnitHuman Herder = new UnitHuman("Herder");
        public static readonly UnitHuman Infantry = new UnitHuman("Infantry");
        public static readonly UnitHuman Miner = new UnitHuman("Miner");
        public static readonly UnitHuman StoneMason = new UnitHuman("Stone Mason");
        public static readonly UnitHuman Tailor = new UnitHuman("Tailor");
        public static readonly UnitHuman Trader = new UnitHuman("Trader");
        public static readonly UnitHuman WoodChopper = new UnitHuman("Wood Chopper");

        public static readonly UnitHuman[] List = new UnitHuman[]{
            Archer, Blacksmith, Builder, Carpenter, Engineer, Farmer, Fisherman, 
            Forager, Herder, Infantry, Miner, StoneMason, Tailor, Trader, WoodChopper
        };

        public static UnitHuman Random { get { return List[UnityEngine.Random.Range(0, List.Count())]; } }  

        private String name;

        private UnitHuman(String name)
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
