namespace SpendWise.Domain
{
  /// <summary>An entity whose equality is determined by its identity.</summary>
  public class BaseEntity<TId> where TId : notnull
  {
      protected BaseEntity(TId id)
      {
          Id = DomainValidation.RequireNonNull(id, "Entity id");
      }

      public TId Id { get; }

      public override bool Equals(object? obj) =>
          ReferenceEquals(this, obj) ||
          obj is BaseEntity<TId> other && EqualityComparer<TId>.Default.Equals(Id, other.Id);

      public override int GetHashCode() => EqualityComparer<TId>.Default.GetHashCode(Id);
  }
}
