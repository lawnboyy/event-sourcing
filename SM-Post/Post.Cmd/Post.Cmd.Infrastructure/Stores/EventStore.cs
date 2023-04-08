using CQRS.Core.Domain;
using CQRS.Core.Events;
using CQRS.Core.Exceptions;
using CQRS.Core.Infrastructure;
using CQRS.Core.Producers;
using Post.Cmd.Domain.Aggregates;

namespace Post.Cmd.Infrastructure.Stores;

public class EventStore : IEventStore
{
  private readonly IEventStoreRepository _eventStoreRepository;
  private readonly IEventProducer _eventProducer;

  public EventStore(IEventStoreRepository eventStoreRepository, IEventProducer eventProducer)
  {
    _eventStoreRepository = eventStoreRepository;
    _eventProducer = eventProducer;
  }

  public async Task<List<BaseEvent>> GetEventsAsync(Guid aggregateId)
  {
    var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);

    if (eventStream == null || !eventStream.Any())
    {
      throw new AggregateNotFoundException("Incorrect post ID provided!");
    }

    return eventStream.OrderBy(e => e.Version).Select(e => e.EventData).ToList();
  }

  public async Task SaveEventsAsync(Guid aggregateId, IEnumerable<BaseEvent> events, int expectedVersion)
  {
    var eventStream = await _eventStoreRepository.FindByAggregateId(aggregateId);

    if (expectedVersion != -1 && eventStream[^1].Version != expectedVersion)
    {
      throw new ConcurrencyException();
    }

    var version = expectedVersion;

    foreach (var @event in events)
    {
      version++;
      @event.Version = version;
      var eventType = @event.GetType().Name;
      var eventModel = new EventModel
      {
        TimeStamp = DateTime.Now,
        AggregateIdentifier = aggregateId,
        AggregateType = nameof(PostAggregate),
        Version = version,
        EventType = eventType,
        EventData = @event
      };

      await _eventStoreRepository.SaveAsync(eventModel);

      // IMPORTANT: This is not a transaction. If the event publishing fails, the event will still be persisted.
      // In a production environment we'd want to wrap the event persistence and the event publishing in a transaction (a mongo transaction in this case).
      // If the event publishing fails, we'd want to rollback the event persistence.
      var topic = Environment.GetEnvironmentVariable("KAFKA_TOPIC") ?? "SocialMediaEvents";
      await _eventProducer.ProduceAsync(topic, @event);
    }
  }
}