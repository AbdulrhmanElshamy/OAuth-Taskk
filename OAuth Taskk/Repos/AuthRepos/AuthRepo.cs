using Google.Apis.Auth;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OAuth_Taskk.Dto;
using OAuth_Taskk.Helpers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OAuth_Taskk.Repos.AuthRepos
{
    public class AuthRepo : IAuthRepo
    {
        private readonly UserManager<IdentityUser> _userManager;  
        private readonly JWT _jwt;
        private readonly Authentication _goolgeSettings;

        public AuthRepo(UserManager<IdentityUser> userManager, IOptions<JWT> jwt, IOptions<Authentication> googlestteing)
        {
            _userManager = userManager;
            _jwt = jwt.Value;
            _goolgeSettings = googlestteing.Value;
           
        }


        private async Task<GoogleJsonWebSignature.Payload> VerifyGoogleToken(ExternalAuthDto externalAuth)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new List<string>() { _goolgeSettings.google.ClientId }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(externalAuth.IdToken, settings);
                return payload;
            }
            catch (Exception ex)
            {
                return null;
            }
        }




        public async Task<Respons<AuthDTO>> RegisterAsync(RegisterDTO registerDTO)
        {
            if (await _userManager.FindByEmailAsync(registerDTO.Email) is not null)
            {
                return new Respons<AuthDTO>
                {
                    Data = new AuthDTO
                    {
                        Email = registerDTO.Email,
                        Username = registerDTO.Username,
                    },
                    Error = "email is exist"
                };
            }

            if (await _userManager.FindByNameAsync(registerDTO.Username) is not null)
            {
                return new Respons<AuthDTO>
                {
                    Data = new AuthDTO
                    {
                        Email = registerDTO.Email,
                        Username = registerDTO.Username,
                    },
                    Error = "username is exist"
                };
            }

            var user = new IdentityUser
            {
                Email = registerDTO.Email,
                UserName = registerDTO.Username,
            };

            var result = await _userManager.CreateAsync(user, registerDTO.Password);

            if (!result.Succeeded)
            {
                var errors = string.Empty;
                foreach (var item in result.Errors)
                {
                    errors += $"{item}.";
                }

                return new Respons<AuthDTO>
                {
                    Data = new AuthDTO
                    {
                        Email = registerDTO.Email,
                        Username = registerDTO.Username,
                    },
                    Error = errors
                };
            }

            var jwtSecurtiToken = await CreateJwtToken(user);

            return new Respons<AuthDTO>
            {
                Data = new AuthDTO
                {
                    Username = registerDTO.Username,
                    Email = registerDTO.Email,
                    IsAuthenticated = true,
                    ExpireOn = jwtSecurtiToken.ValidTo,
                    Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurtiToken)
                }
            };
        }

        public async Task<Respons<AuthDTO>> GetToken(TokenRequestDTO dTO)
        {
            var auth = new AuthDTO();

            var user = await _userManager.FindByEmailAsync(dTO.Email);

            if (user is null || !await _userManager.CheckPasswordAsync(user, dTO.Password))
            {
                return new Respons<AuthDTO>
                {
                    Data = new AuthDTO
                    {
                        Email = user.Email,
                        Username = user.UserName,
                    },
                    Error = "Email Or Password Is Not Correct"
                };
            }

            var jwtSecurtiToken = await CreateJwtToken(user);

            auth.IsAuthenticated = true;
            auth.ExpireOn = jwtSecurtiToken.ValidTo;
            auth.Token = new JwtSecurityTokenHandler().WriteToken(jwtSecurtiToken);
            auth.Email = user.Email;
            auth.Username = user.UserName;

            return new Respons<AuthDTO>
            {
                Data = auth,
            };

        }

        private async Task<JwtSecurityToken> CreateJwtToken(IdentityUser user)
        {
            var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.key));
            var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

            var jwtSecurityToken = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                expires: DateTime.Now.AddDays(_jwt.DurationInDays),
                signingCredentials: signingCredentials);

            return jwtSecurityToken;
        }

        public async Task<Respons<AuthDTO>> ExtrnalLogin(ExternalAuthDto externalAuth)
        {
            var payload = await VerifyGoogleToken(externalAuth);
            if (payload == null)
                return new Respons<AuthDTO>() { Error = "ex" };

            var info = new UserLoginInfo(externalAuth.Provider, payload.Subject, externalAuth.Provider);

            var user = await _userManager.FindByLoginAsync(info.LoginProvider, info.ProviderKey);
            if (user == null)
            {
                user = await _userManager.FindByEmailAsync(payload.Email);

                if (user == null)
                {
                    user = new IdentityUser { Email = payload.Email, UserName = payload.Email };
                    await _userManager.CreateAsync(user);

                    //prepare and send an email for the email confirmation
                    await _userManager.AddLoginAsync(user, info);
                }
                else
                {
                    await _userManager.AddLoginAsync(user, info);
                }
            }

            if (user == null)
                return new Respons<AuthDTO>() { Error = "ex" };

            //check for the Locked out account

            var token = await CreateJwtToken(user);
            return new Respons<AuthDTO>()
            {
                Data = new AuthDTO()
                {
                    IsAuthenticated = true,
                    Token = new JwtSecurityTokenHandler().WriteToken(token),
                    Email = payload.Email,
                    ExpireOn = token.ValidTo
                }
            };
        }
    }
}
