##SpendWise Domain Rules

## 1. Account Balance

- `OpeningBalance` represents the initial amount recorded when an account is created.
- `Balance` represents the current amount recorded for an account.
- For bank accounts, such as Chequing and Savings, a positive balance represents available funds.
- For credit card accounts, a positive balance represents outstanding debt.
- Both `OpeningBalance` and `Balance` must be zero or greater.


## 2. Account Operations

- `Deposit(amount)` is used to add funds to bank accounts, such as Chequing , Savings , DigitalWallet , Cash , and Investment.
- `Withdraw(amount)` is used to remove funds from chequing , savings , digital wallet , cash , and investment accounts. A withdrawal must not exceed the available balance.
- Investment accounts support `Deposit(amount)` and `Withdraw(amount)`. The `Balance` represents the amount recorded in SpendWise and does not necessarily reflect the current 
  market value of investments. 
- Investment trading and market-value calculations are outside the current scope.
- `PayCreditCard(amount)` is used to pay outstanding credit card debt and decreases the credit card balance.
- A credit card purchase increases the outstanding credit card balance.
- All operation amounts must be greater than zero.
- Account operations must respect the account type and must not allow the balance to become negative.
- **Note:** These are proposed domain rules and should be confirmed with the team.


## 3. Transaction Rules

- A transaction must belong to a user.
- The transaction amount must be greater than zero.
- An Income transaction must have a destination account and must not have a source account.
- An Expense transaction must have a source account and must not have a destination account.
- A Transfer transaction must have both a source account and a destination account.
- A transaction cannot have the same source and destination account.
- The source and destination accounts must belong to the transaction's user.
- A transaction date cannot be in the future.
- Income increases the destination account balance.
- Expense decreases the source account balance.
- Transfer decreases the source account balance and increases the destination account balance.
- Account balances must be updated consistently with the transaction.
- A transaction must not cause an account balance to become negative, except where the account type explicitly permits it under the agreed domain rules.

## 4. Budget Rules

- A budget must belong to a user and a category.
- The budget amount must be greater than zero.
- The budget start date must be earlier than its end date.
- A budget applies to expenses associated with its category during its defined date range.
- Budget spending is calculated from the user's expense transactions associated with the budget category within the budget period.
- Income and transfer transactions do not count toward budget spending.
- A budget does not prevent a user from making an expense that exceeds the budget amount.
- A budget may be exceeded, but the application should be able to identify and report the over-budget amount.

## 5. Financial Goal Features

### Contributions
- A contribution allocates money to a financial goal.
- The contribution must be associated with an account that holds the allocated money.
- If the money is already in the selected account, no transfer transaction is required.
- If the money comes from another account, a transfer transaction must be created from the source account to the selected account.
- The contribution amount must be greater than zero.

### Releases
- Releasing money from a goal transfers the released amount to an account selected by the user.
- If the destination is different from the account holding the allocated money, a transfer transaction is required.
- The release amount must be greater than zero and cannot exceed the remaining allocated amount.

### Allocation
- The remaining allocated amount is calculated as total contributions minus total releases.
- Allocated money must be tracked separately for each contribution and its releases.

### Archiving
- A goal can only be archived after its remaining allocated amount has been fully released.
- During archiving, the user selects the destination account for any remaining allocated money.
- If allocated money is held in multiple accounts, the remaining amount must be transferred from each holding account to the selected destination account.
- An already archived goal cannot be archived again.
- Archived goals cannot receive new contributions or releases.
- Goal and contribution history must be preserved after archiving.

### Atomicity
- The transfers, release records, and goal archive operation must succeed or fail together.
- If any part of the archiving operation fails, none of its changes should be committed.

## 6. Category Rules

- A category must belong to a user.
- A category name must not be null, empty, or whitespace.
- A category name must be trimmed before being stored.
- A category description is optional.
- A category description must not exceed 500 characters.
- A user must not have duplicate category names, ignoring case and leading or trailing whitespace.
- A category used by existing transactions or budgets must not be deleted if deletion would break historical records or relationships.
- Categories are used to classify transactions and organize budgets.

## 7. Recurring Transaction Rules

- A recurring transaction must belong to a user.
- The transaction name must not be null, empty, or whitespace.
- The transaction name must be trimmed before being stored.
- The transaction amount must be greater than zero.
- A recurring transaction must have at least one account.
- An Income recurring transaction must have a destination account and must not have a source account.
- An Expense recurring transaction must have a source account and must not have a destination account.
- A Transfer recurring transaction must have both a source account and a destination account.
- The source and destination accounts must not be the same.
- The source and destination accounts must belong to the recurring transaction's user.
- A category is optional. If provided, its ID must not be empty, and the category must belong to the recurring transaction's user.
- The start date cannot be in the past.
- The next occurrence date must be on or after the start date.
- If an end date is provided, it cannot be before the start date.
- If an end date is provided, the next occurrence date cannot be after the end date.
- The recurring frequency must be a valid RecurringFrequency value.
- Recurring transactions generate individual transactions according to their frequency and schedule.

### Updating Recurring Transactions

- Users can update the name, amount, source account, destination account, transaction type, category, frequency, and end date of a recurring transaction.
- The user ID cannot be changed.
- The start date cannot be changed through a normal update.
- Updated values must satisfy all existing recurring transaction business rules.
- The next occurrence date must remain consistent with the updated frequency and schedule.
- Updating a recurring transaction must not modify transactions that have already been generated.

## 8. Currency Rules

- The initial version of SpendWise supports **US Dollar (USD) only**.
- All monetary amounts throughout the application are expressed in USD.
- Every account must have a valid Currency value object.
- During the initial version, an account's currency must be USD.
- Account currency cannot be changed through normal account updates.
- Transactions must use the currency of their associated accounts. Since all accounts use USD, all transactions are expressed in USD.
- Transfers are allowed only between accounts using USD.
- Budget amounts and financial goal target amounts are expressed in USD.
- Goal contributions and releases are expressed in USD.
- Recurring transaction amounts are expressed in USD.
- Currency conversion and foreign exchange rates are outside the scope of the initial version.
- The application must preserve the currency-related design so that multi-currency support can be introduced in a future version.