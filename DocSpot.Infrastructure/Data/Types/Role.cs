#nullable disable
namespace DocSpot.Infrastructure.Data.Types
{
    public class Role
    {
        public string Value { get; private set; }

        private Role(string value) { Value = value; }

        public static Role Admin => new Role("Admin");
        public static Role Doctor => new Role("Doctor");
        public static Role Patient => new Role("Patient");

        public override string ToString() => Value;
    }
}
