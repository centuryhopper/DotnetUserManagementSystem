
using HandyBlazorComponents.Abstracts;

using Shared.Models;

namespace Client.Models;

public class ApplicationsGridEntity : HandyGridEntityAbstract<ApplicationDTO>
{
    public ApplicationsGridEntity() : base()
    {
    }

    public ApplicationsGridEntity(ApplicationDTO Object) : base(Object)
    {
    }

    public override object? DisplayPropertyInGrid(string propertyName)
    {
        switch (propertyName)
        {
            case nameof(Object.Applicationid):
                return Object.Applicationid;
            case nameof(Object.Applicationname):
                return Object.Applicationname;
            case nameof(Object.Userid):
                return Object.Userid;
            case nameof(Object.Roleid):
                return Object.Roleid;
            default:
                throw new Exception("Invalid property name");
        }
    }

    public override int GetPrimaryKey()
    {
        return Object.Applicationid;
    }

    public override void SetPrimaryKey(int id)
    {
        Object.Applicationid = id;
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

