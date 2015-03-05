// ReSharper disable UnusedMemberInSuper.Global
#pragma warning disable 108,114
namespace UnitusCore.Controllers.Misc
{
    internal interface ICircleInfoContainer : IReadonlyCircleInfoContainer,ISchoolInfoContainer
    {
        string Name { set; }
        string Description { set; }
        int MemberCount { set; }
        string WebAddress { set; }
        string Notes { set; }
        string Contact { set; }
        bool CanInterColledge { set; }
        string ActivityDate { set; }
    }

    internal interface IReadonlyCircleInfoContainer
    {
        string Name { get; }
        string Description { get; }
        int MemberCount { get; }
        string WebAddress { get; }
        string Notes { get; }
        string Contact { get; }
        bool CanInterColledge { get; }
        string ActivityDate { get; }
    }
}