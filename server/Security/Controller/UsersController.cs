using DataAccess.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Security.Services;

namespace Security.Controller;

[Route("api/[controller]")]
public class UsersController(IUserService userService): ControllerBase
{
    [Authorize]
    [HttpGet]
    public ActionResult<List<User>> GetAll()
    {
        try
        {
            return Ok(userService.GetUsers());
        }
        catch (Exception e)
        {
            return BadRequest("Could not get users");
        }
    }
}