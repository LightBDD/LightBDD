namespace LightBDD.Core.Notification
{
    public interface ILightBddProgressNotifier
    {
        void NotifyLightBddStart();
        void NotifyLightBddFinished();
    }
}
