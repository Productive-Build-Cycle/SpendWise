namespace SpendWise.Domain
{
    /// <summary>A domain occurrence recorded at a specific instant.</summary>
    public interface IDomainEvent
    {
        TimeOnly OccurredOn { get; }
    }
}
