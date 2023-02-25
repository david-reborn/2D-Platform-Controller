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
        private float bufferTime;
        private bool consumed;
        private float bufferCounter;
        public VisualButton(KeyCode key) : this(key, 0) {
        }

        public VisualButton(KeyCode key, float bufferTime)
        {
            this.key = key;
            this.bufferTime = bufferTime;
            this.consumed = false;
            this.bufferCounter = 0f;
        }

        public bool Pressed()
        {
            return UnityEngine.Input.GetKeyDown(key)||(!this.consumed && (this.bufferCounter > 0f));
        }

        public bool Checked()
        {
            return UnityEngine.Input.GetKey(key);
        }

        public void Update(float deltaTime)
        {
            this.consumed = false;
            this.bufferCounter -= deltaTime;
            bool flag = false;
            if (UnityEngine.Input.GetKeyDown(key))
            {
                this.bufferCounter = this.bufferTime;
                flag = true;
            }
            if (!flag)
            {
                this.bufferCounter = 0f;
                return;
            }
        }
    }
    public static class Input
    {
        public static VisualButton Jump = new VisualButton(KeyCode.Space, 0.2f);
        public static VisualButton Dash = new VisualButton(KeyCode.K, 0.2f);
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
            return Input.LastAim.normalized;
        }

        public static void Update(float deltaTime)
        {
            Jump.Update(deltaTime);
            Dash.Update(deltaTime);
        }
    }




}
