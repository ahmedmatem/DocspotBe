namespace DocSpot.Infrastructure.Data.Abstracts
{
    public interface IAuditable
    {
        DateTime CreatedOn { get; set; }

        DateTime? LastModifiedOn { get; set; }
    }
}
