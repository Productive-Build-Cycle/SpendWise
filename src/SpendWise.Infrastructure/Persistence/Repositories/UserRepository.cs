using SpendWise.Application.Interfaces;
using SpendWise.Domain.Entities;

namespace SpendWise.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    
    public void Add(User user)
    {
        _dbContext.Users.Add(user);
    }
}