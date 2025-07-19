using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Services.Models
{
    public abstract class DeviceOffset<T>
    {
        [JsonProperty("name")]
        public abstract string Name { get; }

        [JsonProperty("processname")]
        public abstract string Proccesname { get; }

        [JsonProperty("description")]
        public virtual string Description { get; }

        [JsonProperty("baseAddress")]
        public nuint BaseAddress { get; internal set; }

        [JsonProperty("pointers")]
        public IEnumerable<nuint> Pointers { get; internal set; }

        [JsonProperty("valueType")]
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
