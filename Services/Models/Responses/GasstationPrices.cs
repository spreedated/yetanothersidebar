using System;

namespace Services.Models.Responses
{
    public class GasstationPrices
    {
        public GasPrice PmExpressDremmen { get; set; }
        public GasPrice BftOberbruch { get; set; }
        public GasPrice ShellHeinsberg { get; set; }

        public class GasPrice
        {
            public double E5 { get; set; }
            public double E10 { get; set; }
            public double Diesel { get; set; }
            public DateTime Date { get; set; }
        }
    }
}
