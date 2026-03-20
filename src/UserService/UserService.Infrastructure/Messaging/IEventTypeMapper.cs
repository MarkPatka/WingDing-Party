namespace UserService.Infrastructure.Messaging;

public interface IEventTypeMapper
{
    Type GetType(string typeName);
    string GetName(Type type);
}