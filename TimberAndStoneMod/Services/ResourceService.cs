using Plugin.BlowyAsteroid.TimberAndStoneMod.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using Timber_and_Stone;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod.Services
{
    public sealed class ResourceService
    {
        private static readonly ResourceService instance = new ResourceService();
        public static ResourceService getInstance() { return instance; }

        public struct OtherStorageTypes
        {
            public static readonly StorageType None = new StorageType(0);
            public static readonly StorageType All = new StorageType(99);
        }

        public static readonly StorageType[] StorageTypes = new StorageType[] 
        { 
            OtherStorageTypes.None, StorageType.Food, StorageType.Wood, StorageType.Masonry, StorageType.Metals, StorageType.Wheat, 
            StorageType.Tools, StorageType.Weapons, StorageType.Armor, StorageType.Treasure, StorageType.Misc
        };

        private const float DEFAULT_RESOURCE_FILL_PERCENTAGE = 1.0f;
        private const float MINIMUM_RESOURCE_MASS = 0.01f;

        private Dictionary<Resource, float> originalMasses = new Dictionary<Resource, float>();

        private ResourceManager resourceManager = ResourceManager.getInstance();
        private WorldManager worldManager = WorldManager.getInstance();

        private ResourceStorage storage;

        private ResourceService()
        {
            storage = worldManager.PlayerFaction.storage as ResourceStorage;
        }

        public List<Resource> getRawMaterials()
        {
            return getAllResources().Where(r => ResourceId.RawMaterialList.Contains(r.index)).ToList();
        }

        public List<Resource> getProcessedMaterials()
        {
            return getAllResources().Where(r => ResourceId.ProcessedMaterialList.Contains(r.index)).ToList();
        }

        public List<Resource> getAllResources()
        {
            return resourceManager.resources.Where(r => r != null && ResourceId.AllPlayerResourcesList.Contains(r.index)).ToList();
        } 

        public List<BuildStructure> getStorageStructures()
        {
            return worldManager.PlayerFaction.structures.Where(s => s.storageAmount > 0).ToList();
        }

        public void addStorageCap(StorageType type, float amount)
        {
            storage.setStorageCap(type, storage.getStorageCap(type) + amount);
        }

        public void addStorageCap(BuildStructure structure)
        {
            if (structure.storageIndex == OtherStorageTypes.All)
            {
                addAllStorageCaps(structure.storageAmount);
            }
            else addStorageCap(StorageTypes[structure.storageIndex], structure.storageAmount);
        }

        public void setStorageCap(StorageType type, float amount)
        {
            storage.setStorageCap(type, amount);
        }

        public float getStorageCap(StorageType type)
        {
            return storage.getStorageCap(type);
        }

        public int getStorageItemCount(StorageType type)
        {
            return getAllResources().Where(r => r.storageIndex == type.index).Count();
        }

        public void setAllStorageCaps(float amount)
        {
            foreach (StorageType type in ResourceService.StorageTypes)
            {
                setStorageCap(type, amount);
            }
        }

        public void addAllStorageCaps(float amount)
        {
            foreach (StorageType type in ResourceService.StorageTypes)
            {
                setStorageCap(type, getStorageCap(type) + amount);
            }
        }

        public int getResourceAmount(Resource resource)
        {
            return storage.getResource(resource);
        }

        public bool removeResource(Resource resource, int amount)
        {
            return storage.removeResource(resource, amount);
        }

        public void addResource(Resource resource, int amount)
        {
            storage.addResource(resource, amount);
        }

        public void setResource(Resource resource, int amount)
        {
            if (removeResource(resource, getResourceAmount(resource)))
            {
                storage.addResource(resource, amount);
            }
        }

        public void addStructureToFactionStorage(BuildStructure structure, IFaction faction)
        {
            if (!structure.CheckResources())
            {
                for (int i = 0; i < structure.resourceTypes.Length; i++)
                {
                    storage.addResource(Resource.FromID(structure.resourceTypes[i]), structure.resourceAmounts[i]);
                }
            }

            structure.faction = faction;
            addStorageCap(structure);
        }

        private int currentResourceAmount;
        public void setResourceMass(Resource resource, float mass)
        {
            removeResource(resource, currentResourceAmount = getResourceAmount(resource));
            resource.mass = mass;
            addResource(resource, currentResourceAmount);
        }

        private int storageItemCount, newQuantity;
        private double availableMassPerItem;
        public void fillResourceStorage(Resource resource, float percentage = DEFAULT_RESOURCE_FILL_PERCENTAGE)
        {
            if ((storageItemCount = getStorageItemCount(resource.storageIndex)) == 0 || resource.mass <= 0) return;

            if (ResourceId.SeedsList.Contains(resource.index)) 
            { 
                percentage *= .25f; 
            }
            else if (ResourceId.FOOD == resource.index)
            {
                percentage *= 7f;
            }

            availableMassPerItem = getStorageCap(resource.storageIndex) * percentage / storageItemCount;
            newQuantity = Convert.ToInt32(availableMassPerItem / resource.mass);

            setResource(resource, newQuantity);
        }

        private const float INCREASE_MULTIPLIER = .1f;
        float cap, available, used;
        public void makeStorageRoom(float increasePercentage = 1.5f)
        {
            foreach (StorageType type in StorageTypes)
            {
                cap = storage.getStorageCap(type);
                available = storage.getStorageAvailable(type);
                used = cap - available;
                if (available / cap < increasePercentage * INCREASE_MULTIPLIER)
                {
                    setStorageCap(type, used * increasePercentage);
                }
            }
        }

        public void restoreStorageCaps()
        {
            setAllStorageCaps(0);
            getStorageStructures().ForEach(s => addStorageCap(s));
        }

        public void lowerResourceMass(Resource resource)
        {
            if (!originalMasses.ContainsKey(resource))
            {
                originalMasses.Add(resource, resource.mass);
            }
            setResourceMass(resource, MINIMUM_RESOURCE_MASS);
        }

        public void restoreResourceMass(Resource resource, float originalMass = float.PositiveInfinity)
        {
            if (originalMasses.TryGetValue(resource, out originalMass))
            {
                setResourceMass(resource, originalMass);
            }
        }  
    }
}
