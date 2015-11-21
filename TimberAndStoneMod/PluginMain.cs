using Plugin.BlowyAsteroid.TimberAndStoneMod.Services;
using Plugin.BlowyAsteroid.TimberAndStoneMod.Components;
using Timber_and_Stone.API;
using Timber_and_Stone.API.Event;
using Timber_and_Stone.Event;
using UnityEngine;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod
{
    public class PluginMain : CSharpPlugin, IEventListener
   {
        private ModSettings modSettings = ModSettings.getInstance();

        public override void OnLoad()
        {
            ModComponent.addComponent(typeof(GameSpeedComponent));
            ModComponent.addComponent(typeof(CheatMenuComponent));
            ModComponent.addComponent(typeof(CreativeMenuComponent));
            ModComponent.addComponent(typeof(TimeComponent));
        }        

        public override void OnEnable()
        {
            EventManager.getInstance().Register(this);
        }

        [Timber_and_Stone.API.Event.EventHandler(Priority.Normal)]
        public void onInvasionNormal(EventInvasion evt)
        {
            if (!modSettings.isPreventInvasionsEnabled) return;

            evt.result = Result.Deny;
        }

        [Timber_and_Stone.API.Event.EventHandler(Priority.Monitor)]
        public void onInvasionMonitor(EventInvasion evt)
        {
            if (!modSettings.isPreventInvasionsEnabled || evt.result != Result.Deny) return;

            ModComponent.log(evt.invasion.getName() + " invasion cancelled.");
        }

        [Timber_and_Stone.API.Event.EventHandler(Priority.Normal)]
        public void onEntityDeathNormal(EventEntityDeath evt)
        {
            if (!modSettings.isPreventDeathEnabled || !UnitService.isFriendly(evt.getUnit())) return;
            
            evt.result = Result.Deny;
        }

        [Timber_and_Stone.API.Event.EventHandler(Priority.Monitor)]
        public void onEntityDeathMonitor(EventEntityDeath evt)
        {
            if (!modSettings.isPreventDeathEnabled || evt.result != Result.Deny) return;

            ALivingEntity unit = evt.getUnit();
            unit.hitpoints = unit.maxHP;
            unit.hunger = 0f;               

            ModComponent.log(unit.unitName + " has been brought back to life.");
        }
    }
}

