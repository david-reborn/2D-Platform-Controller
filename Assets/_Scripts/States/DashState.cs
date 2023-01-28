using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Myd.Platform.Demo
{
    public class DashState : BaseActionState
    {
        public DashState(IPlayerContext context) : base(EActionState.Dash, context)
        {
        }

        public override void OnBegin()
        {
            this.ctx.MaxFall = Constants.MaxFall;
        }

        public override void OnEnd()
        {
            this.ctx.WallSpeedRetentionTimer = 0;
        }

        public override EActionState Update(float deltaTime)
        {
            return state;
        }
    }
}
