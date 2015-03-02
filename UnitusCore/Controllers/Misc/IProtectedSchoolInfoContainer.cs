using UnitusCore.Storage;

namespace UnitusCore.Controllers.Misc
{
    public interface IProtectedSchoolInfoContainer
    {
        DisclosureProtectedResponse BelongedSchool { get; set; }

        DisclosureProtectedResponse Faculty { get; set; }

        DisclosureProtectedResponse Major { get; set; }
    }
}