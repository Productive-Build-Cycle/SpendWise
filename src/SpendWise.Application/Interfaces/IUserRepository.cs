using SpendWise.Domain.Entities;

namespace SpendWise.Application.Interfaces;

public interface IUserRepository
{
    void Add(User user);
}