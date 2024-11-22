-- SQLite

DELETE FROM Transactions;
DELETE FROM Accounts;
DELETE FROM sqlite_sequence WHERE NAME='Transactions';
DELETE FROM sqlite_sequence WHERE NAME='Accounts';
INSERT INTO Accounts (Name, Balance, CreatedAt)
VALUES
    ('John Doe', 1000.00, datetime('now')),
    ('Jane Smith', 2000.00, datetime('now')),
    ('Alice Johnson', 1500.00, datetime('now')),
    ('Bob Brown', 3000.00, datetime('now')),
    ('Charlie Davis', 5000.00, datetime('now'));