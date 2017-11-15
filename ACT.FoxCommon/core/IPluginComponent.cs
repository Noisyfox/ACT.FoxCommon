namespace ACT.FoxCommon.core
{
    public interface IPluginComponentBase<TMainController, in TPlugin>
        where TPlugin : PluginBase<TMainController>
        where TMainController : MainControllerBase
    {
        void AttachToAct(TPlugin plugin);
        void PostAttachToAct(TPlugin plugin);
    }
}
