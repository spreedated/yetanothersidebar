#pragma warning disable S1075

using Services.Models;
using Services.Models.Responses;
using System;
using System.Collections.Generic;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Timers;

namespace Services
{
    public sealed class LgDeviceService : Service<LgsDeviceResponse>
    {
        public LgDeviceService()
        {
            base.CreateTimer(new TimeSpan(0, 0, 10));
        }

        internal override void Processor(object sender, ElapsedEventArgs e)
        {
            this.ProcessIsRunning = true;
            base.RaiseProcessStarted();

            try
            {
                this.GetData().Wait();
            }
            catch (Exception ex)
            {
                base.RaiseError(ex);
            }

            this.ProcessIsRunning = false;
            base.RaiseValuesUpdated();
        }

        private async Task GetData()
        {
            string json = null;
            try
            {
                using (MemoryMappedFile mmf = MemoryMappedFile.OpenExisting($"Global\\LGS_Devices"))
                {
                    using (MemoryMappedViewAccessor accessor = mmf.CreateViewAccessor())
                    {
                        ushort length = accessor.ReadUInt16(0);
                        byte[] buffer = new byte[length];
                        accessor.ReadArray(2, buffer, 0, length);
                        json = System.Text.Encoding.UTF8.GetString(buffer);
                    }
                }
            }
            catch
            {
                //noop
            }

            base.Response = new()
            {
                Devices = [.. JsonSerializer.Deserialize<IEnumerable<LogitechDevice>>(json)],
                Date = DateTime.Now,
            };
        }
    }
}
