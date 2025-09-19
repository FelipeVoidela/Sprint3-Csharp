using Dapper;
using Npgsql;
using System.Collections.Generic;
using System.Threading.Tasks;

// Repository
public class Repository : IDisposable
{
    private readonly string _conn;
    private readonly NpgsqlConnection _connection;

    public Repository(string conn)
    {
        _conn = conn;
        _connection = new NpgsqlConnection(_conn);
    }

    public async Task InitializeAsync()
    {
        await _connection.OpenAsync();
        var sqlInst = @"CREATE TABLE IF NOT EXISTS institutions (
            id serial PRIMARY KEY,
            name text NOT NULL,
            address text NOT NULL
        );";
        var sqlUser = @"CREATE TABLE IF NOT EXISTS users (
            id serial PRIMARY KEY,
            name text NOT NULL,
            email text NOT NULL,
            institutionid integer REFERENCES institutions(id) ON DELETE SET NULL
        );";
        await _connection.ExecuteAsync(sqlInst);
        await _connection.ExecuteAsync(sqlUser);
    }

    public async Task<IEnumerable<User>> GetUsersAsync() =>
        await _connection.QueryAsync<User>("SELECT id, name, email, institutionid FROM users ORDER BY id");

    public async Task<User?> GetUserAsync(int id) =>
        await _connection.QuerySingleOrDefaultAsync<User>("SELECT id, name, email, institutionid FROM users WHERE id = @id", new { id });

    public async Task CreateUserAsync(User u) =>
        await _connection.ExecuteAsync("INSERT INTO users(name, email, institutionid) VALUES(@Name, @Email, @InstitutionId)", u);

    public async Task UpdateUserAsync(User u) =>
        await _connection.ExecuteAsync("UPDATE users SET name = @Name, email = @Email, institutionid = @InstitutionId WHERE id = @Id", u);

    public async Task DeleteUserAsync(int id) =>
        await _connection.ExecuteAsync("DELETE FROM users WHERE id = @id", new { id });

    public async Task<IEnumerable<Institution>> GetInstitutionsAsync() =>
        await _connection.QueryAsync<Institution>("SELECT id, name, address FROM institutions ORDER BY id");

    public async Task<Institution?> GetInstitutionAsync(int id) =>
        await _connection.QuerySingleOrDefaultAsync<Institution>("SELECT id, name, address FROM institutions WHERE id = @id", new { id });

    public async Task CreateInstitutionAsync(Institution i) =>
        await _connection.ExecuteAsync("INSERT INTO institutions(name, address) VALUES(@Name, @Address)", i);

    public async Task UpdateInstitutionAsync(Institution i) =>
        await _connection.ExecuteAsync("UPDATE institutions SET name = @Name, address = @Address WHERE id = @Id", i);

    public async Task DeleteInstitutionAsync(int id) =>
        await _connection.ExecuteAsync("DELETE FROM institutions WHERE id = @id", new { id });

    public void Dispose()
    {
        if (_connection != null)
        {
            if (_connection.State == System.Data.ConnectionState.Open)
            {
                _connection.Close();
            }
            _connection.Dispose();
        }
    }
}