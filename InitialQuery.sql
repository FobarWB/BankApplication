-- SQLite

DELETE FROM Transactions;
DELETE FROM Accounts;
DELETE FROM sqlite_sequence WHERE NAME='Transactions';
DELETE FROM sqlite_sequence WHERE NAME='Accounts';
INSERT INTO Accounts (Name, Balance, CreatedAt)
VALUES
    ('John Doe', 1000.00, '2024-11-21'),
    ('Jane Smith', 2000.00, '2024-11-21'),
    ('Alice Johnson', 1500.00, '2024-11-21'),
    ('Bob Brown', 3000.00, '2024-11-21'),
    ('Charlie Davis', 5000.00, '2024-11-21');

INSERT INTO Transactions (FromAccountNumber, ToAccountNumber, Type, Amount, Date)
VALUES
    (1, 2, 'Transfer', 100.00, '2024-11-21'),
    (2, 3, 'Transfer', 200.00, '2024-11-21'),
    (3, 4, 'Transfer', 300.00, '2024-11-21'),
    (4, 5, 'Transfer', 400.00, '2024-11-21'),
    (5, 1, 'Transfer', 500.00, '2024-11-21');