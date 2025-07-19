using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace ServiceLGAgentBattery.Models
{
    public abstract class DeviceOffset<T>
    {
        [JsonPropertyName("name")]
        public abstract string Name { get; }

        [JsonPropertyName("processname")]
        public abstract string Proccesname { get; }

        [JsonPropertyName("description")]
        public virtual string Description { get; }

        [JsonPropertyName("baseAddress")]
        public nuint BaseAddress { get; internal set; }

        [JsonPropertyName("pointers")]
        public IEnumerable<nuint> Pointers { get; internal set; }

        [JsonPropertyName("valueType")]
        public Type ValueType { get; internal set; } = typeof(T);

        [JsonIgnore()]
        public string PointerPath => $"{this.Proccesname}+{this.BaseAddress}->{string.Join("->", this.Pointers.Select(x => $"0x{x:X}"))}";

        #region Constructor
        protected DeviceOffset(nuint baseAddress)
        {
            this.BaseAddress = baseAddress;
        }

        protected DeviceOffset(nuint baseAddress, IEnumerable<nuint> pointers)
        {
            this.BaseAddress = baseAddress;
            this.Pointers = pointers;
        }
        #endregion

        public void SetPointers(IEnumerable<nuint> pointers)
        {
            this.Pointers = pointers;
        }

        public void SetBaseAddress(nuint baseAddress)
        {
            this.BaseAddress = baseAddress;
        }
    }
}
