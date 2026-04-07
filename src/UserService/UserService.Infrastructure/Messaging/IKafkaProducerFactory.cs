using Confluent.Kafka;

namespace UserService.Infrastructure.Messaging;

public interface IKafkaProducerFactory
{
    IProducer<string, string> GetProducer(string aggregate);
}