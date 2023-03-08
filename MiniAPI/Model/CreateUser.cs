namespace MiniAPI.Model
{
    public class CreateUser
    {
        public string? UserName { get; set; }

        public string? PassWord { get; set; }

        public string? NickName { get; set; }
    }

    public class EditUser: CreateUser
    {
        public Guid guid { get; set; }
    }
}
