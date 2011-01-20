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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using inSSIDer.Properties;

namespace inSSIDer.Misc
{
    public class SignalBars
    {
        public static Image GetImage(int rssi, bool secure)
        {
            if (rssi >= -54)
            {
                return secure ? Resources.Signal5E : Resources.Signal5;
            }
            else if(rssi >= -59)
            {
                return secure ? Resources.Signal4E : Resources.Signal4;
            }
            else if (rssi >= -69)
            {
                return secure ? Resources.Signal3E : Resources.Signal3;
            }
            else if (rssi >= -79)
            {
                return secure ? Resources.Signal2E : Resources.Signal2;
            }
            else if (rssi >= -89)
            {
                return secure ? Resources.Signal1E : Resources.Signal1;
            }
            else
            {
                return secure ? Resources.Signal0E : Resources.Signal0;
            }
        }
    }
}
