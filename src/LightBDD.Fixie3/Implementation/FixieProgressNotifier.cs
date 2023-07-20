using System;
using LightBDD.Core.Notification;
using LightBDD.Framework.Notification;

namespace LightBDD.Fixie3.Implementation
{
    internal class FixieProgressNotifier
    {
        public static IProgressNotifier CreateProgressNotifier()
        {
            return ParallelProgressNotifierProvider.Default.CreateProgressNotifier(Console.WriteLine);
        }
    }
}
