namespace Booking.Application.Common.Security
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = true)]
    public class AuthorizeAttribute : Attribute
    {
        private string[]? _roles;
        public string[]? Roles { 
            get => _roles; 
            set
            {
                if (value != null && value.Any(string.IsNullOrWhiteSpace))
                    throw new ArgumentException("Role names cannot be null or whitespace.", nameof(Roles));
                _roles = value;
            } 
        }

        //TO DO: Now it's useless but in future
        public string? Policy { get; set; }
    }
}
