Topic: userprofile-events
Producer: UserService
Consumers: EventService

Schema (JSON):
  - eventType: string (discriminator, required)
    Allowed values:
      - "UserProfileCreatedIntegrationEvent"
      - "UserProfileUpdatedIntegrationEvent"
  - id: guid (message identifier, for idempotency)
  - occurredOnUtc: ISO 8601 datetime
  - userId: guid (aggregate identifier)
  - displayName: string (only for Updated, optional for others)

Kafka:
  - Key: userId (для партишинга по user'у — все события одного юзера в одной партиции)
  - Headers:
      aggregate: "userprofile"

Delivery: at-least-once (Outbox pattern)
Ordering: per-userId only (because of partitioning)