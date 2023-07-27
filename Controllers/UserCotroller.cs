using Microsoft.AspNetCore.Mvc;
using DotnetAPI.Data;
using DotnetAPI.Models;
using DotnetAPI.Dtos;
namespace DotnetAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class UserCotroller : ControllerBase
{
    DataContextDapper _dapper;
    public UserCotroller(IConfiguration config)
    {
        _dapper = new DataContextDapper(config);
    }
    [HttpGet("Test")]
    public DateTime Test()
    {
        return _dapper.LoadDataSingle<DateTime>("SELECT GETDATE()");
    }


    [HttpGet("GetUsers/")]
    public IEnumerable<User> GetUsers()
    {
        string sql = @"SELECT 
                    [UserId],
                    [FirstName],
                    [LastName],
                    [Email],
                    [Gender],
                    [Active] 
                    FROM TutorialAppSchema.Users";
        IEnumerable<User> users = _dapper.LoadData<User>(sql);
        return users;
    }

    [HttpGet("GetUsers/{userId}")]
    public User GetSingleUser(int userId)
    {
        string sql = @"SELECT 
                    [UserId],
                    [FirstName],
                    [LastName],
                    [Email],
                    [Gender],
                    [Active] 
                    FROM TutorialAppSchema.Users
                    WHERE UserId = " + userId.ToString();
        User user = _dapper.LoadDataSingle<User>(sql);
        return user;
    }
    [HttpPut("EditUser")]
    public IActionResult EditUser(User user)
    {
        string sql = @"
            UPDATE TutorialAppSchema.Users
            SET [FirstName] = '" + user.FirstName +
            "',[LastName] = '" + user.LastName +
            "',[Email] = '" + user.Email +
            "',[Gender] = '" + user.Gender +
            "',[Active] = '" + user.Active +
            "' WHERE UserId = " + user.UserId;
        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Error updating user!");

    }

    [HttpPost("AddUser")]
    public IActionResult AddUser(UserToAddDto user)
    {
        string sql = @"
            INSERT INTO TutorialAppSchema.Users(
            [FirstName],
            [LastName],
            [Email],
            [Gender],
            [Active]
            ) VALUES ( " +
                " '" + user.FirstName +
                "', '" + user.LastName +
                "', '" + user.Email +
                "', '" + user.Gender +
                "', '" + user.Active +

            "')";

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Error Inserting user!");
    }

    [HttpDelete("DeleteUser/{userId}")]
    public IActionResult DeleteUSER(int userId)
    {
        string sql = @"DELETE FROM TutorialAppSchema.Users
            Where UserID = " + userId.ToString();

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Error to delete user!");
    }

    //Users Salary
    [HttpGet("UserSalary/{userId}")]
    public IEnumerable<UserSalaryDto> GetUserSalary(int userId)
    {
        string sql = @"SELECT UserSalary.UserID
                            , UserSalary.Salary
                            , Users.FirstName
                            , Users.LastName
                        FROM TutorialAppSchema.UserSalary
                        INNER JOIN TutorialAppSchema.Users
                        ON UserSalary.UserID = Users.UserID
                        WHERE UserSalary.UserID = " + userId.ToString();
        IEnumerable<UserSalaryDto> userSalary = _dapper.LoadData<UserSalaryDto>(sql);
        return userSalary;
    }

    [HttpPost("UserSalary")]
    public IActionResult PostUserSalary(UserSalary userSalaryInsert)
    {
        string sql = @"
            INSERT INTO TutorialAppSchema.UserSalary (
                UserId,
                Salary
            ) VALUES (" + userSalaryInsert.UserId.ToString()
                + ", " + userSalaryInsert.Salary
                + ")";
        if (_dapper.ExecuteSqlWithRowCount(sql) > 0)
        {
            return Ok(userSalaryInsert);
        }
        throw new Exception("Adding User Salary failed on save");
    }
    [HttpPut("UserSalary")]
    public IActionResult PutUserSalary(UserSalary userSalaryUpdate)
    {
        string sql = @"UPDATE TutorialAppSchema.UserSalary SET Salary="
        + userSalaryUpdate.Salary
        + " WHERE UserId=" + userSalaryUpdate.UserId.ToString();

        if (_dapper.ExecuteSql(sql))
        {
            return Ok(userSalaryUpdate);
        }
        throw new Exception("Updating User Salary failed on save");
    }

    [HttpDelete("UserSalary/{userId}")]
    public IActionResult DeleteUSerSalary(int userId)
    {
        string sql = @"DELETE FROM TutorialAppSchema.UserSalary WHERE UserId="
        + userId.ToString();

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }
        throw new Exception("Deleting User Salary failed on save");
    }
    //
    //Users Job Info
    [HttpGet("UserJobInfo/{userId}")]
    public IEnumerable<UserJobInfo> GetUserJobInfo(int userId)
    {
        return _dapper.LoadData<UserJobInfo>(@"
        SELECT UserJobInfo.UserId
                , UserJobInfo.JobTitle
                , UserJobInfo.Department
        FROM TutorialAppSchema.UserJobInfo
        WHERE UserJobInfo.UserId = " + userId.ToString());
    }

    [HttpPost("UserJobInfo")]
    public IActionResult PostUserJobInfo(UserJobInfo userJobInfoForInsert)
    {
        string sql = @"
            INSERT INTO TutorialAppSchema.UserJobInfo (
                UserId,
                Department,
                JobTitle
            ) VALUES (" + userJobInfoForInsert.UserId
                + ", '" + userJobInfoForInsert.Department
                + "', '" + userJobInfoForInsert.JobTitle
                + "')";

        if (_dapper.ExecuteSql(sql))
        {
            return Ok(userJobInfoForInsert);
        }
        throw new Exception("Adding User Job Info failed on save");
    }

    [HttpPut("UserJobInfo")]
    public IActionResult PutUserJobInfo(UserJobInfo userJobInfoUpdate)
    {
        string sql = "UPDATE TutorialAppSchema.UserJobInfo SET Department='"
            + userJobInfoUpdate.Department
            + "', JobTitle='"
            + userJobInfoUpdate.JobTitle
            + "' WHERE UserId=" + userJobInfoUpdate.UserId.ToString();

        if (_dapper.ExecuteSql(sql))
        {
            return Ok(userJobInfoUpdate);
        }
        throw new Exception("Updating User Job Info failed on save");
    }

    [HttpDelete("UserJobInfo/{userId}")]
    public IActionResult DeleteUserJobInfo(int userId)
    {
        string sql = @"
            DELETE FROM TutorialAppSchema.UserJobInfo 
                WHERE UserId = " + userId.ToString();

        Console.WriteLine(sql);

        if (_dapper.ExecuteSql(sql))
        {
            return Ok();
        }

        throw new Exception("Failed to Delete User");
    }

}
