using System;
using System.Collections.Generic;

namespace ACT.FoxCommon
{
    /// <summary>
    /// Access game client info.
    /// </summary>
    public static class GameClientInfo
    {
        /// <summary>
        /// Read the game language settings from FFXIV_ACT_Plugin
        /// </summary>
        public static GameLanguage GetLanguage()
        {
            dynamic ffxivPlugin = Utils.FindFFXIVPlugin()?.pluginObj;

            if (ffxivPlugin == null)
            {
                return GameLanguage.Unknown;
            }

            try
            {
                return (GameLanguage) ffxivPlugin.DataRepository.GetSelectedLanguageID();
            }
            catch (Exception)
            {
                return GameLanguage.Unknown;
            }
        }

        public enum GameLanguage
        {
            Unknown = 0,
            En = 1,
            Fr = 2,
            De = 3,
            Ja = 4,
            Cn = 5,
            Ko = 6,
        }

        public static readonly HashSet<GameLanguage> GLOBAL_VERSIONS = new HashSet<GameLanguage>(new GameLanguage[]
        {
            GameLanguage.En, GameLanguage.Fr, GameLanguage.De, GameLanguage.Ja
        });

        public static bool IsGlobalGame(this GameLanguage lan)
        {
            return GLOBAL_VERSIONS.Contains(lan);
        }
    }
}
