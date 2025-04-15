
using Server.Entities;
using Shared.Models;

namespace Server.Utils;

public static class DTOMapper
{
    public static Application ToEntity(this ApplicationDTO dto)
    {
        return new()
        {
            Applicationid = dto.Applicationid,
            Userid = dto.Userid,
            Roleid = dto.Roleid,
            Applicationname = dto.Applicationname,
        };
    }

    public static ApplicationDTO ToDTO(this Application obj)
    {
        return new()
        {
            Applicationid = obj.Applicationid,
            Userid = obj.Userid,
            Roleid = obj.Roleid,
            Applicationname = obj.Applicationname,
        };
    }
}
