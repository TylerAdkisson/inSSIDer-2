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
using System.Net.NetworkInformation;
using ManagedWifi;

namespace ManagedWifi
{
	class SudoInterface : NetworkInterface
	{
	    private string _id;
	    private string _name;
	    private string _desc;

        public SudoInterface(WlanInterface wlan)
        {
            _id = wlan.InterfaceGuid.ToString();
            //Calling InterfaceName cuases an infinite loop
            //_name = wlan.InterfaceName;
            _desc = wlan.InterfaceDescription;
        }

        public SudoInterface(string id, string description, string name)
        {
            _id = id;
            _name = name;
            _desc = description;
        }

        public override string Description
        {
            get { return _desc; }
        }

        public override IPInterfaceProperties GetIPProperties()
        {
            //throw new NotImplementedException();
            return null;
        }

        public override IPv4InterfaceStatistics GetIPv4Statistics()
        {
            throw new NotImplementedException();
            //return null;
        }

        public override PhysicalAddress GetPhysicalAddress()
        {
            return PhysicalAddress.None;
        }

        public override string Id
        {
            get { return _id; }
        }

        public override bool IsReceiveOnly
        {
            get { return false; }
        }

        public override string Name
        {
            get { return _name; }
        }

        public override NetworkInterfaceType NetworkInterfaceType
        {
            get { return NetworkInterfaceType.Wireless80211; }
        }

        public override OperationalStatus OperationalStatus
        {
            get { return OperationalStatus.Up; }
        }

        public override long Speed
        {
            get { return 0; }
        }

        public override bool Supports(NetworkInterfaceComponent networkInterfaceComponent)
        {
            return true;
        }

        public override bool SupportsMulticast
        {
            get { return false; }
        }
    }
}
