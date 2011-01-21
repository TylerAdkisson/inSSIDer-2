using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace inSSIDer.UI.Theme
{
    public interface IThemeable
    {
        /// <summary>
        /// Sets the controls's color scheme
        /// </summary>
        /// <param name="scheme">The scheme to use</param>
        void SetColorScheme(ColorScheme scheme);
    }
}
