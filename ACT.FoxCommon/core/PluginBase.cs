using System.Windows.Forms;
using Advanced_Combat_Tracker;

namespace ACT.FoxCommon.core
{
    public abstract class PluginBase<TMainController> : IActPluginV1
        where TMainController : MainControllerBase
    {
        public TMainController Controller { get; protected set; }

        public abstract void DeInitPlugin();
        public abstract void InitPlugin(TabPage pluginScreenSpace, Label pluginStatusText);
    }
}
