using Services.Models;
using SimpleMem;
using System.Linq;

namespace Services
{
    internal static class Extensions
    {
        public static MultiLevelPtr ToMultiLvlPtr<T>(this DeviceOffset<T> device)
        {
            return new((nint)device.BaseAddress, device.Pointers.Select(x => (nint)x).ToArray());
        }
    }
}
