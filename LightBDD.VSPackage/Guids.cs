// Guids.cs
// MUST match guids.h
using System;

namespace Company.LightBDD_VSPackage
{
    static class GuidList
    {
        public const string guidLightBDD_VSPackagePkgString = "d6382c7a-fe20-47e5-b4e1-4d798cef97f1";
        public const string guidLightBDD_VSPackageCmdSetString = "c8d0ece5-1d6b-418b-be53-7f5229baffd7";

        public static readonly Guid guidLightBDD_VSPackageCmdSet = new Guid(guidLightBDD_VSPackageCmdSetString);
    };
}