using DataAccess.Entities;

namespace DataAccess.Repositories;

public interface IUserRepository
{
    public User? GetUserByUsername(string username);
    public void CreateUser(User user);
    public List<User> GetAll();
}