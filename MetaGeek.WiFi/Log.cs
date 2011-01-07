﻿////////////////////////////////////////////////////////////////
//
// Copyright (c) 2007-2010 MetaGeek, LLC
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
using System.Diagnostics;
using System.IO;

namespace MetaGeek.WiFi
{
    public static class Logger
    {
        [Conditional("DEBUG")]
        public static void Start()
        {
            Log.Enable();
        }
    }

    internal static class Log
    {
        private static bool _init;
        private static bool _enable;

        //public static void WriteLine(string message)
        //{
        //    if(!init) Init();
        //    Debug.WriteLine(message);
        //}

        //public static void WriteLine(string message,string category)
        //{
        //    WriteLine((object)message,category);
        //}

        [Conditional("DEBUG")]
        public static void WriteLine(object message)
        {
            if (!_init) Init();
            Debug.WriteLine(message);
        }

        [Conditional("DEBUG")]
        public static void WriteLine(object message, string category)
        {
            if (!_init) Init();
            Debug.WriteLine(message, string.Format("[{0}-{1}-{2} {3}:{4}:{5}] {6}",
                                            new object[]
                                                {
                                                    DateTime.Now.Month.ToString("D2"),
                                                    DateTime.Now.Day.ToString("D2"),
                                                    DateTime.Now.Year.ToString("D2"),
                                                    DateTime.Now.Hour.ToString("D2"),
                                                    DateTime.Now.Minute.ToString("D2"),
                                                    DateTime.Now.Second.ToString("D2"),
                                                    category
                                                }));
        }

        [Conditional("DEBUG")]
        public static void Write(object message)
        {
            if (!_init) Init();
            Debug.Write(message);
        }

        [Conditional("DEBUG")]
        public static void Write(object message, string category)
        {
            if (!_init) Init();
            Debug.Write(message, string.Format("[{0}-{1}-{2} {3}:{4}:{5}] {6}",
                                               new object[]
                                                   {
                                                       DateTime.Now.Month.ToString("D2"),
                                                       DateTime.Now.Day.ToString("D2"),
                                                       DateTime.Now.Year.ToString("D2"),
                                                       DateTime.Now.Hour.ToString("D2"),
                                                       DateTime.Now.Minute.ToString("D2"),
                                                       DateTime.Now.Second.ToString("D2"),
                                                       category
                                                   }));
        }

        [Conditional("DEBUG")]
        private static void Init()
        {
#if !DEBUG
            return;
#else
            if(!_enable) return;
            string filename = string.Format("in2_MetaGeek.WiFi_{0}-{1}-{2}_{3}_{4}_{5}.log",
                                            new object[]
                                                {
                                                    DateTime.Now.Month.ToString("D2"),
                                                    DateTime.Now.Day.ToString("D2"),
                                                    DateTime.Now.Year.ToString("D2"),
                                                    DateTime.Now.Hour.ToString("D2"),
                                                    DateTime.Now.Minute.ToString("D2"),
                                                    DateTime.Now.Second.ToString("D2")
                                                });
            FileStream fsLog = new FileStream(filename, FileMode.CreateNew, FileAccess.Write);
            Debug.AutoFlush = true;

            //Add the listener
            Debug.Listeners.Add(new TextWriterTraceListener(fsLog));
            _init = true;
            WriteLine("Logging initalized","Logging subsystem");
#endif
        }

        [Conditional("DEBUG")]
        public static void Start()
        {
            _enable = true;
            Init();
        }

        /// <summary>
        /// Enables Logger. Use this if logging is already enabled from another location
        /// </summary>
        [Conditional("DEBUG")]
        public static void Enable()
        {
            _enable = true;
            _init = true;
        }
    }
}
