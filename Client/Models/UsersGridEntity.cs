
using HandyBlazorComponents.Abstracts;

using Shared.Models;

namespace Client.Models;

public class UsersGridEntity : HandyGridEntityAbstract<UserDTO>
{

    public UsersGridEntity() : base()
    {
    }

    public UsersGridEntity(UserDTO Object) : base(Object)
    {
    }

    public override object? DisplayPropertyInGrid(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(Object.Id):
                return Object.Id;
            case nameof(Object.Email):
                return Object.Email;
            case nameof(Object.Username):
                return Object.Username;
            case nameof(Object.Roles):
                return string.Join(",", Object.Roles);
            default:
                throw new Exception("Invalid property name");
        }
    }

    public override int GetPrimaryKey()
    {
        return -1;
    }

    public override void SetPrimaryKey(int id)
    {
        ;
    }

    public override void ParsePropertiesFromCSV(Dictionary<string, object> properties)
    {
        base.ParsePropertiesFromCSV(properties);
    }

    public override void SetProperties(Dictionary<string, object> properties)
    {
        base.SetProperties(properties);
    }

}

