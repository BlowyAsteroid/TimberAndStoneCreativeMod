using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod
{
    public interface IModCollectionItem
    {
        String Name { get; }
        String Description { get; }
    }
}
