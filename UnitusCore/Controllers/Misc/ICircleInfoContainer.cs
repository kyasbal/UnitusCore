namespace UnitusCore.Controllers.Misc
{
    internal interface ICircleInfoContainer : IReadonlyCircleInfoContainer
    {
        string Name { set; }
        string Description { set; }
        int MemberCount { set; }
        string WebAddress { set; }
        string BelongedSchool { set; }
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
        string BelongedSchool { get; }
        string Notes { get; }
        string Contact { get; }
        bool CanInterColledge { get; }
        string ActivityDate { get; }
    }
}