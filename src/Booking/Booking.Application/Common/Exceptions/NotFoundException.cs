namespace Booking.Application.Common.Exceptions
{
    public class NotFoundException : Exception
    {
        private readonly string _name;
        private readonly object _key;

        public NotFoundException(string name, object key)
            : base($"Entity \"{name}\" ({key}) was not found.")
        {
            _name = name;
            _key = key;
        }
    }
}
