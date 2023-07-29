using Microsoft.AspNetCore.Mvc;
using DotnetAPI.Data;
using DotnetAPI.Models;
using DotnetAPI.Dtos;
using Microsoft.Data.SqlClient;

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
        string parameters = "";
        if (userId != 0)
        {
            parameters += ", @UserId=" + userId.ToString();
        }
        if (isActive)
        {
            parameters += ", @Active=" + isActive.ToString();
        }
        sql += parameters.Substring(1);
        IEnumerable<UserComplete> users = _dapper.LoadData<UserComplete>(sql);
        return users;
    }

    //INSERT OR UPDATE USER PROCEDURE
    [HttpPut("UpsertUser")]
    public IActionResult UpsertUser(UserComplete user)
    {
        string sql = @"EXEC TutorialAppSchema.spUser_Upsert
            @FirstName = '" + user.FirstName +
            "', @LastName = '" + user.LastName +
            "', @Email = '" + user.Email +
            "', @Gender = '" + user.Gender +
            "', @Active = '" + user.Active +
            "', @JobTitle = '" + user.JobTitle +
            "', @Department = '" + user.Department +
            "', @Salary = '" + user.Salary +
            "',  @UserId = " + user.UserId;
        if (_dapper.ExecuteSql(sql))
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
            @UserID = " + userId.ToString();

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Error to delete user!");
    }

}
