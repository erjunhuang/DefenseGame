using System;
using System.Collections.Generic;
using System.Text;

namespace QGame.Core.Event
{
    public enum EventId : int
    {
        LoginSuccess,
         
        SceneQuit,
        UserUiClick,
        GamePaused,
        UserClick,
        ActionStart,
        ActionCancel,

        Captured,
        GameResult,
        BattleHurt,
        BattleSkillHurt,
        SpawningEnemies,
        AddCurrency,
        CurrencyChanged,
        WaveChanged,
        BattleSkillEnd,
    }
}
