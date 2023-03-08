using Microsoft.AspNetCore.Mvc;
using MiniAPI.Model;

namespace MiniAPI.Services.IService
{
    public interface IUserService
    {
        Task<IResult> GetAllToUsersAsync();

        Task<IResult> GetUserByIdAsync(Guid guid);

        Task<IResult> CreateUserAsync(CreateUser createUser);

        IResult UpdateUser(EditUser editUser);
    }
}
