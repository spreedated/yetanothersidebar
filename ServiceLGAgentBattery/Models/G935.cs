using System.Collections.Generic;

namespace ServiceLGAgentBattery.Models
{
    public sealed class G935<T> : DeviceOffset<T>
    {
        public override string Name { get; } = "G935 Headset";

        public override string Proccesname { get; } = "lghub_agent";

        #region Constructor
        public G935(nuint baseAddress) : base(baseAddress) { }

        public G935(nuint baseAddress, IEnumerable<nuint> pointers) : base(baseAddress, pointers) { }
        #endregion
    }
}
