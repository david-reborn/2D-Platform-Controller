using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Myd.Common
{
    public static class Calc
    {
        public static bool OnInterval(float val, float prevVal, float interval)
        {
            return (int)(prevVal / interval) != (int)(val / interval);
        }
    }
}
