namespace Medius.Models.Enums
{
    public enum FilterType
    {
        Category = 1,
        Technology = 2
    }

    public enum CaseType
    {
        Copyright = 1,
        Trademark = 2,
        Patent = 3,
        Design = 4
    }

    public enum Status
    {
        Sent,
        Seen,
        Processed,
        Pending,
        Reject,
        Publish
    }

    public enum ModeofRegistration
    {
        Fast = 1,
        Normal = 2
    }
    public enum Role
    {
        Admin,
        User,
        SubAdmin
    }
}
