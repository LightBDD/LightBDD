using LightBDD.Core.Notification;
using LightBDD.Framework.Notification;

namespace LightBDD.TUnit.Implementation
{
    internal class TUnitProgressNotifier
    {
        private static void WriteOutput(string text)
        {
            TestContext.Current?.OutputWriter.WriteLine(text);
        }

        private static void WriteImmediateProgress(string text)
        {
            TestContext.Current?.OutputWriter.WriteLine(text);
        }

        public static IProgressNotifier[] CreateProgressNotifiers()
        {
            return
            [
                ParallelProgressNotifierProvider.Default.CreateProgressNotifier(WriteImmediateProgress),
                new DefaultProgressNotifier(WriteOutput)
            ];
        }
    }
}
