namespace SpendWise.Domain
{
  /// <summary>Entity root that collects domain events within its aggregate boundary.</summary>
  public abstract class AggregateRoot<TId, TEvent> : BaseEntity<TId>, IAggregateRoot<TEvent>
      where TId : notnull
      where TEvent : IDomainEvent
  {
      private readonly List<TEvent> _domainEvents = [];

      protected AggregateRoot(TId id) : base(id) { }

      /// <summary>Returns a detached, read-only snapshot, then empties the pending collection.</summary>
      public IReadOnlyList<TEvent> PullDomainEvents()
      {
          IReadOnlyList<TEvent> events = Array.AsReadOnly(_domainEvents.ToArray());
          _domainEvents.Clear();
          return events;
      }

      public void ClearDomainEvents() => _domainEvents.Clear();

      /// <summary>Records an event produced by the aggregate.</summary>
      protected void RegisterEvent(TEvent domainEvent) =>
          _domainEvents.Add(DomainValidation.RequireNonNull(domainEvent, nameof(domainEvent)));
  }
}
