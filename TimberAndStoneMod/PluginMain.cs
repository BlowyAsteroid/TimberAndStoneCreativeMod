using Plugin.BlowyAsteroid.TimberAndStoneMod.Collections;
using Plugin.BlowyAsteroid.TimberAndStoneMod.Components;
using Plugin.BlowyAsteroid.TimberAndStoneMod.Services;
using System;
using Timber_and_Stone.API;
using Timber_and_Stone.API.Event;
using Timber_and_Stone.Event;
using UnityEngine;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod
{
    public sealed class PluginMain : CSharpPlugin, IEventListener
   {
        private readonly GUIManager guiManager = GUIManager.getInstance();
        private readonly EventManager eventManager = EventManager.getInstance();
        private readonly ModSettings modSettings = ModSettings.getInstance();

        private void log<T>(T obj) { guiManager.AddTextLine(Convert.ToString(obj)); }

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
            addComponent(typeof(GameSaveComponent));
        }

        public override void OnEnable()
        {
            eventManager.Register(this);
        }
    }
}

