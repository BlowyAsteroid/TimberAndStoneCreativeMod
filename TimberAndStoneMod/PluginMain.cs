using Plugin.BlowyAsteroid.TimberAndStoneMod.Services;
using Plugin.BlowyAsteroid.TimberAndStoneMod.Components;
using Timber_and_Stone.API;
using Timber_and_Stone.API.Event;
using Timber_and_Stone.Event;
using UnityEngine;
using System;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod
{
    public class PluginMain : CSharpPlugin, IEventListener
   {
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
            if (!ModSettings.isPreventInvasionsEnabled) return;

            evt.result = Result.Deny;
        }

        [Timber_and_Stone.API.Event.EventHandler(Priority.Monitor)]
        public void onInvasionMonitor(EventInvasion evt)
        {
            if (evt.result != Result.Deny) return;

            ModComponent.log(String.Format("A {0} invasion has been cancelled.", evt.invasion.getName()));
        }
        
        [Timber_and_Stone.API.Event.EventHandler(Priority.Normal)]
        public void onEntityDeathNormal(EventEntityDeath evt)
        {
            if (!ModSettings.isPreventDeathEnabled || !UnitService.isFriendly(evt.getUnit())) return;
            
            evt.result = Result.Deny;
        }

        [Timber_and_Stone.API.Event.EventHandler(Priority.Monitor)]
        public void onEntityDeathMonitor(EventEntityDeath evt)
        {
            if (evt.result != Result.Deny) return;
                        
            ALivingEntity unit = evt.getUnit();
            unit.hitpoints = unit.maxHP;
            unit.hunger = 0f;
        }
    }
}

