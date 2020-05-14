namespace JannikB.Glue.AspNetCore.Tests
{
    public interface IUser
    {
        string Id { get; set; }
        string DisplayName { get; set; }
        public string Email { get; set; }
    }
}
