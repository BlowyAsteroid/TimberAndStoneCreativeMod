using Plugin.BlowyAsteroid.TimberAndStoneMod.Utils;
using System;
using UnityEngine;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod
{
    public abstract class GUIPluginComponent : PluginComponent
    {
        protected const float WINDOW_TITLE_HEIGHT = 28f;

        protected float width { get; private set; }
        protected float height { get; private set; }

        protected bool isComponentVisible { get; set; }
        protected bool isVisibleDuringGameOver { get; set; }
        protected bool isVisibleInMainMenu { get; set; }
        protected bool isVisibleInGame { get; set; }
        protected bool isMouseHover { get; private set; }

        protected int windowId { get; private set; }
        protected String title { get; set; }

        protected GUISection sectionMain { get; private set; }

        private Vector2 mousePosition = Vector2.zero;
        private Rect container = new Rect();

        protected Rect ParentContainer { get { return container; } }

        protected float containerX
        {
            get { return container.x; }
            set { container.x = value; }
        }

        protected float containerY
        {
            get { return container.y; }
            set { container.y = value; }
        }

        protected float containerWidth
        {
            get { return container.width; }
            set { container.width = value; }
        }

        protected float containerHeight
        {
            get { return container.height; }
            set { container.height = value; }
        }

        public abstract void OnDraw(int windowId);

        public GUIPluginComponent()
        {
            isComponentVisible = true;
            isVisibleInGame = true;
            windowId = GUIWindowId.Next;
            title = String.Empty;
            sectionMain = new GUISection();
        }

        public void OnGUI()
        {
            if (isExceptionThrown) return;

            if (isComponentVisible)
            {
                if (Input.mousePresent)
                {
                    translateMouse();
                }

                if ((guiManager.inGame && isVisibleInGame)
                    || (guiManager.gameOver && isVisibleDuringGameOver)
                    || (!guiManager.inGame && isVisibleInMainMenu))
                {
                    container = GUI.Window(windowId, container, OnDraw, String.Empty, guiManager.windowBoxStyle);
                    isMouseHover = container.Contains(mousePosition) ? guiManager.mouseInGUI = true : false;
                }
            }
        }

        protected void setWindowSize(float width, float height)
        {
            this.width = width;
            this.height = height;
            container.Set(container.x, container.y, width, height);
        }

        protected void setWindowPosition(float x, float y)
        {
            container.Set(x, y, container.width, container.height);
        }

        private Vector2 translateMouse()
        {
            mousePosition.Set(Input.mousePosition.x, Screen.height - Input.mousePosition.y);
            return mousePosition;
        } 
        
        protected void Window(String title)
        {
            guiManager.DrawWindow(RectangleUtils.get(0, 0, container.width, container.height), title, false);
        }

        public void Label(float x, float y, float width, float height, String text)
        {
            guiManager.DrawTextCenteredWhite(RectangleUtils.get(x, y, width, height), text);
        }

        public bool Button(float x, float y, float width, float height, String text)
        {
            return guiManager.DrawButton(RectangleUtils.get(x, y, width, height), text);
        }

        public bool Button(float x, float y, float width, float height, String text, ref bool doOnClick)
        {
            return guiManager.DrawButton(RectangleUtils.get(x, y, width, height), text) ? (doOnClick = true) : doOnClick;
        }

        public bool CheckBox(float x, float y, float width, float height, String text, ref bool toggled)
        {
            return guiManager.DrawCheckBox(RectangleUtils.get(x, y, width, height), text, ref toggled);
        }

        public bool CheckBox(float x, float y, float width, float height, String text, ref bool toggled, ref bool doOnClick)
        {
            return guiManager.DrawCheckBox(RectangleUtils.get(x, y, width, height), text, ref toggled) ? (doOnClick = true) : doOnClick;
        }
    }
}
