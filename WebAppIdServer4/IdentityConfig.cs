using IdentityModel;
using IdentityServer4;
using IdentityServer4.Models;
using IdentityServer4.Test;
using System.Security.Claims;

namespace WebAppIdServer4
{
    public class IdentityConfig
    {
        /// <summary>
        /// 定义身份资源
        /// </summary>
        public static IEnumerable<IdentityResource> IdentityResources =>
         new IdentityResource[]
         {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
         };

        /// <summary>
        /// ApiResource 的 Scope 正式独立出来为 ApiScope 对象，区别ApiResource 和 Scope的关系, Scope 是属于ApiResource 的一个属性，可以包含多个Scope。
        /// </summary>
        public static IEnumerable<ApiResource> ApiResources =>
            new ApiResource[] {
            new ApiResource("api1","api1")
            {
                Scopes={ 
                    //"client_scope1",
                    //"Implicit_scope1",
                    "code_scope1"
                },
                UserClaims={ JwtClaimTypes.Role,ClaimTypes.Role},
                ApiSecrets={ new Secret("apipwd".Sha256())}
            }
        };


        /// <summary>
        /// Authorization Server保护了哪些 API Scope（作用域）
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<ApiScope> GetApiScopes()
        {
            return new ApiScope[]
            {
                new ApiScope(
                    //"client_scope1",
                    "code_scope1"
                    ),
            };
        }


        /// <summary>
        /// 哪些客户端 Client（应用） 可以使用这个 Authorization Server
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Client> GetClients()
        {
            return new[]
            {
                // ClientCredentials（客户端凭证）
                new Client()
                {
                    ClientId="YuanIdentity", ///客户端的标识，要是惟一的
                    ClientName="YuanIdentity",
                    ClientSecrets=new []{new Secret("6KGqzUx6nfZZp0a4NH2xenWSJQWAT8la".Sha256())}, ////客户端密码，进行了加密
                    AllowedGrantTypes= GrantTypes.ClientCredentials, ////授权方式，这里采用的是客户端认证模式，只要ClientId，以及ClientSecrets正确即可访问对应的AllowedScopes里面的api资源
                    AllowedScopes=new[]{"client_scope1" }, //定义这个客户端可以访问的APi资源数组，上面只有一个api
                   
                },
                // ResourceOwnerPassword（密码模式）
                new Client()
                {
                  ClientId="YuanIdentityPass",
                  ClientName="YuanIdentityPass",
                  AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
                  ClientSecrets = { new Secret("6KGqzUx6nfZZp0a4NH2xenWSJQWAT8la".Sha256()) },
                  AllowedScopes={ "client_scope1" }
                },
                // 简化模式
                new Client ()
                {
                    ClientId="ImplicitClient",
                    ClientName="ImplicitClient",
                    AllowedGrantTypes=GrantTypes.Implicit,
                    //ClientSecrets={ new Secret("6KGqzUx6nfZZp0a4NH2xenWSJQWAT8la".Sha256()) },
                    RedirectUris={
                        // 跳转登录到的客户端的地址
                        "http://localhost:5003/signin-oidc",  
                    },
                    //PostLogoutRedirectUris={
                        // 跳转登出到的客户端的
                        //"http://localhost:5003/signout-callback-oidc",
                    //},
                    AllowedScopes={
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "Implicit_scope1"
                    },
                    // 是否需要同意授权
                    RequireConsent=true,
                    AllowAccessTokensViaBrowser=true,
                    AlwaysIncludeUserClaimsInIdToken=true,
                },
                new Client()
                {
                    ClientId="CodeClient",
                    ClientName="CodeClient",
                    AllowedGrantTypes=GrantTypes.Code,
                    RedirectUris={
                        // 跳转到客户端的地址
                        "http://localhost:5003/signin-oidc",
                    },
                    // 跳转登出到的客户端的地址
                    PostLogoutRedirectUris={
                        "http://localhost:5003/signout-callback-oidc",
                    },
                    ClientSecrets={ new Secret("6KGqzUx6nfZZp0a4NH2xenWSJQWAT8la".Sha256()) },
                    AllowedScopes =
                    {
                        IdentityServerConstants.StandardScopes.OpenId,
                        IdentityServerConstants.StandardScopes.Profile,
                        "code_scope1"
                    },
                    RequireConsent=true,
                    AllowAccessTokensViaBrowser=true,
                    AlwaysIncludeUserClaimsInIdToken=true,
                }
            };
        }

        /// <summary>
        /// 哪些User可以被这个AuthorizationServer识别并授权
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<TestUser> GetTestUsers()
        {
            return new[]
            {
               new TestUser
               {
                   SubjectId="001",
                   Username="i3yuan",
                   Password="123456",
                   Claims={
                      new Claim(JwtClaimTypes.Name,"letian"),
                      //new Claim(JwtClaimTypes.Role,"admin"),
                      new Claim(ClaimTypes.Role,"admin"),
                      new Claim("username", "zhangsan")
        },
               }
           };
        }
    }
}
