using Google.Apis.Auth;
using OAuth_Taskk.Dto;
using OAuth_Taskk.Helpers;

namespace OAuth_Taskk.Repos.AuthRepos
{
    public interface IAuthRepo
    {

        Task<Respons<AuthDTO>> ExtrnalLogin(ExternalAuthDto externalAuth);

        Task<Respons<AuthDTO>> RegisterAsync(RegisterDTO registerDTO);

        Task<Respons<AuthDTO>> GetToken(TokenRequestDTO tokenRequestDTO);
    }
}
