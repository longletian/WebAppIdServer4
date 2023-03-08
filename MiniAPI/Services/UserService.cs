using Microsoft.EntityFrameworkCore;
using MiniAPI.Model;
using MiniAPI.Services.IService;

namespace MiniAPI.Services
{
    public class UserService : IUserService
    {
        private readonly DataContext dataContext;

        public UserService(DataContext _dataContext)
        {
            this.dataContext = _dataContext;
        }

        public async Task<IResult> GetAllToUsersAsync()
        {
            return TypedResults.Ok(await dataContext.Users.ToArrayAsync());
        }

        public async Task<IResult> GetUserByIdAsync(Guid guid)
        {
            return TypedResults.Ok(await dataContext.Users.Where((c) => c.Id == guid).FirstOrDefaultAsync());
        }


        public async Task<IResult> CreateUserAsync(CreateUser createUser)
        {
            await dataContext.Users.AddAsync(new UserEntity
            {
                NickName = createUser?.NickName ?? string.Empty,
                PassWord = createUser?.PassWord ?? string.Empty,
                UserName = createUser?.UserName ?? string.Empty,
                Id = Guid.NewGuid(),
            });
            return TypedResults.Ok("插入成功");
        }

        public IResult UpdateUser(EditUser editUser)
        {
             dataContext.Users.Update(new UserEntity
             {
                NickName = editUser?.NickName ?? string.Empty,
                PassWord = editUser?.PassWord ?? string.Empty,
                UserName = editUser?.UserName ?? string.Empty,
                Id = Guid.NewGuid(),
            });
            return TypedResults.Ok("update success");
        }

    }
}
