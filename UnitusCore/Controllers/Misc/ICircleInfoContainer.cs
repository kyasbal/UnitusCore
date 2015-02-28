namespace UnitusCore.Controllers.Misc
{
    internal interface ICircleInfoContainer : IReadonlyCircleInfoContainer
    {
        new string Name { set; }
        new string Description { set; }
        new int MemberCount { set; }
        new string WebAddress { set; }
        new string BelongedSchool { set; }
        new string Notes { set; }
        new string Contact { set; }
        new bool CanInterColledge { set; }
        new string ActivityDate { set; }
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