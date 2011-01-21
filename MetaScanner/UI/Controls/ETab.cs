////////////////////////////////////////////////////////////////
//
// Copyright (c) 2007-2011 MetaGeek, LLC
//
// Licensed under the Apache License, Version 2.0 (the "License"); 
// you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at 
//
//	http://www.apache.org/licenses/LICENSE-2.0 
//
// Unless required by applicable law or agreed to in writing, software 
// distributed under the License is distributed on an "AS IS" BASIS, 
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and 
// limitations under the License. 
//
////////////////////////////////////////////////////////////////

using System.Windows.Forms;
using System.Drawing;
using System;
namespace inSSIDer.UI.Controls
{
    public class ETab : Panel
    {
        private string _text;
        public Guid Id;
        public bool Selected;

        public float TabWidth;
        //public Font Font;

        public RectangleF _bounds;

        //public Control Contents;

        public ETab(string text, Font font)
        {
            Id = Guid.NewGuid();
            Text = text;
            Font = font == null ? SystemFonts.DefaultFont : font;
        }

        public ETab(string text)
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
    }
}