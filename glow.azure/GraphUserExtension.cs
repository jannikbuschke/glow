namespace Gertrud.Users
{
    public static class GraphUserExtension
    {
        public static User ToUser(this Microsoft.Graph.User user)
        {
            return new User { Id = user.Id, DisplayName = user.DisplayName, Email = user.Mail };
        }
    }
}
