using System;
using System.Timers;
using UnityEngine;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod
{
    public abstract class PluginComponent : MonoBehaviour
    {
        private const int DEFAULT_UPDATES_PER_SECOND = 1;
        private const int MAX_UPDATES_PER_SECOND = 10;

        protected static class Mouse
        {
            public const int LEFT = 0;
            public const int RIGHT = 1;
            public const int MIDDLE = 2;
            public const String SCROLL_WHEEL = "Mouse ScrollWheel";
        }

        protected readonly GUIManager guiManager = GUIManager.getInstance();
        protected readonly TimeManager timeManager = TimeManager.getInstance();
        protected readonly WorldManager worldManager = WorldManager.getInstance();

        protected ControlPlayer controlPlayer { get; private set; }
        protected bool isExceptionThrown { get; private set; }

        private Timer updateTimer = new Timer();
        private int updatesPerSecond = DEFAULT_UPDATES_PER_SECOND;

        public abstract void OnStart();
        public virtual void OnUpdate() { }
        public virtual void OnInput() { }

        public PluginComponent()
        {
            isExceptionThrown = false;
            controlPlayer = guiManager.controllerObj.GetComponent<ControlPlayer>();
        }

        public void Start()
        {
            try { OnStart(); } 
            catch (Exception e) { log(e); }

            updateTimer.Interval = 1000 / updatesPerSecond;
            updateTimer.Elapsed += updateTimer_Elapsed;
            updateTimer.Stop();
            updateTimer.Start();
        }

        public void Update()
        {
            if (isExceptionThrown) return;

            if (Input.mousePresent || Input.anyKey)
            {
                try { OnInput(); }
                catch (Exception e) { log(e); }                
            }
        }

        private void updateTimer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (isExceptionThrown)
            {
                updateTimer.Stop();
                return;
            }

            try { OnUpdate(); }
            catch (Exception ex) { log(ex); }
        }
        
        protected void setUpdatesPerSecond(int updatesPerSecond)
        {
            this.updatesPerSecond = updatesPerSecond > 0 ? updatesPerSecond <= MAX_UPDATES_PER_SECOND ? updatesPerSecond : MAX_UPDATES_PER_SECOND : DEFAULT_UPDATES_PER_SECOND;
        }
        
        protected bool isMouseInWorld(out Vector3 worldPosition)
        {
            try
            {
                worldPosition = controlPlayer.WorldPositionAtMouse();
                return true;
            }
            catch (Exception e)
            {
                worldPosition = Vector3.zero;
                return false;
            }
        }        

        protected void log<T>(T obj)
        {
            if (obj is Exception)
            {
                Exception e = obj as Exception;
                guiManager.AddTextLine(String.Format("{0}: {1}", e.GetType(), e.Message));

                isExceptionThrown = true;
            }
            else guiManager.AddTextLine(Convert.ToString(obj));
        } 
    }
}
