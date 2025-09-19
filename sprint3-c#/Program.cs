using Microsoft.Extensions.Configuration;
using System.Text.Json;
using System.Globalization;

public class Program
{
    private static IConfiguration _config;
    private static Repository _repo;

    public static async Task Main(string[] args)
    {
        _config = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: true)
            .Build();

        var connString = _config.GetConnectionString("Default") ?? "Host=localhost;Port=5432;Username=postgres;Password=Felipe123;Database=sprint3_db";

        try
        {
            _repo = new Repository(connString);
            await _repo.InitializeAsync();

            Console.WriteLine("SISTEMA - Usuários e Instituições");
            await ShowMainMenu();
        }
        catch (Npgsql.NpgsqlException ex)
        {
            Console.WriteLine($"\nErro de conexão com o banco de dados: {ex.Message}");
            Console.WriteLine("Verifique se o PostgreSQL está em execução e se a connection string está correta.");
        }
        finally
        {
            _repo?.Dispose();
        }
    }

    private static async Task ShowMainMenu()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("1 - Gerenciar Usuários");
            Console.WriteLine("2 - Gerenciar Instituições");
            Console.WriteLine("3 - Importar/Exportar (json/txt)");
            Console.WriteLine("0 - Sair");
            Console.Write("Escolha: ");
            var opt = Console.ReadLine();

            try
            {
                switch (opt)
                {
                    case "1":
                        await ManageUsers();
                        break;
                    case "2":
                        await ManageInstitutions();
                        break;
                    case "3":
                        await ImportExport();
                        break;
                    case "0":
                        return;
                    default:
                        Console.WriteLine("Opção inválida.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nOcorreu um erro: {ex.Message}");
            }
        }
    }

    private static async Task ManageUsers()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Usuarios: 1-Listar 2-Criar 3-Editar 4-Deletar 0-Voltar");
            Console.Write("Escolha: ");
            var o = Console.ReadLine();
            if (o == "0") return;

            try
            {
                switch (o)
                {
                    case "1":
                        var users = await _repo.GetUsersAsync();
                        foreach (var u in users) Console.WriteLine(u);
                        break;
                    case "2":
                        var newUser = new User();
                        Console.Write("Nome: ");
                        newUser.Name = Console.ReadLine() ?? "";
                        Console.Write("Email: ");
                        newUser.Email = Console.ReadLine() ?? "";
                        Console.Write("InstituicaoId (opcional): ");
                        var s = Console.ReadLine();
                        newUser.InstitutionId = string.IsNullOrWhiteSpace(s) ? null : int.Parse(s);
                        await _repo.CreateUserAsync(newUser);
                        Console.WriteLine("Usuário criado com sucesso.");
                        break;
                    case "3":
                        Console.Write("ID do usuário a editar: ");
                        var userIdToEdit = int.Parse(Console.ReadLine() ?? "0");
                        var userToUpdate = await _repo.GetUserAsync(userIdToEdit);
                        if (userToUpdate == null)
                        {
                            Console.WriteLine("Usuário não encontrado.");
                            continue;
                        }
                        Console.Write($"Nome ({userToUpdate.Name}): ");
                        var newUserName = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(newUserName)) userToUpdate.Name = newUserName;

                        Console.Write($"Email ({userToUpdate.Email}): ");
                        var newUserEmail = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(newUserEmail)) userToUpdate.Email = newUserEmail;

                        Console.Write($"InstituicaoId ({userToUpdate.InstitutionId}): ");
                        var newUserInstId = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(newUserInstId)) userToUpdate.InstitutionId = int.Parse(newUserInstId);

                        await _repo.UpdateUserAsync(userToUpdate);
                        Console.WriteLine("Usuário atualizado com sucesso.");
                        break;
                    case "4":
                        Console.Write("ID do usuário a deletar: ");
                        var userIdToDelete = int.Parse(Console.ReadLine() ?? "0");
                        await _repo.DeleteUserAsync(userIdToDelete);
                        Console.WriteLine("Usuário deletado com sucesso.");
                        break;
                    default:
                        Console.WriteLine("Opção inválida.");
                        break;
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Entrada inválida. Por favor, insira um número.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocorreu um erro: {ex.Message}");
            }
        }
    }

    private static async Task ManageInstitutions()
    {
        while (true)
        {
            Console.WriteLine();
            Console.WriteLine("Instituicoes: 1-Listar 2-Criar 3-Editar 4-Deletar 0-Voltar");
            Console.Write("Escolha: ");
            var o = Console.ReadLine();
            if (o == "0") return;

            try
            {
                switch (o)
                {
                    case "1":
                        var institutions = await _repo.GetInstitutionsAsync();
                        foreach (var i in institutions) Console.WriteLine(i);
                        break;
                    case "2":
                        var newInstitution = new Institution();
                        Console.Write("Nome: ");
                        newInstitution.Name = Console.ReadLine() ?? "";
                        Console.Write("Endereco: ");
                        newInstitution.Address = Console.ReadLine() ?? "";
                        await _repo.CreateInstitutionAsync(newInstitution);
                        Console.WriteLine("Instituição criada com sucesso.");
                        break;
                    case "3":
                        Console.Write("ID da instituição a editar: ");
                        var instIdToEdit = int.Parse(Console.ReadLine() ?? "0");
                        var instToUpdate = await _repo.GetInstitutionAsync(instIdToEdit);
                        if (instToUpdate == null)
                        {
                            Console.WriteLine("Instituição não encontrada.");
                            continue;
                        }

                        Console.Write($"Nome ({instToUpdate.Name}): ");
                        var newInstName = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(newInstName)) instToUpdate.Name = newInstName;

                        Console.Write($"Endereco ({instToUpdate.Address}): ");
                        var newInstAddress = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(newInstAddress)) instToUpdate.Address = newInstAddress;

                        await _repo.UpdateInstitutionAsync(instToUpdate);
                        Console.WriteLine("Instituição atualizada com sucesso.");
                        break;
                    case "4":
                        Console.Write("ID da instituição a deletar: ");
                        var instIdToDelete = int.Parse(Console.ReadLine() ?? "0");
                        await _repo.DeleteInstitutionAsync(instIdToDelete);
                        Console.WriteLine("Instituição deletada com sucesso.");
                        break;
                    default:
                        Console.WriteLine("Opção inválida.");
                        break;
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Entrada inválida. Por favor, insira um número.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ocorreu um erro: {ex.Message}");
            }
        }
    }

    private static async Task ImportExport()
    {
        Console.WriteLine("1 - Exportar JSON (usuarios + instituicoes)");
        Console.WriteLine("2 - Importar JSON");
        Console.WriteLine("3 - Exportar TXT");
        Console.WriteLine("4 - Importar TXT");
        Console.Write("Escolha: ");
        var o = Console.ReadLine();

        // Define o caminho para a pasta 'export' no diretório do projeto
        var exportPath = Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "export");
        Directory.CreateDirectory(exportPath);

        var jsonFilePath = Path.Combine(exportPath, "export.json");
        var txtFilePath = Path.Combine(exportPath, "export.txt");

        switch (o)
        {
            case "1":
                var data = new { users = await _repo.GetUsersAsync(), institutions = await _repo.GetInstitutionsAsync() };
                var json = JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true });
                await File.WriteAllTextAsync(jsonFilePath, json);
                Console.WriteLine($"Arquivo '{jsonFilePath}' salvo.");
                break;
            case "2":
                try
                {
                    var j = await File.ReadAllTextAsync(jsonFilePath);
                    var doc = JsonSerializer.Deserialize<JsonDocument>(j);
                    if (doc == null) return;

                    if (doc.RootElement.TryGetProperty("institutions", out var insts))
                    {
                        foreach (var el in insts.EnumerateArray())
                        {
                            var inst = JsonSerializer.Deserialize<Institution>(el.GetRawText());
                            if (inst != null) await _repo.CreateInstitutionAsync(inst);
                        }
                    }

                    if (doc.RootElement.TryGetProperty("users", out var usrs))
                    {
                        foreach (var el in usrs.EnumerateArray())
                        {
                            var u = JsonSerializer.Deserialize<User>(el.GetRawText());
                            if (u != null) await _repo.CreateUserAsync(u);
                        }
                    }
                    Console.WriteLine("Dados importados do JSON.");
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine($"Arquivo '{jsonFilePath}' não encontrado.");
                }
                break;
            case "3":
                var users = await _repo.GetUsersAsync();
                var insts = await _repo.GetInstitutionsAsync();
                using (var sw = new StreamWriter(txtFilePath))
                {
                    sw.WriteLine("Institutions:");
                    foreach (var i in insts) sw.WriteLine($"{i.Id}|{i.Name}|{i.Address}");
                    sw.WriteLine("Users:");
                    foreach (var u in users) sw.WriteLine($"{u.Id}|{u.Name}|{u.Email}|{u.InstitutionId}");
                }
                Console.WriteLine($"Arquivo '{txtFilePath}' salvo.");
                break;
            case "4":
                try
                {
                    var lines = await File.ReadAllLinesAsync(txtFilePath);
                    var isInstitutionsSection = false;
                    var isUsersSection = false;

                    foreach (var l in lines)
                    {
                        if (l.StartsWith("Institutions:"))
                        {
                            isInstitutionsSection = true;
                            isUsersSection = false;
                            continue;
                        }
                        if (l.StartsWith("Users:"))
                        {
                            isInstitutionsSection = false;
                            isUsersSection = true;
                            continue;
                        }
                        if (string.IsNullOrWhiteSpace(l)) continue;

                        var parts = l.Split('|');
                        if (isInstitutionsSection && parts.Length == 3)
                        {
                            var inst = new Institution { Name = parts[1], Address = parts[2] };
                            await _repo.CreateInstitutionAsync(inst);
                        }
                        else if (isUsersSection && parts.Length >= 3)
                        {
                            var instId = parts.Length == 4 && int.TryParse(parts[3], out var id) ? (int?)id : null;
                            var user = new User { Name = parts[1], Email = parts[2], InstitutionId = instId };
                            await _repo.CreateUserAsync(user);
                        }
                    }
                    Console.WriteLine("Dados importados do TXT.");
                }
                catch (FileNotFoundException)
                {
                    Console.WriteLine($"Arquivo '{txtFilePath}' não encontrado.");
                }
                break;
            default:
                Console.WriteLine("Opção inválida.");
                break;
        }
    }
}