using Plugin.BlowyAsteroid.TimberAndStoneMod.Services;
using Plugin.BlowyAsteroid.TimberAndStoneMod.Components;
using Timber_and_Stone.API;
using Timber_and_Stone.API.Event;
using Timber_and_Stone.Event;
using UnityEngine;
using System;
using System.Linq;
using Timber_and_Stone.Tasks;

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
            ModComponent.addComponent(typeof(FriendlyNpcComponent));
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
            if (UnitPreference.isFriendlyNPC(evt.getUnit()))
            {
                evt.result = Result.Deny;
            }

            if (!ModSettings.isPreventDeathEnabled || !UnitService.isFriendly(evt.getUnit())) return;
            
            evt.result = Result.Deny;
        }

        [Timber_and_Stone.API.Event.EventHandler(Priority.Monitor)]
        public void onEntityDeathMonitor(EventEntityDeath evt)
        {
            if (!UnitService.isFriendly(evt.getUnit()))
            {
                FriendlyNpcComponent.updateNPCsNear(evt.getUnit());
            }

            if (evt.result != Result.Deny) return;

            healUnit(evt.getUnit());
        }

        private void healUnit(ALivingEntity entity)
        {
            entity.hitpoints = entity.maxHP;
            entity.hunger = 0f;
        }
    }
}

