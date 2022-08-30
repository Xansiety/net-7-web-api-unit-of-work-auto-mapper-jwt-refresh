using API.Helpers;
using Core.Entities.Auth;
using Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace API.Services
{
    public class UserService : IUserService
    {
        private readonly JWT _jwt;
        private readonly IUnityOfWork _unitOfWork;
        private readonly IPasswordHasher<Usuario> _passwordHasher;

        public UserService(IUnityOfWork unitOfWork, IOptions<JWT> jwt,
            IPasswordHasher<Usuario> passwordHasher)
        {
            _jwt = jwt.Value;
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

    }
}
