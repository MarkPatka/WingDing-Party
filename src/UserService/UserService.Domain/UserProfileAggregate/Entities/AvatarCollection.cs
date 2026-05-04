using System.ComponentModel.DataAnnotations.Schema;
using UserService.Domain.Common.Exceptions;
using UserService.Domain.UserProfileAggregate.ValueObjects;

namespace UserService.Domain.UserProfileAggregate.Entities;

public class AvatarCollection
{
    private readonly List<Avatar> _items;

    private AvatarCollection()
    {
    }

    public AvatarCollection(List<Avatar>? items = null)
    {
        _items = items;
    }

    [NotMapped] public IReadOnlyList<Avatar> Items => _items.AsReadOnly();

    public void Add(Avatar avatar)
    {
        if (_items.Any(a => a.Id == avatar.Id))
        {
            throw new AvatarAlreadyExistsException($"Avatar with id {avatar.Id} already exists.");
        }

        _items.Add(avatar);
        SetDefault(avatar.Id, avatar.IsDefault);
    }

    public Avatar? GetById(AvatarId avatarId)
    {
        return _items.FirstOrDefault(x => x.Id == avatarId);
    }

    public void Update(Avatar avatar)
    {
        if (!_items.Any(a => a.Id == avatar.Id))
        {
            throw new AvatarNotFoundException($"Avatar with id {avatar.Id} does not exist.");
        }

        _items[_items.IndexOf(avatar)] = avatar;
        SetDefault(avatar.Id, avatar.IsDefault);
    }

    public void Remove(AvatarId id)
    {
        var avatar = _items.SingleOrDefault(a => a.Id == id);

        if (avatar == null)
        {
            throw new AvatarNotFoundException("Avatar not found");
        }

        _items.Remove(avatar);
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

        foreach (var a in _items)
        {
            if (a.Id != id && a.IsDefault)
            {
                a.Update(a.IsActive, false);
            }
        }

        avatar.Update(true, isDefault);
    }
}