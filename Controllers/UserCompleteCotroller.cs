using Microsoft.AspNetCore.Mvc;
using DotnetAPI.Data;
using DotnetAPI.Models;
using DotnetAPI.Dtos;
using Microsoft.Data.SqlClient;
using Dapper;
using System.Data;
using DotnetAPI.Helpers;
using Microsoft.AspNetCore.Authorization;

namespace DotnetAPI.Controllers;

[Authorize]
[ApiController]
[Route("[controller]")]
public class UserCompleteCotroller : ControllerBase
{
    private readonly DataContextDapper _dapper;
    private readonly ReusableSql _reusableSql;
    public UserCompleteCotroller(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
        _reusableSql = new ReusableSql(config);
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
        if (_reusableSql.UpsertUser(user))
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
