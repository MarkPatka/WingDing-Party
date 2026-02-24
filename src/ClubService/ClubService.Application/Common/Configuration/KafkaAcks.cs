namespace ClubService.Application.Common.Configuration;

public enum KafkaAcks
{
    All = -1,
    None = 0,
    Leader = 1,
}