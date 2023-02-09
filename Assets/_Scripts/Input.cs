using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Myd.Platform.Demo
{
    public enum Facings
    {
        Right = 1,
        Left = -1
    }

    public struct VirtualIntegerAxis
    {

    }
    public struct VirtualJoystick
    {
        public Vector2 Value { get => new Vector2(UnityEngine.Input.GetAxisRaw("Horizontal"), UnityEngine.Input.GetAxisRaw("Vertical"));}
    }
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
        public static VisualButton Grab = new VisualButton(KeyCode.J);
        public static VirtualJoystick Aim = new VirtualJoystick();
        public static Vector2 LastAim;

        //根据当前朝向,决定移动方向.
        public static Vector2 GetAimVector(Facings defaultFacing = Facings.Right)
        {
            Vector2 value = Input.Aim.Value;
            //TODO 考虑辅助模式

            //TODO 考虑摇杆
            if (value == Vector2.zero)
            {
                Input.LastAim = Vector2.right * ((int)defaultFacing);
            }
            else
            {
                Input.LastAim = value;
            }
            return Input.LastAim;
        }
    }




}
