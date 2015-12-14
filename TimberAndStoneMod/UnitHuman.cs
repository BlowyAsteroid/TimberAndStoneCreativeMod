using System;
using System.Collections.Generic;
using System.Linq;
using Timber_and_Stone;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod
{
    public sealed class UnitHuman : IModCollectionItem
    {
        public static readonly UnitHuman All = new UnitHuman("All");
        public static readonly UnitHuman Crafter = new UnitHuman("Crafter");
        public static readonly UnitHuman Military = new UnitHuman("Military");

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

        public static UnitHuman valueOf(String professionName)
        {
            foreach (UnitHuman unitType in List)
            {
                if (unitType.Name.ToLower().Equals(professionName.ToLower()))
                {
                    return unitType;
                }
            }

            return null;
        }

        public static readonly UnitHuman[] List = new UnitHuman[]{
            Archer, Blacksmith, Builder, Carpenter, Engineer, Farmer, Fisherman, 
            Forager, Herder, Infantry, Miner, StoneMason, Tailor, Trader, WoodChopper
        };

        public static readonly UnitHuman[] CrafterList = new UnitHuman[] 
        {
            Blacksmith, Carpenter, Engineer, StoneMason, Tailor, Crafter
        };

        public static readonly UnitHuman[] MilitaryList = new UnitHuman[]
        {
            Archer, Infantry, Military
        };

        public static UnitHuman Random { get { return List[UnityEngine.Random.Range(0, List.Count())]; } }

        public static bool isRelatedTo(UnitHuman source, UnitHuman other)
        {
            return source == All 
                || (source == Crafter && CrafterList.Contains(other))
                || (source == Military && MilitaryList.Contains(other))
                || source == other;
        }

        private String name;
        private UnitHuman(String name)
        {
            this.name = name;
        }
        public String Name { get { return this.name; } }
        public String Description { get { return this.name; } }

        public static void setCurrentProfessionMax(APlayableEntity entity)
        {
            entity.getProfession().setExperience(AProfession.maxExperience);
            entity.getProfession().setLevel(AProfession.maxLevel);
        }

        public static void setAllProfessionsMax(APlayableEntity entity)
        {
            foreach (KeyValuePair<Type, AProfession> key in entity.professions)
            {
                key.Value.setExperience(AProfession.maxExperience);
                key.Value.setLevel(AProfession.maxLevel);
            }
        }
    }
}
