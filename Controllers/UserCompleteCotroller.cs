using Microsoft.AspNetCore.Mvc;
using DotnetAPI.Data;
using DotnetAPI.Models;
using DotnetAPI.Dtos;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;

namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserCompleteCotroller : ControllerBase
{
    DataContextDapper _dapper;
    public UserCompleteCotroller(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }
    //GET USERS INFO, SALARY INFO AND JOB INFO PROCEDURE
    [HttpGet("GetUsersComplete/{userId}/{isActive}")]
    public IEnumerable<UserComplete> GetUsers(int userId, bool isActive)
    {
        string sql = @"EXEC TutorialAppSchema.spUsers_Get";
        string stringParameters = "";
        DynamicParameters sqlParameters = new DynamicParameters();

        if (userId != 0)
        {
            stringParameters += ", @UserId=@UserIdParameter";
            sqlParameters.Add("@UserIdParameter", userId, DbType.Int32);
        }
        if (isActive)
        {
            stringParameters += ", @Active=@ActiveParameter";
            sqlParameters.Add("@ActiveParameter", isActive, DbType.Boolean);
        }
        if (stringParameters.Length > 0)
        {
            sql += stringParameters.Substring(1);
        }

        IEnumerable<UserComplete> users = _dapper.LoadDataWithParameters<UserComplete>(sql, sqlParameters);
        return users;
    }

    //INSERT OR UPDATE USER PROCEDURE
    [HttpPut("UpsertUser")]
    public IActionResult UpsertUser(UserComplete user)
    {
        string sql = @"EXEC TutorialAppSchema.spUser_Upsert
            @FirstName = @FirsNameParam,
            @LastName = @LastNameParam,
            @Email = @EmailParam,
            @Gender = @GenderParam,
            @Active = @ActiveParam,
            @JobTitle = @JobTitleParam,
            @Department = @DepartmentParam,
            @Salary = @SalaryParam,
            @UserId = @UserIdParam";
        DynamicParameters sqlParameters = new DynamicParameters();
        sqlParameters.Add("@FirsNameParam", user.FirstName, DbType.String);
        sqlParameters.Add("@LastNameParam", user.LastName, DbType.String);
        sqlParameters.Add("@EmailParam", user.Email, DbType.String);
        sqlParameters.Add("@GenderParam", user.Gender, DbType.String);
        sqlParameters.Add("@ActiveParam", user.Active, DbType.Boolean);
        sqlParameters.Add("@JobTitleParam", user.JobTitle, DbType.String);
        sqlParameters.Add("@DepartmentParam", user.Department, DbType.String);
        sqlParameters.Add("@SalaryParam", user.Salary, DbType.Decimal);
        sqlParameters.Add("@UserIdParam", user.UserId, DbType.Int32);
        if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
        {
            return Ok();
        }

        throw new Exception("Error updating user!");

    }
    //DELETE PROCEDURE
    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUSER(int userId)
    {
        string sql = @"EXEC TutorialAppSchema.spUser_Delete
            @UserID = @UserIdParam";

        DynamicParameters sqlParameters = new DynamicParameters();
        sqlParameters.Add("@UserIdParam", userId, DbType.Int32);

        if (_dapper.ExecuteSqlWithParameters(sql, sqlParameters))
        {
            return Ok();
        }

        throw new Exception("Error to delete user!");
    }

}
