using DirectoryOfPreferences.Domain.Entity.Base;

namespace DirectoryOfPreferences.Domain.Entity
{
    public class Preference : Entity<Guid>
    {
        public string Name { get; set; }
        public Preference(Guid id, string name) : base(id)
        {
            Name = name;
        }
        public Preference(string name) : this(Guid.NewGuid(), name)
        {

        }
        protected Preference() : base(Guid.NewGuid())
        {

        }
    }
}
