using System;

namespace LightBDD.Framework.Reporting.UnitTests
{
    class AppDomainHelper
    {
        public static string BaseDirectory
        {
            get
            {
#if NET451
                return AppDomain.CurrentDomain.BaseDirectory;
#else
                return AppContext.BaseDirectory;
#endif
            }
        }
    }
}