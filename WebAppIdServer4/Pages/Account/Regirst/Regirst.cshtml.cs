using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using WebAppIdServer4.Model.Entity;

namespace WebAppIdServer4.Pages.Account.Regirst
{
    public class RegirstModel : PageModel
    {
        private readonly UserManager<UserEntity> userManager;
        private readonly SignInManager<UserEntity> signInManager;

        public RegirstModel(UserManager<UserEntity> _userManager, SignInManager<UserEntity> _signInManager)
        {
            userManager=_userManager;
            signInManager=_signInManager;
        }

        [BindProperty]
        public RegirstInputModel Input { get; set; }

        public async void OnGet()
        {
            if (Input.Password == Input.AginPassword)
            {
                UserEntity userEntity = await userManager.FindByNameAsync(Input.Username);
                if (userEntity != null)
                    throw new Exception("用户名已存在");

                userEntity = new UserEntity()
                {
                     UserName   = Input.Username,
                     PasswordHash= Input.Password.GetHashCode().ToString(),
                };

                await userManager.CreateAsync(userEntity);

                await signInManager.SignInAsync(userEntity,true);
            }

        }
    }
}
