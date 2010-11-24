using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace inSSIDer.Misc
{
    static class TabUtil
    {
        static Graphics g;

        static TabUtil()
        {
            // Create graphics for measuring strings and stuff
            g = Graphics.FromImage(new Bitmap(1, 1));
        }

        public static SizeF MeasureString(string text, Font font)
        {
            return g.MeasureString(text, font);
        }
    }
}
