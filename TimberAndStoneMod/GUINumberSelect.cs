using Plugin.BlowyAsteroid.TimberAndStoneMod.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Plugin.BlowyAsteroid.TimberAndStoneMod
{
    public class GUINumberSelect
    {
        private const float BUTTON_SIZE = 20f;

        public String Name { get; set; }
        public float Value { get; set; }
        public float Min { get; set; }
        public float Max { get; set; }
        public float Increment { get; set; }

        public float X { get; set; }
        public float Y { get; set; }
        public float Width { get; set; }
        public float Height { get; set; }

        private readonly GUIManager guiManager = GUIManager.getInstance();
        
        public GUINumberSelect(String name = "", float value = 0f, float min = 0f, float max = 100f, float increment = 1f)
        {
            this.Name = name;
            this.Value = value;
            this.Min = min;
            this.Max = max;
            this.Increment = increment;
        }

        public bool Draw(Rect container)
        {
            return this.Draw(container.x, container.y, container.width, container.height);
        }

        private float tempX, tempWidth;
        private bool isValueChanged;
        public bool Draw(float x, float y, float width, float height)        
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;

            isValueChanged = false;
            tempX = x;
            tempWidth = 0;

            if (this.Name.Trim() != String.Empty)
            {
                tempWidth = this.Name.Length * 10;
                guiManager.DrawTextLeftBlack(RectangleUtils.get(tempX, y, tempWidth), this.Name);
                tempX += tempWidth;
            }

            if (GUI.Button(RectangleUtils.get(tempX, y, BUTTON_SIZE), String.Empty, guiManager.minusButtonStyle) && this.Value > this.Min)
            {
                this.Value--;
                isValueChanged = true;
            }      

            tempX += BUTTON_SIZE;

            tempWidth = width - BUTTON_SIZE * 2 - tempWidth;
            guiManager.DrawTextCenteredBlack(RectangleUtils.get(tempX, y, tempWidth), this.Value.ToString());
            tempX += tempWidth;

            if (GUI.Button(RectangleUtils.get(tempX, y, BUTTON_SIZE), String.Empty, guiManager.plusButtonStyle) && this.Value < this.Max)
            {
                this.Value++;
                isValueChanged = true;
            }

            return isValueChanged;
        }
    }
}
