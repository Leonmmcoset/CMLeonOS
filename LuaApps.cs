using IL2CPU.API.Attribs;

namespace CMLeonOS
{
    public static class LuaApps
    {
        [ManifestResourceStream(ResourceName = "CMLeonOS.LuaApps.helloworld.lua")]
        public static readonly byte[] helloworld;
        
        [ManifestResourceStream(ResourceName = "CMLeonOS.LuaApps.testspeed.lua")]
        public static readonly byte[] testspeed;
        
        [ManifestResourceStream(ResourceName = "CMLeonOS.LuaApps.calculator.lua")]
        public static readonly byte[] calculator;
    }
}