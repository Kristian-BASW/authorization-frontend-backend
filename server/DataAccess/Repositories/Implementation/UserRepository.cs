using DataAccess.Entities;

namespace DataAccess.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _dbContext;

    public UserRepository(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public User? GetUserByUsername(string username)
    {
        return _dbContext.Users.Where(x => x.Username == username)
            .FirstOrDefault();
            
    }

    public void CreateUser(User user)
    {
        if (_dbContext.Users.Any(x => x.Username == user.Username) == false)
        {
            _dbContext.Users.Add(user);
            _dbContext.SaveChanges();
        }
    }

    public List<User> GetAll()
    {
        return _dbContext.Users.ToList();
    }
}