namespace DocSpot.Infrastructure.Data.Abstracts
{
    using System.ComponentModel.DataAnnotations;

    public abstract class BaseModel : IAuditable, IDeletable
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public bool IsDeleted { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? DeletedOn { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime CreatedOn { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? LastModifiedOn { get; set; }
    }
}
