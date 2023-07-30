using System.Data;
using System.Security.Cryptography;
using AutoMapper;
using Dapper;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Helpers;
using DotnetAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace DotnetAPI.Controllers
{
    [Authorize]
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly DataContextDapper _dapper;
        private readonly AuthHelper _authHelper;
        private readonly ReusableSql _reusableSql;
        private readonly IMapper _mapper;
        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _authHelper = new AuthHelper(config);
            _reusableSql = new ReusableSql(config);
            _mapper = new Mapper(new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<UserForRegistrationDto, UserComplete>();
            }));
        }

        [AllowAnonymous]
        [HttpPost("Register")]
        public IActionResult Register(UserForRegistrationDto userForRegistration)
        {
            if (userForRegistration.Password == userForRegistration.PasswordConfirm)
            {
                //verificando se o e-mail já é cadastrado
                string sqlCheckUserExists = @"SELECT Email FROM TutorialAppSchema.Users
                    WHERE Email = '" + userForRegistration.Email + "'";
                IEnumerable<string> existingUser = _dapper.LoadData<string>(sqlCheckUserExists);
                if (existingUser.Count() == 0)
                {
                    UserForLoginDto userForSetPassword = new UserForLoginDto()
                    {
                        Email = userForRegistration.Email,
                        Password = userForRegistration.Password
                    };
                    if (_authHelper.setPassword(userForSetPassword))
                    {
                        UserComplete userComplete = _mapper.Map<UserComplete>(userForRegistration);
                        userComplete.Active = true;
                        if (_reusableSql.UpsertUser(userComplete))
                        {
                            return Ok();
                        }
                        throw new Exception("Error to add user!");

                    }
                    throw new Exception("Error to register user!");
                }
                throw new Exception("User with this email already exists!");
            }
            throw new Exception("Password and PasswordConfirm do not match!");

        }

        [HttpPut("ResetPassword")]
        public IActionResult ResetPassword(UserForLoginDto userForSetPassword)
        {
            if (_authHelper.setPassword(userForSetPassword))
            {
                return Ok();
            }
            throw new Exception("Error to update Password");

        }
        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDto userForLogin)
        {
            string sqlForHashAndSalt = @"EXEC TutorialAppSchema.spLoginConfirmation_Get
            @Email =  @EmailParam";

            DynamicParameters parameters = new DynamicParameters();

            // SqlParameter emailParameter = new SqlParameter("@EmailParam", SqlDbType.VarChar);
            // emailParameter.Value = userForLogin.Email;
            // parameters.Add(emailParameter);
            parameters.Add("@EmailParam", userForLogin.Email, DbType.String);
            UserForLoginConfirmationDto userForLoginConfirmation = _dapper.LoadDataSingleWithParameters<UserForLoginConfirmationDto>(sqlForHashAndSalt, parameters);

            byte[] passwordHash = _authHelper.GetPasswordHash(userForLogin.Password, userForLoginConfirmation.PasswordSalt);

            for (int i = 0; i < passwordHash.Length; i++)
            {
                if (passwordHash[i] != userForLoginConfirmation.PasswordHash[i])
                {
                    throw new Exception("Password is incorrect!");
                }
            }
            int userId = _dapper.LoadDataSingle<int>("SELECT UserId FROM TutorialAppSchema.Users WHERE Email = '" + userForLogin.Email + "'");

            return Ok(new Dictionary<string, string>{
                {"token", _authHelper.CreateToken(userId)}
            });
        }
        [HttpGet("RefreshToken")]
        public string RefreshToken()
        {
            string sqlGetUserId = @"SELECT UserId FROM TutorialAppSchema.Users WHERE UserId = '" +
                                    User.FindFirst("userId")?.Value + "'";
            int userId = _dapper.LoadDataSingle<int>(sqlGetUserId);
            return _authHelper.CreateToken(userId);
        }

    }
}