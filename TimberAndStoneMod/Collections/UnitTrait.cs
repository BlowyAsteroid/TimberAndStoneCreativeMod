using System;
using System.Linq;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod.Collections
{
    public enum UnitTraitType { GOOD, BAD }

    public sealed class UnitTrait : IUnitSettingCollectionItem
    {
        public static readonly UnitTrait Athletic = new UnitTrait("trait.athletic", UnitTraitType.GOOD, "Athletic");
        public static readonly UnitTrait Charismatic = new UnitTrait("trait.charismatic", UnitTraitType.GOOD, "Charismatic");
        public static readonly UnitTrait Courageous = new UnitTrait("trait.courageous", UnitTraitType.GOOD, "Courageous");
        public static readonly UnitTrait GoodVision = new UnitTrait("trait.goodvision", UnitTraitType.GOOD, "Good Vision");
        public static readonly UnitTrait Hardworker = new UnitTrait("trait.hardworker", UnitTraitType.GOOD, "Hard Worker");
        public static readonly UnitTrait StrongBack = new UnitTrait("trait.strongback", UnitTraitType.GOOD, "Strong Back");
        public static readonly UnitTrait QuickLearner = new UnitTrait("trait.quicklearner", UnitTraitType.GOOD, "Quick Learner");
        public static readonly UnitTrait BadVision = new UnitTrait("trait.badvision", UnitTraitType.BAD, "Bad Vision");
        public static readonly UnitTrait Clumsy = new UnitTrait("trait.clumsy", UnitTraitType.BAD, "Clumsy");
        public static readonly UnitTrait Cowardly = new UnitTrait("trait.cowardly", UnitTraitType.BAD, "Cowardly");
        public static readonly UnitTrait Disloyal = new UnitTrait("trait.disloyal", UnitTraitType.BAD, "Disloyal");
        public static readonly UnitTrait Lazy = new UnitTrait("trait.lazy", UnitTraitType.BAD, "Lazy");
        public static readonly UnitTrait Overeater = new UnitTrait("trait.overeater", UnitTraitType.BAD, "Overeater");
        public static readonly UnitTrait Sluggish = new UnitTrait("trait.sluggish", UnitTraitType.BAD, "Sluggish");
        public static readonly UnitTrait WeakBack = new UnitTrait("trait.weakback", UnitTraitType.BAD, "Weak Back");

        public static readonly UnitTrait[] List = new UnitTrait[] {
            Athletic, Charismatic, Courageous, GoodVision, Hardworker, StrongBack, QuickLearner,
            BadVision, Clumsy, Cowardly, Disloyal, Lazy, Overeater, Sluggish, WeakBack
        };

        public static UnitTrait Random { get { return List[UnityEngine.Random.Range(0, List.Count())]; } }

        private String name;
        private String description;
        private UnitTraitType type;

        private UnitTrait(String name, UnitTraitType type, String description)
        {
            this.name = name;
            this.type = type;
            this.description = description;
        }

        public String Name { get { return this.name; } }
        public String Description { get { return this.description; } }
        public UnitTraitType Type { get { return this.type; } }

        public static void setBestTraits(APlayableEntity entity)
        {
            foreach (UnitTrait trait in UnitTrait.List)
            {
                entity.preferences[trait.Name] = trait.Type == UnitTraitType.GOOD;
            }
        }
    }
}
