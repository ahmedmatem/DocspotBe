namespace DocSpot.Core.Contracts
{
    using DocSpot.Infrastructure.Data.Models;

    public interface IScheduleService
    {
        public Task AddAsync(Schedule schedule);
    }
}
