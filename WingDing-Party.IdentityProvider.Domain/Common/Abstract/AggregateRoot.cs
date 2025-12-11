namespace WingDing_Party.IdentityProvider.Domain.Common.Abstract;

public abstract class AggregateRoot<Tid> : Entity<Tid>
    where Tid : notnull
{
    private readonly List<IDomainEvent> _domainEvents = [];


    protected AggregateRoot(Tid id) : base(id) { }

#pragma warning disable CS8618
    protected AggregateRoot() { }
#pragma warning disable CS8618


    protected void RaiseDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }
}