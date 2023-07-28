using System.Data;
using System.Security.Cryptography;
using DotnetAPI.Data;
using DotnetAPI.Dtos;
using DotnetAPI.Helpers;
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
        public AuthController(IConfiguration config)
        {
            _dapper = new DataContextDapper(config);
            _authHelper = new AuthHelper(config);
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
                    //"A salt is a piece of random data added to a password before it is hashed and stored"
                    byte[] passwordSalt = new byte[128 / 8];
                    using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
                    {
                        rng.GetNonZeroBytes(passwordSalt);
                    }

                    byte[] passwordHash = _authHelper.GetPasswordHash(userForRegistration.Password, passwordSalt);

                    string sqlAddAuth = @"
                        INSERT INTO TutorialAppSchema.Auth  ([Email],
                        [PasswordHash],
                        [PasswordSalt]) VALUES ('" + userForRegistration.Email +
                        "', @PasswordHash, @PasswordSalt)";

                    List<SqlParameter> parameters = new List<SqlParameter>();
                    SqlParameter passwordSaltParameter = new SqlParameter("@PasswordSalt", SqlDbType.VarBinary);
                    passwordSaltParameter.Value = passwordSalt;

                    SqlParameter passwordHashParameter = new SqlParameter("@PasswordHash", SqlDbType.VarBinary);
                    passwordHashParameter.Value = passwordHash;

                    parameters.Add(passwordSaltParameter);
                    parameters.Add(passwordHashParameter);
                    if (_dapper.ExecuteSqlWithParameters(sqlAddAuth, parameters))
                    {
                        string sqlAddUser = @"
                                    INSERT INTO TutorialAppSchema.Users(
                                    [FirstName],
                                    [LastName],
                                    [Email],
                                    [Gender],
                                    [Active]
                                    ) VALUES ( " +
                                        " '" + userForRegistration.FirstName +
                                        "', '" + userForRegistration.LastName +
                                        "', '" + userForRegistration.Email +
                                        "', '" + userForRegistration.Gender +
                                        "', 1)";
                        if (_dapper.ExecuteSql(sqlAddUser))
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
        [AllowAnonymous]
        [HttpPost("Login")]
        public IActionResult Login(UserForLoginDto userForLogin)
        {
            string sqlForHashAndSalt = @"SELECT Email, PasswordHash, PasswordSalt FROM TutorialAppSchema.Auth
                WHERE Email = '" + userForLogin.Email + "'";
            UserForLoginConfirmationDto userForLoginConfirmation = _dapper.LoadDataSingle<UserForLoginConfirmationDto>(sqlForHashAndSalt);

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