using Serilog;
using ServiceLGAgentBattery.Logic;
using ServiceLGAgentBattery.Models;
using SimpleMem;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ServiceLGAgentBattery.Steps
{
    internal class Setup : Step
    {
        public Setup() : base()
        {
            base.Id = 0;
            base.Name = "Setup";
            base.ContinueOnError = false;
            base.IsActive = true;
        }

        public override async Task Processor()
        {
            try
            {
                MemoryMappedFile mmf = MemoryMappedFile.OpenExisting("Global\\SkinCalc\\ServiceLGAgentBattery", MemoryMappedFileRights.Read);

                using (MemoryMappedViewStream accessor = mmf.CreateViewStream())
                {
                    using (StreamReader r = new(accessor))
                    {
                        LogitechBatteryResponse response = JsonSerializer.Deserialize<LogitechBatteryResponse>(r.ReadToEnd());
                    }
                }
            }
            catch (Exception ex)
            {
            }

            Globals.MemoryMapped ??= MemoryMappedFile.CreateNew("Global\\SkinCalc\\ServiceLGAgentBattery", 1024, MemoryMappedFileAccess.ReadWrite);

            LogitechBatteryResponse data = await GetData();

            string dataJson = JsonSerializer.Serialize(data);

            using (MemoryMappedViewStream accessor = Globals.MemoryMapped.CreateViewStream())
            {
                using (StreamWriter sw = new(accessor))
                {
                    sw.Write(dataJson);
                }
            }

            Log.Information("Shared memory created");

            await Task.CompletedTask;
        }

        private static async Task<LogitechBatteryResponse> GetData()
        {
            G915<float> g915_address0 = new(0x0248CDF8, [0x10, 0x20, 0x60, 0x38, 0x10, 0x2E0, 0x2E8, 0x8, 0x8, 0x50]);
            G915<float> g915_address1 = new(0x06C10B80, [0x8, 0x8, 0x70, 0x0, 0x24, 0x58, 0x358, 0x3F8, 0x20, 0x50]);
            G915<float> g915_address2 = new(0x0248CDF8, [0x10, 0x20, 0xB8, 0x88, 0x38, 0x80, 0x2E0, 0x3F8, 0x20, 0x50]);
            G915<float> g915_address3 = new(0x0248CDF0, [0x10, 0x20, 0x60, 0x38, 0x10, 0x2E8, 0x10, 0x3F8, 0x28, 0x50]);

            G935<float> g935_address0 = new(0x0248CDE8, [0x30, 0x20, 0x68, 0x30, 0x48, 0x2E0, 0x3F8, 0x28, 0x0, 0x50]);
            G935<float> g935_address1 = new(0x02FA9690, [0x98, 0x8, 0x90, 0x78, 0x40, 0x0, 0x450, 0x3E8, 0x8, 0x50]);
            G935<float> g935_address2 = new(0x02F9D730, [0x8, 0x8, 0x48, 0x78, 0x30, 0x38, 0x450, 0x3E8, 0x8, 0x50]);
            G935<float> g935_address3 = new(0x02F9D710, [0x8, 0x8, 0x48, 0xB8, 0x0, 0x0, 0x450, 0x3F8, 0x48, 0x50]);

            float g915_result = default;
            float g935_result = default;

            await Task.Factory.StartNew(() =>
            {
                g915_result = GetFinalResult(g915_address0, g915_address1, g915_address2, g915_address3);
                g935_result = GetFinalResult(g935_address0, g935_address1, g935_address2, g935_address3);
            });

            return new()
            {
                G915_Battery = g915_result,
                G935_Battery = g935_result,
                Date = DateTime.Now
            };
        }

        /// <summary>
        /// Calculate final result from multiple offsets
        /// </summary>
        /// <param name="deviceOffsets"></param>
        /// <returns>Value between 0 - 100, returns -1 on error</returns>
        internal static float GetFinalResult(params DeviceOffset<float>[] deviceOffsets)
        {
            Memory mem = new(deviceOffsets[^1].Proccesname);

            List<float> results = [];
            foreach (DeviceOffset<float> g in deviceOffsets)
            {
                try
                {
                    results.Add(mem.ReadValueFromMlPtr<float>(g.ToMultiLvlPtr()));
                }
                catch
                {
                    // ignored, skip value
                }
            }

            if (results.Count <= 0)
            {
                return -1;
            }

            float res = results.GroupBy(x => x).OrderByDescending(x => x.Count()).Select(x => x.Key).First();

            return res >= 0 && res <= 100 ? res : -1;
        }
    }
}