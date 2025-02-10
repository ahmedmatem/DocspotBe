namespace DocSpot.Infrastructure.Data
{
    using Microsoft.EntityFrameworkCore;

    using DocSpot.Infrastructure.Data.Abstracts;

    public static class Extensions
    {
        public static void ApplyAuditInfoRules(this ApplicationDbContext context)
        {
            var auditInfoEntries = context.ChangeTracker.Entries()
                .Where(e => e.Entity is IAuditable
                && (e.State == EntityState.Modified || e.State == EntityState.Added))
                .ToList();

            foreach (var entry in auditInfoEntries)
            {
                var entity = (BaseModel)entry.Entity;
                if (entry.State == EntityState.Added)
                {
                    entity.CreatedOn = DateTime.Now;
                }
                else
                {
                    var dateTimeNow = DateTime.Now;
                    entity.LastModifiedOn = dateTimeNow;
                    if (entry.Entity is IDeletable)
                    {
                        if (((IDeletable)entry.Entity).IsDeleted)
                        {
                            entity.DeletedOn = dateTimeNow;
                        }
                    }
                }
            }
        }
    }
}
