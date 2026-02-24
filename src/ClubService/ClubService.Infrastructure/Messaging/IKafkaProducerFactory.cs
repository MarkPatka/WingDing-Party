using Confluent.Kafka;

namespace ClubService.Infrastructure.Messaging;

public interface IKafkaProducerFactory
{
    IProducer<string, string> GetProducer(string aggregate);
}