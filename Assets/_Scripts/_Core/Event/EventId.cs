using System;
using System.Collections.Generic;
using System.Text;

namespace QGame.Core.Event
{
    public enum EventId : int
    {
        LoginSuccess,
        Captured,
        SceneQuit,
        UserUiClick,
        GamePaused,
        UserClick,
        WaveStart,
        ActionStart,
        ActionCancel,
    }
}
