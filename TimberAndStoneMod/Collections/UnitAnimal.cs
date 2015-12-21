using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod.Collections
{
    public sealed class UnitAnimal : IUnitCollectionItem
    {
        public static readonly UnitAnimal Boar = new UnitAnimal("Boar");
        public static readonly UnitAnimal Sheep = new UnitAnimal("Sheep");
        public static readonly UnitAnimal Chicken = new UnitAnimal("Chicken");

        public static readonly UnitAnimal[] List = new UnitAnimal[] 
        {
            Boar, Chicken, Sheep
        };

        public static UnitAnimal Random { get { return List[UnityEngine.Random.Range(0, List.Count())]; } }

        private String name;
        private UnitAnimal(String name)
        {
            this.name = name;            
        }
        public String Name { get { return this.name; } }
        public String Description { get { return this.name; } }
    }
}
