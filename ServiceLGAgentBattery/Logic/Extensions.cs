using ServiceLGAgentBattery.Models;
using SimpleMem;
using System.Linq;

namespace ServiceLGAgentBattery.Logic
{
    internal static class Extensions
    {
        public static MultiLevelPtr ToMultiLvlPtr<T>(this DeviceOffset<T> device)
        {
            return new((nint)device.BaseAddress, device.Pointers.Select(x => (nint)x).ToArray());
        }
    }
}
