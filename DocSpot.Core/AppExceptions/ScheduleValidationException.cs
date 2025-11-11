namespace DocSpot.Core.AppExceptions
{
    public class ScheduleValidationException : ApplicationException
    {
        public ScheduleValidationException(string? message) : base(message)
        {
        }
    }
}
