using ClubService.Application.Common.Configuration;
using Microsoft.Extensions.Options;

namespace ClubService.Infrastructure.Messaging;

public class KafkaOptionsValidator : IValidateOptions<Dictionary<string, KafkaOptions>>
{
    public ValidateOptionsResult Validate(string name, Dictionary<string, KafkaOptions> options)
    {
        foreach (var (aggregate, cfg) in options)
        {
            if (string.IsNullOrWhiteSpace(cfg.BootstrapServers))
                return ValidateOptionsResult.Fail($"BootstrapServers is missing for aggregate '{aggregate}'");
            if (string.IsNullOrWhiteSpace(cfg.ProduceEventsTopic))
                return ValidateOptionsResult.Fail($"ProduceEventsTopic is missing for aggregate '{aggregate}'");
        }

        return ValidateOptionsResult.Success;
    }
}