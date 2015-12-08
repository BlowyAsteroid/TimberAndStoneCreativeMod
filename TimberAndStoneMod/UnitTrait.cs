﻿using System;
using System.Linq;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod
{
    public enum UnitTraitType { GOOD, BAD }

    public class UnitTrait
    {
        public static readonly UnitTrait Athletic = new UnitTrait("trait.athletic", UnitTraitType.GOOD);
        public static readonly UnitTrait Charismatic = new UnitTrait("trait.charismatic", UnitTraitType.GOOD);
        public static readonly UnitTrait Courageous = new UnitTrait("trait.courageous", UnitTraitType.GOOD);
        public static readonly UnitTrait GoodVision = new UnitTrait("trait.goodvision", UnitTraitType.GOOD);
        public static readonly UnitTrait Hardworker = new UnitTrait("trait.hardworker", UnitTraitType.GOOD);
        public static readonly UnitTrait StrongBack = new UnitTrait("trait.strongback", UnitTraitType.GOOD);
        public static readonly UnitTrait QuickLearner = new UnitTrait("trait.quicklearner", UnitTraitType.GOOD);
        public static readonly UnitTrait BadVision = new UnitTrait("trait.badvision", UnitTraitType.BAD);
        public static readonly UnitTrait Clumsy = new UnitTrait("trait.clumsy", UnitTraitType.BAD);
        public static readonly UnitTrait Cowardly = new UnitTrait("trait.cowardly", UnitTraitType.BAD);
        public static readonly UnitTrait Disloyal = new UnitTrait("trait.disloyal", UnitTraitType.BAD);
        public static readonly UnitTrait Lazy = new UnitTrait("trait.lazy", UnitTraitType.BAD);
        public static readonly UnitTrait Overeater = new UnitTrait("trait.overeater", UnitTraitType.BAD);
        public static readonly UnitTrait Sluggish = new UnitTrait("trait.sluggish", UnitTraitType.BAD);
        public static readonly UnitTrait WeakBack = new UnitTrait("trait.weakback", UnitTraitType.BAD);

        public static readonly UnitTrait[] List = new UnitTrait[] {
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
