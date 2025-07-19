namespace CTableConverter.Models
{
    public record CheatEntry
    {
        public string Address { get; set; }
        public string[] Offsets { get; set; }
    }
}
