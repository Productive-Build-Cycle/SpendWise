namespace SpendWise.Domain
{
  /// <summary>Exposes domain events recorded within an aggregate boundary.</summary>
  public interface IAggregateRoot<TEvent> where TEvent : IDomainEvent
  {
      /// <summary>Returns an immutable snapshot and clears pending domain events.</summary>
      IReadOnlyList<TEvent> PullDomainEvents();

      /// <summary>Discards pending domain events.</summary>
      void ClearDomainEvents();
  }
}
