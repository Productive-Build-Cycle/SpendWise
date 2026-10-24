using SpendWise.Domain.Exceptions;
using SpendWise.Domain.Common;
using System;
using System.Collections.Generic;


namespace SpendWise.Domain.Entities;

public sealed class User : BaseEntity
{
    public string FirstName { get; private set; } = null!;
    public string LastName { get; private set; } = null!;

   public  ICollection<Account> Accounts { get;private set; } = new List<Account>();
    public ICollection<Transactions> Transactions { get; private set; } = new List<Transactions>();

    public ICollection<Budget> Budgets { get; private set; } = new List<Budget>();

    public ICollection<Category> Categories { get; private set; } = new List<Category>();

    public ICollection<FinancialGoal> FinancialGoals { get; private set; } = new List<FinancialGoal>();

    public ICollection<RecurringTransaction> RecurringTransactions { get; private set; } = new List<RecurringTransaction>();
    public User(Guid id, string firstName, string lastName)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("Invalid user id", nameof(id));

        Id = id;
        SetFirstName(firstName);
        SetLastName(lastName);
    }

    public void SetFirstName(string firstName)
    {
        if (string.IsNullOrWhiteSpace(firstName)) throw new ValidationException("نام نمی تواند خالی باشد.");
        firstName = firstName.Trim();
        if (firstName.Length < 2 || firstName.Length > 50)
            throw new ValidationException("نام باید بین 2 تا 50 کاراکتر باشد.");
        FirstName = firstName;
    }

    public void SetLastName(string lastName)
    {
        if (string.IsNullOrWhiteSpace(lastName)) throw new ValidationException("نام خانوادگی نمی تواند خالی باشد.");
        lastName = lastName.Trim();
        if (lastName.Length < 2 || lastName.Length > 50)
            throw new ValidationException("نام خانوادگی باید بین 2 تا 50 کاراکتر باشد.");
        LastName = lastName;
    }
}