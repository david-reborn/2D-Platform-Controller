using System;
using System.Collections;
using UnityEngine;

namespace Myd.Platform
{
    public class EventManager
    {
        public static EventManager instance;

        public static EventManager Get()
        {
            if (instance == null)
            {
                instance = new EventManager();
            }
            return instance;
        }
    }
}