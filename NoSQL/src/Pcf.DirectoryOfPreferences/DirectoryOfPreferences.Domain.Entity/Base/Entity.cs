namespace DirectoryOfPreferences.Domain.Entity.Base
{
    public abstract class Entity<TId>(TId id) : IEquatable<Entity<TId>> where TId : struct
    {
        /// <summary>
        /// Gets the ID of the entity.
        /// </summary>
        public TId Id { get; protected set; } = id;

        public bool Equals(Entity<TId>? other)
            => other is not null && EqualityComparer<TId>.Default.Equals(Id, other.Id);
    }
}
