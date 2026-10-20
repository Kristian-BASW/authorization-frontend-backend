using DataAccess.Entities;
using Security.Dtos;

namespace Security.Services;

public interface IUserService
{
    User? TryLogin(TryLogin user);
    
    void TryRegister(CreateUserRequest user);
    List<User>  GetUsers();
}
