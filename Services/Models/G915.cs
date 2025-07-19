using System.Collections.Generic;

namespace Services.Models
{
    public sealed class G915<T> : DeviceOffset<T>
    {
        public override string Name { get; } = "G915 Keyboard";

        public override string Proccesname { get; } = "lghub_agent";

        #region Constructor
        public G915(nuint baseAddress) : base(baseAddress) { }

        public G915(nuint baseAddress, IEnumerable<nuint> pointers) : base(baseAddress, pointers) { }
        #endregion
    }
}
