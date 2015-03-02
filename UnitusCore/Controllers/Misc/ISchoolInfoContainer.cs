using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UnitusCore.Controllers.Misc
{
    public interface ISchoolInfoContainer
    {
        string BelongedSchool { get; set; }

    }

    public interface IMajorInfoContainer:ISchoolInfoContainer
    {
        string Faculty { get; set; }

        string Major { get; set; }
    }

}