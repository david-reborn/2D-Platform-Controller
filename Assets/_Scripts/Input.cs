using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Myd.Platform.Demo
{
    public struct VisualButton
    {
        private KeyCode key;

        public VisualButton(KeyCode key)
        {
            this.key = key;
        }

        public bool Pressed()
        {
            return UnityEngine.Input.GetKeyDown(key);
        }

        public bool Checked()
        {
            return UnityEngine.Input.GetKey(key);
        }
    }
    public static class Input
    {
        public static VisualButton Jump = new VisualButton(KeyCode.Space);
        public static VisualButton Dash = new VisualButton(KeyCode.K);
    }


}
