using AutoMapper;
using DemoJWT.Entities;
using DemoJWT.Interfaces;
using DemoShared.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DemoJWT.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IMapper _mapper;
        private readonly IConfiguration _config;
        public UserRepository(UserManager<AppUser> userManager, SignInManager<AppUser> signInManager,
            RoleManager<AppRole> roleManager, IConfiguration config, IMapper mapper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _config = config;
            _mapper = mapper;
        }
        public async Task<string> Authenticate(LoginRequestDTO request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null) throw new Exception("Cannot find user name");
            var result = await _signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, true);
            if(!result.Succeeded)
            {
                return null;
            }
            var roles = await _userManager.GetRolesAsync(user);
            var claims = new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.Role, String.Join(";", roles)),
            };
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(_config["Tokens:Issuer"],
                _config["Tokens:Audience"], claims, expires: DateTime.Now.AddHours(3), signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }


        public async Task<bool> Register(RegisterRequestDTO request)
        {
            var user = _mapper.Map<AppUser>(request);
            var result = await _userManager.CreateAsync(user, request.Password);
            if (!result.Succeeded) return false;
            return true;
        }
    }
}
