using UserService.Domain.Common.Exceptions;
using UserService.Domain.UserProfileAggregate.ValueObjects;

namespace UserService.Domain.UserProfileAggregate.Entities;

public class AvatarCollection
{
    private readonly List<Avatar> _items;

    public AvatarCollection(IEnumerable<Avatar>? items = null) =>
        _items = items?.ToList() ?? new List<Avatar>();

    public IReadOnlyList<Avatar> Items => _items.AsReadOnly();

    public bool Add(Avatar avatar)
    {
        if (_items.Any(a => a.Id == avatar.Id)) return false;
        _items.Add(avatar);
        return true;
    }

    public bool Remove(AvatarId id)
    {
        var avatar = _items.SingleOrDefault(a => a.Id == id);

        if (avatar == null)
        {
            throw new AvatarNotFoundException("Avatar not found");
        }

        _items.Remove(avatar);
        return true;
    }

    public void SetActive(AvatarId id, bool isActive)
    {
        var avatar = _items.SingleOrDefault(a => a.Id == id);
        if (avatar == null)
        {
            throw new AvatarNotFoundException("Avatar not found");
        }

        avatar.Update(avatar.IsDefault, isActive);
    }

    public void SetDefault(AvatarId id, bool isDefault)
    {
        var avatar = _items.SingleOrDefault(a => a.Id == id);
        if (avatar == null)
        {
            throw new AvatarNotFoundException("Avatar not found");
        }

        if (!isDefault)
        {
            if (!avatar.IsDefault) return;
            avatar.Update(false, avatar.IsActive);
            return;
        }

        if (avatar.IsDefault)
        {
            return;   
        }

        foreach (var a in _items)
        {
            if (a.Id != id && a.IsDefault)
            {
                a.Update(false, a.IsActive);
            }
        }

        avatar.Update(isDefault, true);
    }
}