using Plugin.BlowyAsteroid.TimberAndStoneMod.Services;
using Plugin.BlowyAsteroid.TimberAndStoneMod.Components;
using Timber_and_Stone.API.Event;
using System;
using Timber_and_Stone.API;
using Timber_and_Stone.Event;
using UnityEngine;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod
{
    public sealed class PluginMain : CSharpPlugin, IEventListener
   {
        private readonly GUIManager guiManager = GUIManager.getInstance();
        private readonly EventManager eventManager = EventManager.getInstance();
        private readonly ModSettings modSettings = ModSettings.getInstance();

        private void log(String message) { guiManager.AddTextLine(message); }
        private void log<T>(T obj) { log(Convert.ToString(obj)); }

        private Component addComponent(Type componentType)
        {
            return guiManager.gameObject.AddComponent(componentType);
        }    

        public override void OnLoad()
        {
            addComponent(typeof(GameSpeedComponent));
            addComponent(typeof(CheatMenuComponent));
            addComponent(typeof(CreativeMenuComponent));
            addComponent(typeof(TimeComponent));
            addComponent(typeof(ModSettingsComponent));
        }

        public override void OnEnable()
        {
            eventManager.Register(this);
        }
        
        [Timber_and_Stone.API.Event.EventHandler(Priority.Normal)]
        public void onInvasionNormal(EventInvasion evt)
        {
            if (!modSettings.isPeacefulEnabled) return;

            evt.result = Result.Deny;
        }

        [Timber_and_Stone.API.Event.EventHandler(Priority.Monitor)]
        public void onInvasionMonitor(EventInvasion evt)
        {
            if (evt.result != Result.Deny) return;

            log(String.Format("A {0} invasion has been cancelled.", evt.invasion.getName()));
        }

        [Timber_and_Stone.API.Event.EventHandler(Priority.Normal)]
        public void onMigrantAcceptNormal(EventMigrantAccept evt)
        {
            UnitPreference.setPreference(evt.unit, UnitPreference.WAIT_IN_HALL_WHILE_IDLE, true);
            UnitPreference.setPreference(evt.unit, UnitPreference.TRAIN_UNDER_LEVEL_3, true);
        }
        
        [Timber_and_Stone.API.Event.EventHandler(Priority.Normal)]
        public void onEntityDeathNormal(EventEntityDeath evt)
        {
            if (!UnitService.isFriendly(evt.getUnit())) return;

            UnitPreference.setPreference(evt.getUnit(), UnitPreference.IS_PLAYER_UNIT, true);

            if (!modSettings.isPeacefulEnabled) return;
            
            evt.result = Result.Deny;
        }

        [Timber_and_Stone.API.Event.EventHandler(Priority.Monitor)]
        public void onEntityDeathMonitor(EventEntityDeath evt)
        {
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

