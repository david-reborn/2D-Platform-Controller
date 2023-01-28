using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Myd.Platform.Demo
{
    public enum EActionState
    {
        Normal,
        Dash,
        Size,
    }

    public abstract class BaseActionState
    {
        protected EActionState state;
        protected IPlayerContext ctx;

        protected BaseActionState(EActionState state, IPlayerContext context)
        {
            this.ctx = context;
        }

        public EActionState State { get => state; }

        //每一帧都执行的逻辑
        public abstract EActionState Update(float deltaTime);

        public abstract void OnBegin();

        public abstract void OnEnd();
    }

    /// <summary>
    /// 有限状态机
    /// </summary>
    public class FiniteStateMachine<S> where S: BaseActionState
    {
        private S[] states;

        private int currState = -1;
        private int prevState = -1;

        public FiniteStateMachine(int size)
        {
            this.states = new S[size];
        }

        public void AddState(S state)
        {
            this.states[(int)state.State] = state;
        }

        public void Update(float deltaTime)
        {  
            this.currState = (int)this.states[this.currState].Update(deltaTime);
        }

        //改变状态
        public void SetState(int nextState)
        {
            if (this.currState == nextState)
                return;

            this.prevState = this.currState;
            this.currState = nextState;
            if (this.prevState != -1)
            {
                this.states[this.prevState].OnEnd();
            }
            this.states[this.currState].OnBegin();

            //TODO 处理协程
        }
    }
}
