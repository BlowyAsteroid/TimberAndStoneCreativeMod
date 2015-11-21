using System;
using System.Linq;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod
{
    public enum UnitTraitType { GOOD, BAD }

    public class UnitTrait
    {
        public static UnitTrait Athletic = new UnitTrait("trait.athletic", UnitTraitType.GOOD);
        public static UnitTrait Charismatic = new UnitTrait("trait.charismatic", UnitTraitType.GOOD);
        public static UnitTrait Courageous = new UnitTrait("trait.courageous", UnitTraitType.GOOD);
        public static UnitTrait GoodVision = new UnitTrait("trait.goodvision", UnitTraitType.GOOD);
        public static UnitTrait Hardworker = new UnitTrait("trait.hardworker", UnitTraitType.GOOD);
        public static UnitTrait StrongBack = new UnitTrait("trait.strongback", UnitTraitType.GOOD);
        public static UnitTrait QuickLearner = new UnitTrait("trait.quicklearner", UnitTraitType.GOOD);
        public static UnitTrait BadVision = new UnitTrait("trait.badvision", UnitTraitType.BAD);
        public static UnitTrait Clumsy = new UnitTrait("trait.clumsy", UnitTraitType.BAD);
        public static UnitTrait Cowardly = new UnitTrait("trait.cowardly", UnitTraitType.BAD);
        public static UnitTrait Disloyal = new UnitTrait("trait.disloyal", UnitTraitType.BAD);
        public static UnitTrait Lazy = new UnitTrait("trait.lazy", UnitTraitType.BAD);
        public static UnitTrait Overeater = new UnitTrait("trait.overeater", UnitTraitType.BAD);
        public static UnitTrait Sluggish = new UnitTrait("trait.sluggish", UnitTraitType.BAD);
        public static UnitTrait WeakBack = new UnitTrait("trait.weakback", UnitTraitType.BAD);

        public static UnitTrait[] List = new UnitTrait[] {
            Athletic, Sluggish, Clumsy, QuickLearner, Cowardly, StrongBack, WeakBack, 
            Hardworker, Lazy, BadVision, GoodVision, Charismatic, Courageous, Disloyal, Overeater
        };

        public static UnitTrait Random { get { return List[UnityEngine.Random.Range(0, List.Count())]; } }

        private String name;
        private UnitTraitType type;

        private UnitTrait(String name, UnitTraitType type)
        {
            this.name = name;
            this.type = type;
        }

        public String Name { get { return this.name; } }
        public UnitTraitType Type { get { return this.type; } }
    }
}
