namespace AljasAuthApi.Models
{
    public class UserAccessResponse
    {
        public string Id { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string RoleName { get; set; } = string.Empty;
         public string RoleId{get; set;}= string.Empty;
        public AllowedModules AllowedModules { get; set; } = new AllowedModules();
    }
}
