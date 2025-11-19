namespace DocSpot.Core.Models
{
    public class SlotDto
    {
        public string Time { get; set; } = default!;   // "HH:mm"

        public bool Available { get; set; }
    }
}
