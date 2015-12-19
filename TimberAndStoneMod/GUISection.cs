using Plugin.BlowyAsteroid.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Plugin.BlowyAsteroid
{
    public class GUISection
    {
        public enum FlowDirection { VERTICAL, HORIZONTAL }
        public enum Overflow { HIDDEN, SCROLL, WRAP }
        
        public const float DEFAULT_CONTROL_WIDTH = 256f;
        public const float DEFAULT_CONTROL_HEIGHT = 32f;
        public const float DEFAULT_CONTROL_MARGIN = 16f;
        public const float DEFAULT_CONTROL_PADDING = 4f;
        
        public FlowDirection Direction { get; set; }
        public Overflow Flow { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }
        public float ControlPadding { get; set; }
        public float ControlMargin { get; set; }
        public float ControlWidth { get; set; }
        public float ControlHeight { get; set; }
        public float X { get; set; }
        public float Y { get; set; }

        public bool isHasBegun { get; private set; }
        public bool isScrolling { get; private set; }

        private int currentXIndex { get; set; }
        private int currentYIndex { get; set; }

        private Rect scrollContainer = new Rect();
        private Rect scrollViewContainer = new Rect();
        private Vector2 scrollPosition = Vector2.zero;

        private readonly GUIManager guiManager = GUIManager.getInstance();

        public GUISection()
        {
            this.ControlPadding = DEFAULT_CONTROL_PADDING;
            this.ControlMargin = DEFAULT_CONTROL_MARGIN;
            this.ControlWidth = DEFAULT_CONTROL_WIDTH;
            this.ControlHeight = DEFAULT_CONTROL_HEIGHT;
            this.Direction = GUISection.FlowDirection.VERTICAL;
            this.Flow = GUISection.Overflow.SCROLL;
        }

        private const float SCROLL_BAR_SIZE = 20f;
        public void Begin(float startX, float startY, float width, float height)
        {
            this.X = startX + this.ControlMargin;
            this.Y = startY + this.ControlMargin;
            this.Width = width - this.ControlMargin * 2;
            this.Height = height - this.ControlMargin * 2; ;
            this.currentXIndex = 0;
            this.currentYIndex = 0;

            if (this.Flow == Overflow.SCROLL || this.Flow == Overflow.WRAP)
            {
                if (!this.isHasBegun)
                {                    
                    this.scrollContainer.Set(this.X, this.Y, this.Width, this.Height);
                }

                this.scrollViewContainer.Set(this.X, this.Y, this.Width, this.Height);
                scrollPosition = GUI.BeginScrollView(scrollViewContainer, scrollPosition, scrollContainer);
            }
            
            this.isScrolling = false;
            this.isHasBegun = true;
        }

        public void End()
        {
            if (this.Flow == Overflow.SCROLL || this.Flow == Overflow.WRAP)
            {
                GUI.EndScrollView();
            }
        }

        public void Label(String text)
        {
            guiManager.DrawTextCenteredWhite(this.ControlPosition, text);
            updateControlIndexes();
        }

        public bool Button(String text)
        {
            return updateControlIndexes(guiManager.DrawButton(this.ControlPosition, text));
        }

        public bool Button(String text, ref bool doOnClick)
        {
            return updateControlIndexes(guiManager.DrawButton(this.ControlPosition, text) ? (doOnClick = true) : doOnClick);            
        }

        public bool CheckBox(String text, ref bool toggled)
        {
            return updateControlIndexes(guiManager.DrawCheckBox(this.ControlPosition, text, ref toggled));            
        }

        public bool CheckBox(String text, ref bool toggled, ref bool doOnClick)
        {
            return updateControlIndexes(guiManager.DrawCheckBox(this.ControlPosition, text, ref toggled) ? (doOnClick = true) : doOnClick);
        }

        private void updateScrollContainer(float width, float height)
        {
            this.scrollContainer.Set(this.X, this.Y, width, height);
        }

        private bool updateControlIndexes(bool flag = false)
        {
            this.isScrolling = false;

            if (this.Direction == FlowDirection.VERTICAL)
            {
                this.currentYIndex++;
               
                switch (this.Flow)
                {
                    case Overflow.WRAP:
                        if (this.controlYPosition + this.ControlHeight > this.Y + this.Height)
                        {
                            this.currentYIndex = 0;
                            this.currentXIndex++;

                            if (this.controlXPosition + this.ControlWidth > this.Width)
                            {
                                this.isScrolling = true;     
                            }

                            updateScrollContainer(this.controlXPosition - this.X, this.scrollViewContainer.height - SCROLL_BAR_SIZE);
                        }

                        break;

                    case Overflow.SCROLL:
                        if (this.controlYPosition > this.Y + this.Height)
                        {
                            this.isScrolling = true;                                
                        }

                        updateScrollContainer(this.scrollViewContainer.width - SCROLL_BAR_SIZE, this.controlYPosition - this.Y);

                        break;
                }
               
            }
            else if (this.Direction == FlowDirection.HORIZONTAL)
            {
                this.currentXIndex++;
                
                switch (this.Flow)
                {
                    case Overflow.WRAP:
                        if (this.controlXPosition + this.ControlWidth > this.X + this.Width)
                        {
                            this.currentXIndex = 0;
                            this.currentYIndex++;

                            if (this.controlYPosition + this.ControlHeight > this.Height)
                            {
                                this.isScrolling = true;
                            }

                            updateScrollContainer(this.scrollViewContainer.width - SCROLL_BAR_SIZE, this.controlYPosition - this.Y);
                        }

                        break;

                    case Overflow.SCROLL:
                        if (this.controlXPosition > this.X + this.Width)
                        {
                            this.isScrolling = true;
                        }

                        updateScrollContainer(this.controlXPosition - this.X, this.scrollViewContainer.height - SCROLL_BAR_SIZE);

                        break;
                }  
            }

            return flag;
        }

        public float controlYPosition
        {
            get { return this.Y + this.currentYIndex * (this.ControlHeight + this.ControlPadding); }
        }

        public float controlXPosition
        {
            get { return this.X + this.currentXIndex * (this.ControlWidth + this.ControlPadding); }
        }

        public Rect ControlPosition 
        { 
            get { return RectangleUtils.get(this.controlXPosition, this.controlYPosition, this.ControlWidth, this.ControlHeight); } 
        }

        public float nextControlYPosition
        {
            get { return this.Y + (this.currentYIndex + 1) * (this.ControlHeight + this.ControlPadding); }
        }

        public float nextControlXPosition
        {
            get { return this.X + (this.currentXIndex + 1) * (this.ControlWidth + this.ControlPadding); }
        }

        public bool hasChildren { get { return this.currentXIndex > 0 || this.currentYIndex > 0; } }
    }
}
