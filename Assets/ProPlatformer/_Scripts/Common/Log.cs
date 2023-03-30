using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Myd.Common
{
    public static class Logging
    {
        public static void Log(object message)
        {
            Debug.Log(message);
        }
    }
}
