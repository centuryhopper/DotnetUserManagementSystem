
using HandyBlazorComponents.Abstracts;

using Shared.Models;

namespace Client.Models;

public class RolesGridEntity : HandyGridEntityAbstract<RoleDTO>
{

    public RolesGridEntity() : base()
    {
    }

    public RolesGridEntity(RoleDTO Object) : base(Object)
    {
    }

    public override object? DisplayPropertyInGrid(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(Object.Id):
                return Object.Id;
            case nameof(Object.RoleName):
                return Object.RoleName;
            case nameof(Object.IsActive):
                return Object.IsActive;
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

