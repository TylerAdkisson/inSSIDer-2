﻿using System.Windows.Forms;
using System.Drawing;
using System;
namespace inSSIDer.UI.Controls
{
    public class Tab : Panel
    {
        private string _text;
        public Guid Id;
        public bool Selected;

        public float TabWidth;
        //public Font Font;

        public RectangleF _bounds;

        //public Control Contents;

        public Tab(string text, Font font)
        {
            Id = Guid.NewGuid();
            Text = text;
            Font = font == null ? SystemFonts.DefaultFont : font;
        }

        public Tab(string text)
            : this(text, default(Font)) { }

        public override string Text
        {
            get { return _text; }
            set
            {
                //Calculate width
                //Width = TabUtil.MeasureString(value, SystemFonts.DefaultFont).Width + 10;
                _text = value;
            }
        }

        public RectangleF TabBounds
        {
            get { return _bounds;/* == null ? RectangleF.Empty : _bounds;*/ }
            set { if (value != null) { _bounds = value; } }
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Tab)) return false;
            return (obj as Tab).Id.Equals(Id);
        }
    }
}