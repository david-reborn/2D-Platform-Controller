using Myd.Platform.Core;
using System;
using System.Collections.Generic;

namespace Myd.Platform
{
    public interface IGameContext
    {
        IEffectControl EffectControl { get; }

        ISoundControl SoundControl { get; }


    }
}
