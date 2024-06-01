using Microsoft.EntityFrameworkCore;
using Models;

namespace Server
{
    public class DbServer
    {
        public static async Task ShowFirst5RowsOfEveryTable()
        {
            try
            {
                await using ServerDbContext _db = new();

                IQueryable<User> users = _db.User
                    .OrderByDescending(u => u.Id)
                    .Take(5);

                IQueryable<Field> fields = _db.Field
                    .OrderByDescending(f => f.Id)
                    .Take(5);

                IQueryable<Cell> cells = _db.Cell
                    .OrderByDescending(c => c.Id)
                    .Take(5)
                    .Include(item => item.Field);

                IQueryable<Game> games = _db.Game
                    .OrderByDescending(g => g.Id)
                    .Take(5)
                    .Include(item => item.HostUser)
                    .Include(item => item.ClientUser);

                Console.WriteLine("\nUsers:\n");
                foreach (User user in users)
                {
                    Console.WriteLine($"Id: {user.Id} | Login: {user.Login} | Password: {user.Password}");
                }

                Console.WriteLine("\nGames:\n");
                foreach (var game in games)
                {
                    Console.WriteLine($"Id: {game.Id} | Name: {game.Name} | IsPrivate?: {game.IsPrivate}" +
                        $"Password: {game.Password} | HostUserId: {game.HostUser.Id} | ClientUserId: {game.ClientUser.Id}");
                }
                
                Console.WriteLine("\nFields:\n");
                foreach (var field in fields)
                {
                    Console.WriteLine($"Id: {field.Id}");
                }
                
                Console.WriteLine("\nCells:\n");
                foreach (var cell in cells)
                {
                    Console.WriteLine($"Id: {cell.Id} | FieldId: {cell.Field.Id} | " +
                        $"IsContainsShip?: {cell.IsContainsShip} | IsHit?: {cell.IsHit}");
                }
            }
            catch
            {
                throw;
            }
        }

        public static async Task<bool> RegisterUser(string login, string pass)
        {
            try
            {
                await using ServerDbContext _db = new();

                foreach (User user in _db.User)
                {
                    if (user.Login == login)
                    {
                        return false;
                    }
                }

                await _db.User.AddAsync(new()
                {
                    Login = login,
                    Password = pass
                });

                await _db.SaveChangesAsync();

                return true;
            }
            catch
            {
                throw;
            }
        }
        public static async Task<User?> SignIn(string login, string pass)
        {
            try
            {
                await using ServerDbContext _db = new();

                User? res = null;

                foreach (User user in _db.User)
                {
                    if (user.Login == login && user.Password == pass)
                    {
                        res = user;
                    }
                }

                return res;
            }
            catch
            {
                throw;
            }
        }

        public static async Task<List<Game>> GetGameList()
        {
            try
            {
                await using ServerDbContext _db = new();

                List<Game> games = await _db.Game
                    .Where(item => item.ClientUser == null)
                    .Include(item => item.HostUser)
                    .Include(item => item.ClientUser)
                    .ToListAsync();

                return games;
            }
            catch
            {
                throw;
            }
        }

        public static async Task<Game?> GetGame(int gameId)
        {
            try
            {
                await using ServerDbContext _db = new();

                Game? game = await _db.Game
                    .Where(item => item.Id == gameId)
                    .Include(item => item.ClientUser)
                    .FirstOrDefaultAsync();

                return game;
            }
            catch
            {
                throw;
            }
        }

        public static async Task<Game?> JoinGame(int gameId, int userId)
        {
            try
            {
                await using ServerDbContext _db = new();

                Game? game = await _db.Game
                    .Where(item => item.Id == gameId)
                    .Include(item => item.HostUser)
                    .FirstOrDefaultAsync();

                User? user = await _db.User.FirstOrDefaultAsync(item => item.Id == userId);

                if (game is null || user is null)
                {
                    return null;
                }

                game.ClientUser = user;

                await _db.SaveChangesAsync();

                game.HostUser.IsReady = false;

                return game;
            }
            catch
            {
                throw;
            }
        }

        public static async Task<Game?> CreateGame(Game game, int userId)
        {
            try
            {
                await using ServerDbContext _db = new();

                foreach (Game g in _db.Game.Where(item => item.ClientUser == null))
                {
                    if (g.Name == game.Name)
                    {
                        return null;
                    }
                }

                User? user = await _db.User.FirstOrDefaultAsync(item => item.Id == userId);

                if (user is null)
                {
                    return null;
                }

                Game newGame = (await _db.Game.AddAsync(new()
                {
                    Name = game.Name,
                    IsPrivate = game.IsPrivate,
                    Password = game.Password,
                    HostUser = user
                })).Entity;

                await _db.SaveChangesAsync();

                return newGame;
            }
            catch
            {
                throw;
            }
        }

        public static async Task<string> Ready(Field field, int userId, int gameId)
        {
            try
            {
                await using ServerDbContext _db = new();

                int fieldId = 0;

                User? user = await _db.User.FirstOrDefaultAsync(item => item.Id == userId);

                Game? game = await _db.Game
                    .Where(item => item.Id == gameId)
                    .Include(item => item.HostUser)
                    .Include(item => item.ClientUser)
                    .Include(item => item.HostField)
                    .Include(item => item.ClientField)
                    .FirstOrDefaultAsync();

                if (user is null)
                {
                    return "Incorrect user ID";
                }

                if (game is null)
                {
                    return "Incorrect game ID";
                }

                if (user.IsReady)
                {
                    user.IsReady = false;

                    await _db.SaveChangesAsync();

                    return "SUCCESS";
                }

                if (CheckField(field) == false)
                {
                    return "Incorrect ships placement";
                }

                if (userId == game.HostUser.Id)
                {
                    if (game.HostField is null)
                    {
                        game.HostField = field;

                        fieldId = -1;
                    }
                    else
                    {
                        fieldId = game.HostField.Id;
                    }
                }
                else if (userId == game.ClientUser?.Id)
                {
                    if (game.ClientField is null)
                    {
                        game.ClientField = field;

                        fieldId = -1;
                    }
                    else
                    {
                        fieldId = game.ClientField.Id;
                    }
                }
                else
                {
                    return "Incorrect user ID";
                }

                if (fieldId != -1)
                {
                    List<Cell>? cells = await _db.Field
                                        .Where(item => item.Id == fieldId)
                                        .Select(item => item.Cells)
                                        .FirstOrDefaultAsync();

                    for (int i = 0; i < cells?.Count; i++)
                    {
                        cells[i].IsContainsShip = field.Cells[i].IsContainsShip;
                        cells[i].IsHit = field.Cells[i].IsHit;
                    }
                }

                user.IsReady = true;

                if (game.ClientUser is not null && game.HostUser.IsReady && game.ClientUser.IsReady)
                {
                    game.HostUser.IsTurn = true;
                    game.ClientUser.IsTurn = false;
                }

                await _db.SaveChangesAsync();

                return "SUCCESS";
            }
            catch
            {
                throw;
            }
        }
        public static async Task<Game?> EnemyWait(int gameId, int userId)
        {
            try
            {
                await using ServerDbContext _db = new();

                User? user = await _db.User.FirstOrDefaultAsync(item => item.Id == userId);

                Game? game = await _db.Game
                    .Where(item => item.Id == gameId)
                    .Include(item => item.HostUser)
                    .Include(item => item.ClientUser)
                    .Include(item => item.HostField)
                        .ThenInclude(item => item.Cells)
                    .Include(item => item.ClientField)
                        .ThenInclude(item => item.Cells)
                    .Include(item => item.Winner)
                    .FirstOrDefaultAsync();

                if (user is null || game is null)
                {
                    return null;
                }

                if (game.ClientUser is not null && game.HostUser.IsReady && game.ClientUser.IsReady)
                {
                    if (userId == game.HostUser.Id)
                    {
                        foreach (Cell cell in game.ClientField?.Cells!)
                        {
                            if (cell.IsHit == false)
                            {
                                cell.IsContainsShip = false;
                            }
                        }
                    }
                    else
                    {
                        foreach (Cell cell in game.HostField?.Cells!)
                        {
                            if (cell.IsHit == false)
                            {
                                cell.IsContainsShip = false;
                            }
                        }
                    }
                }

                if (game.HostField?.Cells.Count(item => item.IsContainsShip && item.IsHit) == 20)
                {
                    game.Winner = game.ClientUser;
                }

                if (game.ClientField?.Cells.Count(item => item.IsContainsShip && item.IsHit) == 20)
                {
                    game.Winner = game.HostUser;
                }

                return game;
            }
            catch
            {
                throw;
            }
        }

        public static async Task<string> Shoot(int fieldId, int gameId, int userId, int index)
        {
            try
            {
                await using ServerDbContext context = new();

                Field? field = await context.Field
                    .Where(item => item.Id == fieldId)
                    .Include(item => item.Cells)
                    .FirstOrDefaultAsync();

                Game? game = await context.Game
                    .Where(item => item.Id == gameId)
                    .Include(item => item.HostUser)
                    .Include(item => item.ClientUser)
                    .FirstOrDefaultAsync();

                User? user = await context.User.FirstOrDefaultAsync(item => item.Id == userId);

                if (field is null || game is null || user is null)
                {
                    return "field or game or user is null";
                }

                if (user.IsTurn == false)
                {
                    return "not your turn";
                }

                field.Cells = field.Cells.OrderBy(item => item.Id).ToList();

                field.Cells[index].IsHit = true;

                if (field.Cells[index].IsContainsShip)
                {
                    game.HostUser.IsTurn = user.Login == game.HostUser.Login;
                    game.ClientUser!.IsTurn = user.Login == game.ClientUser.Login;

                    List<int>? neighbours = [index];

                    foreach (int dir in new[] { -1, 1, -10, 10 })
                    {
                        int seek = dir;

                        while (index + seek >= 0
                            && index + seek < field.Cells.Count
                            && (dir != -1 || (index + seek + 1) % 10 != 0)
                            && (dir != 1 || (index + seek) % 10 != 0)
                            && field.Cells[index + seek].IsContainsShip)
                        {
                            if (field.Cells[index + seek].IsHit == false)
                            {
                                neighbours = null;
                                break;
                            }

                            neighbours?.Add(index + seek);
                            seek += dir;
                        }
                    }

                    if (neighbours is not null)
                    {
                        foreach (int seek in neighbours)
                        {
                            foreach (int dir in new[] { -11, -10, -9, -1, 1, 9, 10, 11 })
                            {
                                if (seek + dir >= 0
                                    && seek + dir < field.Cells.Count
                                    && ((dir != -11 && dir != -1 && dir != 9) || (seek + dir + 1) % 10 != 0)
                                    && ((dir != -9 && dir != 1 && dir != 11) || (seek + dir) % 10 != 0))
                                {
                                    field.Cells[seek + dir].IsHit = true;
                                }
                            }
                        }
                    }
                }
                else
                {
                    game.HostUser.IsTurn = user.Login != game.HostUser.Login;
                    game.ClientUser!.IsTurn = user.Login == game.HostUser.Login;
                }

                await context.SaveChangesAsync();

                return "SUCCESS";
            }
            catch
            {
                throw;
            }
        }

        //оптимизация? не, не слышал
        private static bool CheckField(Field field)
        {
            int[] shipCount = [4, 3, 2, 1];
            List<int> busyCells = [];
            var c = field.Cells;

            bool IsCellFree(int i)
            {
                foreach (int bc in busyCells)
                    if (i == bc)
                        return false;
                return true;
            }

            for(int i = 0; i < field.Cells.Count; i++)
            {
                if (c[i].IsContainsShip && IsCellFree(i))
                {
                    if (i == 0)
                    {
                        if (c[i + 1].IsContainsShip)
                        {
                            if (c[i + 2].IsContainsShip)
                            {
                                if (c[i + 3].IsContainsShip)
                                {
                                    if (c[i + 4].IsContainsShip)
                                        return false;
                                    else
                                    {
                                        shipCount[3]--;
                                        busyCells.Add(i);
                                        busyCells.Add(i + 1);
                                        busyCells.Add(i + 2);
                                        busyCells.Add(i + 3);
                                        busyCells.Add(i + 4);
                                        busyCells.Add(i + 10);
                                        busyCells.Add(i + 11);
                                        busyCells.Add(i + 12);
                                        busyCells.Add(i + 13);
                                        busyCells.Add(i + 14);
                                    }
                                    if (c[i + 13].IsContainsShip || c[i + 14].IsContainsShip)
                                        return false;
                                }
                                else
                                {
                                    shipCount[2]--;
                                    busyCells.Add(i);
                                    busyCells.Add(i + 1);
                                    busyCells.Add(i + 2);
                                    busyCells.Add(i + 3);
                                    busyCells.Add(i + 10);
                                    busyCells.Add(i + 11);
                                    busyCells.Add(i + 12);
                                    busyCells.Add(i + 13);
                                }
                                if (c[i + 12].IsContainsShip || c[i + 13].IsContainsShip)
                                    return false;
                            }
                            else
                            {
                                shipCount[1]--;
                                busyCells.Add(i);
                                busyCells.Add(i + 1);
                                busyCells.Add(i + 2);
                                busyCells.Add(i + 10);
                                busyCells.Add(i + 11);
                                busyCells.Add(i + 12);
                            }
                            if (c[i + 10].IsContainsShip || c[i + 11].IsContainsShip || c[i + 12].IsContainsShip)
                                return false;
                        }
                        else if (c[i + 10].IsContainsShip)
                        {
                            if (c[i + 20].IsContainsShip)
                            {
                                if (c[i + 30].IsContainsShip)
                                {
                                    if (c[i + 40].IsContainsShip)
                                        return false;
                                    else
                                    {
                                        shipCount[3]--;
                                        busyCells.Add(i);
                                        busyCells.Add(i + 1);
                                        busyCells.Add(i + 10);
                                        busyCells.Add(i + 11);
                                        busyCells.Add(i + 20);
                                        busyCells.Add(i + 21);
                                        busyCells.Add(i + 30);
                                        busyCells.Add(i + 31);
                                        busyCells.Add(i + 40);
                                        busyCells.Add(i + 41);
                                    }
                                    if (c[i + 31].IsContainsShip || c[i + 41].IsContainsShip)
                                        return false;
                                }
                                else
                                {
                                    shipCount[2]--;
                                    busyCells.Add(i);
                                    busyCells.Add(i + 1);
                                    busyCells.Add(i + 10);
                                    busyCells.Add(i + 11);
                                    busyCells.Add(i + 20);
                                    busyCells.Add(i + 21);
                                    busyCells.Add(i + 30);
                                    busyCells.Add(i + 31);
                                }
                                if (c[i + 21].IsContainsShip || c[i + 31].IsContainsShip)
                                    return false;
                            }
                            else
                            {
                                shipCount[1]--;
                                busyCells.Add(i);
                                busyCells.Add(i + 1);
                                busyCells.Add(i + 10);
                                busyCells.Add(i + 11);
                                busyCells.Add(i + 20);
                                busyCells.Add(i + 21);
                            }
                            if (c[i + 11].IsContainsShip || c[i + 21].IsContainsShip)
                                return false;
                        }
                        else
                        {
                            shipCount[0]--;
                            busyCells.Add(i);
                            busyCells.Add(i + 1);
                            busyCells.Add(i + 10);
                            busyCells.Add(i + 11);
                            if (c[i + 11].IsContainsShip)
                                return false;
                        }
                    }
                    else if (i == 9)
                    {
                        if (c[i - 1].IsContainsShip)
                        {
                            if (c[i - 2].IsContainsShip)
                            {
                                if (c[i - 3].IsContainsShip)
                                {
                                    if (c[i - 4].IsContainsShip)
                                        return false;
                                    else
                                    {
                                        shipCount[3]--;
                                        busyCells.Add(i);
                                        busyCells.Add(i - 1);
                                        busyCells.Add(i - 2);
                                        busyCells.Add(i - 3);
                                        busyCells.Add(i - 4);
                                        busyCells.Add(i + 6);
                                        busyCells.Add(i + 7);
                                        busyCells.Add(i + 8);
                                        busyCells.Add(i + 9);
                                        busyCells.Add(i + 10);
                                    }
                                    if (c[i + 7].IsContainsShip || c[i + 6].IsContainsShip)
                                        return false;
                                }
                                else
                                {
                                    shipCount[2]--;
                                    busyCells.Add(i);
                                    busyCells.Add(i - 1);
                                    busyCells.Add(i - 2);
                                    busyCells.Add(i - 3);
                                    busyCells.Add(i + 7);
                                    busyCells.Add(i + 8);
                                    busyCells.Add(i + 9);
                                    busyCells.Add(i + 10);
                                }
                                if (c[i + 8].IsContainsShip || c[i + 7].IsContainsShip)
                                    return false;
                            }
                            else
                            {
                                shipCount[1]--;
                                busyCells.Add(i);
                                busyCells.Add(i - 1);
                                busyCells.Add(i - 2);
                                busyCells.Add(i + 8);
                                busyCells.Add(i + 9);
                                busyCells.Add(i + 10);
                            }
                            if (c[i + 10].IsContainsShip || c[i + 9].IsContainsShip || c[i + 8].IsContainsShip)
                                return false;
                        }
                        else if (c[i + 10].IsContainsShip)
                        {
                            if (c[i + 20].IsContainsShip)
                            {
                                if (c[i + 30].IsContainsShip)
                                {
                                    if (c[i + 40].IsContainsShip)
                                        return false;
                                    else
                                    {
                                        shipCount[3]--;
                                        busyCells.Add(i);
                                        busyCells.Add(i - 1);
                                        busyCells.Add(i + 9);
                                        busyCells.Add(i + 10);
                                        busyCells.Add(i + 19);
                                        busyCells.Add(i + 20);
                                        busyCells.Add(i + 29);
                                        busyCells.Add(i + 30);
                                        busyCells.Add(i + 39);
                                        busyCells.Add(i + 40);
                                    }
                                    if (c[i + 29].IsContainsShip || c[i + 39].IsContainsShip)
                                        return false;
                                }
                                else
                                {
                                    shipCount[2]--;
                                    busyCells.Add(i);
                                    busyCells.Add(i - 1);
                                    busyCells.Add(i + 9);
                                    busyCells.Add(i + 10);
                                    busyCells.Add(i + 19);
                                    busyCells.Add(i + 20);
                                    busyCells.Add(i + 29);
                                    busyCells.Add(i + 30);
                                }
                                if (c[i + 19].IsContainsShip || c[i + 29].IsContainsShip)
                                    return false;
                            }
                            else
                            {
                                shipCount[1]--;
                                busyCells.Add(i);
                                busyCells.Add(i - 1);
                                busyCells.Add(i + 9);
                                busyCells.Add(i + 10);
                                busyCells.Add(i + 19);
                                busyCells.Add(i + 20);
                            }
                            if (c[i + 9].IsContainsShip || c[i + 19].IsContainsShip)
                                return false;
                        }
                        else
                        {
                            shipCount[0]--;
                            busyCells.Add(i);
                            busyCells.Add(i - 1);
                            busyCells.Add(i + 9);
                            busyCells.Add(i + 10);
                            if (c[i + 9].IsContainsShip)
                                return false;
                        }
                    }
                    else if (i == 90)
                    {
                        if (c[i + 1].IsContainsShip)
                        {
                            if (c[i + 2].IsContainsShip)
                            {
                                if (c[i + 3].IsContainsShip)
                                {
                                    if (c[i + 4].IsContainsShip)
                                        return false;
                                    else
                                    {
                                        shipCount[3]--;
                                        busyCells.Add(i);
                                        busyCells.Add(i + 1);
                                        busyCells.Add(i + 2);
                                        busyCells.Add(i + 3);
                                        busyCells.Add(i + 4);
                                        busyCells.Add(i - 6);
                                        busyCells.Add(i - 7);
                                        busyCells.Add(i - 8);
                                        busyCells.Add(i - 9);
                                        busyCells.Add(i - 10);
                                    }
                                    if (c[i - 7].IsContainsShip || c[i - 6].IsContainsShip)
                                        return false;
                                }
                                else
                                {
                                    shipCount[2]--;
                                    busyCells.Add(i);
                                    busyCells.Add(i + 1);
                                    busyCells.Add(i + 2);
                                    busyCells.Add(i + 3);
                                    busyCells.Add(i - 7);
                                    busyCells.Add(i - 8);
                                    busyCells.Add(i - 9);
                                    busyCells.Add(i - 10);
                                }
                                if (c[i - 8].IsContainsShip || c[i - 7].IsContainsShip)
                                    return false;
                            }
                            else
                            {
                                shipCount[1]--;
                                busyCells.Add(i);
                                busyCells.Add(i + 1);
                                busyCells.Add(i + 2);
                                busyCells.Add(i - 8);
                                busyCells.Add(i - 9);
                                busyCells.Add(i - 10);
                            }
                            if (c[i - 10].IsContainsShip || c[i - 9].IsContainsShip || c[i - 8].IsContainsShip)
                                return false;
                        }
                        else if (c[i - 10].IsContainsShip)
                        {
                            if (c[i - 20].IsContainsShip)
                            {
                                if (c[i - 30].IsContainsShip)
                                {
                                    if (c[i - 40].IsContainsShip)
                                        return false;
                                    else
                                    {
                                        shipCount[3]--;
                                        busyCells.Add(i);
                                        busyCells.Add(i + 1);
                                        busyCells.Add(i - 9);
                                        busyCells.Add(i - 10);
                                        busyCells.Add(i - 19);
                                        busyCells.Add(i - 20);
                                        busyCells.Add(i - 29);
                                        busyCells.Add(i - 30);
                                        busyCells.Add(i - 39);
                                        busyCells.Add(i - 40);
                                    }
                                    if (c[i - 29].IsContainsShip || c[i - 39].IsContainsShip)
                                        return false;
                                }
                                else
                                {
                                    shipCount[2]--;
                                    busyCells.Add(i);
                                    busyCells.Add(i + 1);
                                    busyCells.Add(i - 9);
                                    busyCells.Add(i - 10);
                                    busyCells.Add(i - 19);
                                    busyCells.Add(i - 20);
                                    busyCells.Add(i - 29);
                                    busyCells.Add(i - 30);
                                }
                                if (c[i - 19].IsContainsShip || c[i - 29].IsContainsShip)
                                    return false;
                            }
                            else
                            {
                                shipCount[1]--;
                                busyCells.Add(i);
                                busyCells.Add(i + 1);
                                busyCells.Add(i - 9);
                                busyCells.Add(i - 10);
                                busyCells.Add(i - 19);
                                busyCells.Add(i - 20);
                            }
                            if (c[i - 9].IsContainsShip || c[i - 19].IsContainsShip)
                                return false;
                        }
                        else
                        {
                            shipCount[0]--;
                            busyCells.Add(i);
                            busyCells.Add(i + 1);
                            busyCells.Add(i - 9);
                            busyCells.Add(i - 10);
                            if (c[i - 9].IsContainsShip)
                                return false;
                        }
                    }
                    else if (i == 99)
                    {
                        if (c[i - 1].IsContainsShip)
                        {
                            if (c[i - 2].IsContainsShip)
                            {
                                if (c[i - 3].IsContainsShip)
                                {
                                    if (c[i - 4].IsContainsShip)
                                        return false;
                                    else
                                    {
                                        shipCount[3]--;
                                        busyCells.Add(i);
                                        busyCells.Add(i - 1);
                                        busyCells.Add(i - 2);
                                        busyCells.Add(i - 3);
                                        busyCells.Add(i - 4);
                                        busyCells.Add(i - 10);
                                        busyCells.Add(i - 11);
                                        busyCells.Add(i - 12);
                                        busyCells.Add(i - 13);
                                        busyCells.Add(i - 14);

                                    }
                                    if (c[i - 13].IsContainsShip || c[i - 14].IsContainsShip)
                                        return false;
                                }
                                else
                                {
                                    shipCount[2]--;
                                    busyCells.Add(i);
                                    busyCells.Add(i - 1);
                                    busyCells.Add(i - 2);
                                    busyCells.Add(i - 3);
                                    busyCells.Add(i - 10);
                                    busyCells.Add(i - 11);
                                    busyCells.Add(i - 12);
                                    busyCells.Add(i - 13);
                                }
                                if (c[i - 12].IsContainsShip || c[i - 13].IsContainsShip)
                                    return false;
                            }
                            else
                            {
                                shipCount[1]--;
                                busyCells.Add(i);
                                busyCells.Add(i - 1);
                                busyCells.Add(i - 2);
                                busyCells.Add(i - 10);
                                busyCells.Add(i - 11);
                                busyCells.Add(i - 12);
                            }
                            if (c[i - 10].IsContainsShip || c[i - 11].IsContainsShip || c[i - 12].IsContainsShip)
                                return false;
                        }
                        else if (c[i - 10].IsContainsShip)
                        {
                            if (c[i - 20].IsContainsShip)
                            {
                                if (c[i - 30].IsContainsShip)
                                {
                                    if (c[i - 40].IsContainsShip)
                                        return false;
                                    else
                                    {
                                        shipCount[3]--;
                                        busyCells.Add(i);
                                        busyCells.Add(i - 1);
                                        busyCells.Add(i - 10);
                                        busyCells.Add(i - 11);
                                        busyCells.Add(i - 20);
                                        busyCells.Add(i - 21);
                                        busyCells.Add(i - 30);
                                        busyCells.Add(i - 31);
                                        busyCells.Add(i - 40);
                                        busyCells.Add(i - 41);
                                    }
                                    if (c[i - 31].IsContainsShip || c[i - 41].IsContainsShip)
                                        return false;
                                }
                                else
                                {
                                    shipCount[2]--;
                                    busyCells.Add(i);
                                    busyCells.Add(i - 1);
                                    busyCells.Add(i - 10);
                                    busyCells.Add(i - 11);
                                    busyCells.Add(i - 20);
                                    busyCells.Add(i - 21);
                                    busyCells.Add(i - 30);
                                    busyCells.Add(i - 31);
                                }
                                if (c[i - 21].IsContainsShip || c[i - 31].IsContainsShip)
                                    return false;
                            }
                            else
                            {
                                shipCount[1]--;
                                busyCells.Add(i);
                                busyCells.Add(i - 1);
                                busyCells.Add(i - 10);
                                busyCells.Add(i - 11);
                                busyCells.Add(i - 20);
                                busyCells.Add(i - 21);
                            }
                            if (c[i - 11].IsContainsShip || c[i - 21].IsContainsShip)
                                return false;
                        }
                        else
                        {
                            shipCount[0]--;
                            busyCells.Add(i);
                            busyCells.Add(i - 1);
                            busyCells.Add(i - 10);
                            busyCells.Add(i - 11);
                            if (c[i - 11].IsContainsShip)
                                return false;
                        }
                    }
                }
            }

            for (int i = 1; i < field.Cells.Count - 1; i++)
            {
                if (c[i].IsContainsShip && IsCellFree(i))
                {
                    if (i >= 1 && i <= 8)
                    {
                        if (c[i + 1].IsContainsShip)
                        {
                            if (c[i + 2].IsContainsShip)
                            {
                                if (c[i + 3].IsContainsShip)
                                {
                                    if (c[i + 4].IsContainsShip)
                                        return false;
                                    else
                                    {
                                        shipCount[3]--;
                                        busyCells.Add(i);
                                        busyCells.Add(i - 1);
                                        busyCells.Add(i + 1);
                                        busyCells.Add(i + 2);
                                        busyCells.Add(i + 3);
                                        busyCells.Add(i + 4);
                                        busyCells.Add(i + 9);
                                        busyCells.Add(i + 10);
                                        busyCells.Add(i + 11);
                                        busyCells.Add(i + 12);
                                        busyCells.Add(i + 13);
                                        busyCells.Add(i + 14);
                                    }
                                    if (c[i + 13].IsContainsShip || c[i + 14].IsContainsShip)
                                        return false;
                                }
                                else
                                {
                                    shipCount[2]--;
                                    busyCells.Add(i);
                                    busyCells.Add(i - 1);
                                    busyCells.Add(i + 1);
                                    busyCells.Add(i + 2);
                                    busyCells.Add(i + 3);
                                    busyCells.Add(i + 9);
                                    busyCells.Add(i + 10);
                                    busyCells.Add(i + 11);
                                    busyCells.Add(i + 12);
                                    busyCells.Add(i + 13);
                                }
                                if (c[i + 12].IsContainsShip || c[i + 13].IsContainsShip)
                                    return false;
                            }
                            else
                            {
                                shipCount[1]--;
                                busyCells.Add(i);
                                busyCells.Add(i - 1);
                                busyCells.Add(i + 1);
                                busyCells.Add(i + 2);
                                busyCells.Add(i + 9);
                                busyCells.Add(i + 10);
                                busyCells.Add(i + 11);
                                busyCells.Add(i + 12);
                            }
                            if (c[i + 9].IsContainsShip || c[i + 10].IsContainsShip || c[i + 11].IsContainsShip || c[i + 12].IsContainsShip)
                                return false;
                        }
                        else if (c[i + 10].IsContainsShip)
                        {
                            if (c[i + 20].IsContainsShip)
                            {
                                if (c[i + 30].IsContainsShip)
                                {
                                    if (c[i + 40].IsContainsShip)
                                        return false;
                                    else
                                    {
                                        shipCount[3]--;
                                        busyCells.Add(i);
                                        busyCells.Add(i - 1);
                                        busyCells.Add(i + 1);
                                        busyCells.Add(i + 9);
                                        busyCells.Add(i + 10);
                                        busyCells.Add(i + 11);
                                        busyCells.Add(i + 19);
                                        busyCells.Add(i + 20);
                                        busyCells.Add(i + 21);
                                        busyCells.Add(i + 29);
                                        busyCells.Add(i + 30);
                                        busyCells.Add(i + 31);
                                        busyCells.Add(i + 39);
                                        busyCells.Add(i + 40);
                                        busyCells.Add(i + 41);
                                    }
                                    if (c[i + 29].IsContainsShip || c[i + 31].IsContainsShip || c[i + 39].IsContainsShip || c[i + 41].IsContainsShip)
                                        return false;
                                }
                                else
                                {
                                    shipCount[2]--;
                                    busyCells.Add(i);
                                    busyCells.Add(i - 1);
                                    busyCells.Add(i + 1);
                                    busyCells.Add(i + 9);
                                    busyCells.Add(i + 10);
                                    busyCells.Add(i + 11);
                                    busyCells.Add(i + 19);
                                    busyCells.Add(i + 20);
                                    busyCells.Add(i + 21);
                                    busyCells.Add(i + 29);
                                    busyCells.Add(i + 30);
                                    busyCells.Add(i + 31);
                                }
                                if (c[i + 19].IsContainsShip || c[i + 21].IsContainsShip || c[i + 29].IsContainsShip || c[i + 31].IsContainsShip)
                                    return false;
                            }
                            else
                            {
                                shipCount[1]--;
                                busyCells.Add(i);
                                busyCells.Add(i - 1);
                                busyCells.Add(i + 1);
                                busyCells.Add(i + 9);
                                busyCells.Add(i + 10);
                                busyCells.Add(i + 11);
                                busyCells.Add(i + 19);
                                busyCells.Add(i + 20);
                                busyCells.Add(i + 21);
                            }
                            if (c[i + 9].IsContainsShip || c[i + 11].IsContainsShip || c[i + 19].IsContainsShip || c[i + 21].IsContainsShip)
                                return false;
                        }
                        else
                        {
                            shipCount[0]--;
                            busyCells.Add(i);
                            busyCells.Add(i - 1);
                            busyCells.Add(i + 1);
                            busyCells.Add(i + 9);
                            busyCells.Add(i + 10);
                            busyCells.Add(i + 11);
                            if (c[i + 9].IsContainsShip || c[i + 11].IsContainsShip)
                                return false;
                        }
                    }
                    else if (i >= 91 && i <= 98)
                    {
                        if (c[i + 1].IsContainsShip)
                        {
                            if (c[i + 2].IsContainsShip)
                            {
                                if (c[i + 3].IsContainsShip)
                                {
                                    if (c[i + 4].IsContainsShip)
                                        return false;
                                    else
                                    {
                                        shipCount[3]--;
                                        busyCells.Add(i);
                                        busyCells.Add(i - 1);
                                        busyCells.Add(i + 1);
                                        busyCells.Add(i + 2);
                                        busyCells.Add(i + 3);
                                        busyCells.Add(i + 4);
                                        busyCells.Add(i - 6);
                                        busyCells.Add(i - 7);
                                        busyCells.Add(i - 8);
                                        busyCells.Add(i - 9);
                                        busyCells.Add(i - 10);
                                        busyCells.Add(i - 11);
                                    }
                                    if (c[i - 7].IsContainsShip || c[i - 6].IsContainsShip)
                                        return false;
                                }
                                else
                                {
                                    shipCount[2]--;
                                    busyCells.Add(i);
                                    busyCells.Add(i - 1);
                                    busyCells.Add(i + 1);
                                    busyCells.Add(i + 2);
                                    busyCells.Add(i + 3);
                                    busyCells.Add(i - 7);
                                    busyCells.Add(i - 8);
                                    busyCells.Add(i - 9);
                                    busyCells.Add(i - 10);
                                    busyCells.Add(i - 11);
                                }
                                if (c[i - 8].IsContainsShip || c[i - 7].IsContainsShip)
                                    return false;
                            }
                            else
                            {
                                shipCount[1]--;
                                busyCells.Add(i);
                                busyCells.Add(i - 1);
                                busyCells.Add(i + 1);
                                busyCells.Add(i + 2);
                                busyCells.Add(i - 8);
                                busyCells.Add(i - 9);
                                busyCells.Add(i - 10);
                                busyCells.Add(i - 11);
                            }
                            if (c[i - 11].IsContainsShip || c[i - 10].IsContainsShip || c[i - 9].IsContainsShip || c[i - 8].IsContainsShip)
                                return false;
                        }
                        else if (c[i - 10].IsContainsShip)
                        {
                            if (c[i - 20].IsContainsShip)
                            {
                                if (c[i - 30].IsContainsShip)
                                {
                                    if (c[i - 40].IsContainsShip)
                                        return false;
                                    else
                                    {
                                        shipCount[3]--;
                                        busyCells.Add(i);
                                        busyCells.Add(i - 1);
                                        busyCells.Add(i + 1);
                                        busyCells.Add(i - 9);
                                        busyCells.Add(i - 10);
                                        busyCells.Add(i - 11);
                                        busyCells.Add(i - 19);
                                        busyCells.Add(i - 20);
                                        busyCells.Add(i - 21);
                                        busyCells.Add(i - 29);
                                        busyCells.Add(i - 30);
                                        busyCells.Add(i - 39);
                                        busyCells.Add(i - 40);
                                        busyCells.Add(i - 41);
                                    }
                                    if (c[i - 39].IsContainsShip || c[i - 41].IsContainsShip)
                                        return false;
                                }
                                else
                                {
                                    shipCount[2]--;
                                    busyCells.Add(i);
                                    busyCells.Add(i - 1);
                                    busyCells.Add(i + 1);
                                    busyCells.Add(i - 9);
                                    busyCells.Add(i - 10);
                                    busyCells.Add(i - 11);
                                    busyCells.Add(i - 19);
                                    busyCells.Add(i - 20);
                                    busyCells.Add(i - 21);
                                    busyCells.Add(i - 29);
                                    busyCells.Add(i - 30);
                                    busyCells.Add(i - 31);
                                }
                                if (c[i - 29].IsContainsShip || c[i - 31].IsContainsShip)
                                    return false;
                            }
                            else
                            {
                                shipCount[1]--;
                                busyCells.Add(i);
                                busyCells.Add(i - 1);
                                busyCells.Add(i + 1);
                                busyCells.Add(i - 9);
                                busyCells.Add(i - 10);
                                busyCells.Add(i - 11);
                                busyCells.Add(i - 19);
                                busyCells.Add(i - 20);
                                busyCells.Add(i - 21);
                            }
                            if (c[i - 9].IsContainsShip || c[i - 11].IsContainsShip || c[i - 19].IsContainsShip || c[i - 21].IsContainsShip)
                                return false;
                        }
                        else
                        {
                            shipCount[0]--;
                            busyCells.Add(i);
                            busyCells.Add(i - 1);
                            busyCells.Add(i + 1);
                            busyCells.Add(i - 9);
                            busyCells.Add(i - 10);
                            busyCells.Add(i - 11);
                            if (c[i - 9].IsContainsShip || c[i - 11].IsContainsShip)
                                return false;
                        }
                    }
                    else if (i % 10 == 0)
                    {
                        if (c[i + 1].IsContainsShip)
                        {
                            if (c[i + 2].IsContainsShip)
                            {
                                if (c[i + 3].IsContainsShip)
                                {
                                    if (c[i + 4].IsContainsShip)
                                        return false;
                                    else
                                    {
                                        shipCount[3]--;
                                        busyCells.Add(i);
                                        busyCells.Add(i + 1);
                                        busyCells.Add(i + 2);
                                        busyCells.Add(i + 3);
                                        busyCells.Add(i + 4);
                                        busyCells.Add(i + 10);
                                        busyCells.Add(i + 11);
                                        busyCells.Add(i + 12);
                                        busyCells.Add(i + 13);
                                        busyCells.Add(i + 14);
                                    }
                                    if (c[i + 13].IsContainsShip || c[i + 14].IsContainsShip)
                                        return false;
                                }
                                else
                                {
                                    shipCount[2]--;
                                    busyCells.Add(i);
                                    busyCells.Add(i + 1);
                                    busyCells.Add(i + 2);
                                    busyCells.Add(i + 3);
                                    busyCells.Add(i + 10);
                                    busyCells.Add(i + 11);
                                    busyCells.Add(i + 12);
                                    busyCells.Add(i + 13);
                                }
                                if (c[i + 12].IsContainsShip || c[i + 13].IsContainsShip)
                                    return false;
                            }
                            else
                            {
                                shipCount[1]--;
                                busyCells.Add(i);
                                busyCells.Add(i + 1);
                                busyCells.Add(i + 2);
                                busyCells.Add(i + 10);
                                busyCells.Add(i + 11);
                                busyCells.Add(i + 12);
                            }
                            if (c[i + 10].IsContainsShip || c[i + 11].IsContainsShip || c[i + 12].IsContainsShip)
                                return false;
                        }
                        else if (c[i + 10].IsContainsShip)
                        {
                            if (c[i + 20].IsContainsShip)
                            {
                                if (c[i + 30].IsContainsShip)
                                {
                                    if (c[i + 40].IsContainsShip)
                                        return false;
                                    else
                                    {
                                        shipCount[3]--;
                                        busyCells.Add(i);
                                        busyCells.Add(i + 1);
                                        busyCells.Add(i + 10);
                                        busyCells.Add(i + 11);
                                        busyCells.Add(i + 20);
                                        busyCells.Add(i + 21);
                                        busyCells.Add(i + 30);
                                        busyCells.Add(i + 31);
                                        busyCells.Add(i + 40);
                                        busyCells.Add(i + 41);
                                    }
                                    if (c[i + 31].IsContainsShip || c[i + 41].IsContainsShip)
                                        return false;
                                }
                                else
                                {
                                    shipCount[2]--;
                                    busyCells.Add(i);
                                    busyCells.Add(i + 1);
                                    busyCells.Add(i + 10);
                                    busyCells.Add(i + 11);
                                    busyCells.Add(i + 20);
                                    busyCells.Add(i + 21);
                                    busyCells.Add(i + 30);
                                    busyCells.Add(i + 31);
                                }
                                if (c[i + 21].IsContainsShip || c[i + 31].IsContainsShip)
                                    return false;
                            }
                            else
                            {
                                shipCount[1]--;
                                busyCells.Add(i);
                                busyCells.Add(i + 1);
                                busyCells.Add(i + 10);
                                busyCells.Add(i + 11);
                                busyCells.Add(i + 20);
                                busyCells.Add(i + 21);
                            }
                            if (c[i + 11].IsContainsShip || c[i + 21].IsContainsShip)
                                return false;
                        }
                        else
                        {
                            shipCount[0]--;
                            busyCells.Add(i);
                            busyCells.Add(i + 1);
                            busyCells.Add(i + 10);
                            busyCells.Add(i + 11);
                            if (c[i + 11].IsContainsShip)
                                return false;
                        }
                    }
                    else if (i % 10 == 9)
                    {
                        if (c[i - 1].IsContainsShip)
                        {
                            if (c[i - 2].IsContainsShip)
                            {
                                if (c[i - 3].IsContainsShip)
                                {
                                    if (c[i - 4].IsContainsShip)
                                        return false;
                                    else
                                    {
                                        shipCount[3]--;
                                        busyCells.Add(i);
                                        busyCells.Add(i - 1);
                                        busyCells.Add(i - 2);
                                        busyCells.Add(i - 3);
                                        busyCells.Add(i - 4);
                                        busyCells.Add(i + 6);
                                        busyCells.Add(i + 7);
                                        busyCells.Add(i + 8);
                                        busyCells.Add(i + 9);
                                        busyCells.Add(i + 10);
                                    }
                                    if (c[i + 7].IsContainsShip || c[i + 6].IsContainsShip)
                                        return false;
                                }
                                else
                                {
                                    shipCount[2]--;
                                    busyCells.Add(i);
                                    busyCells.Add(i - 1);
                                    busyCells.Add(i - 2);
                                    busyCells.Add(i - 3);
                                    busyCells.Add(i + 7);
                                    busyCells.Add(i + 8);
                                    busyCells.Add(i + 9);
                                    busyCells.Add(i + 10);
                                }
                                if (c[i + 8].IsContainsShip || c[i + 7].IsContainsShip)
                                    return false;
                            }
                            else
                            {
                                shipCount[1]--;
                                busyCells.Add(i);
                                busyCells.Add(i - 1);
                                busyCells.Add(i - 2);
                                busyCells.Add(i + 8);
                                busyCells.Add(i + 9);
                                busyCells.Add(i + 10);
                            }
                            if (c[i + 10].IsContainsShip || c[i + 9].IsContainsShip || c[i + 8].IsContainsShip)
                                return false;
                        }
                        else if (c[i + 10].IsContainsShip)
                        {
                            if (c[i + 20].IsContainsShip)
                            {
                                if (c[i + 30].IsContainsShip)
                                {
                                    if (c[i + 40].IsContainsShip)
                                        return false;
                                    else
                                    {
                                        shipCount[3]--;
                                        busyCells.Add(i);
                                        busyCells.Add(i - 1);
                                        busyCells.Add(i + 9);
                                        busyCells.Add(i + 10);
                                        busyCells.Add(i + 19);
                                        busyCells.Add(i + 20);
                                        busyCells.Add(i + 29);
                                        busyCells.Add(i + 30);
                                        busyCells.Add(i + 39);
                                        busyCells.Add(i + 40);
                                    }
                                    if (c[i + 29].IsContainsShip || c[i + 39].IsContainsShip)
                                        return false;
                                }
                                else
                                {
                                    shipCount[2]--;
                                    busyCells.Add(i);
                                    busyCells.Add(i - 1);
                                    busyCells.Add(i + 9);
                                    busyCells.Add(i + 10);
                                    busyCells.Add(i + 19);
                                    busyCells.Add(i + 20);
                                    busyCells.Add(i + 29);
                                    busyCells.Add(i + 30);
                                }
                                if (c[i + 19].IsContainsShip || c[i + 29].IsContainsShip)
                                    return false;
                            }
                            else
                            {
                                shipCount[1]--;
                                busyCells.Add(i);
                                busyCells.Add(i - 1);
                                busyCells.Add(i + 9);
                                busyCells.Add(i + 10);
                                busyCells.Add(i + 19);
                                busyCells.Add(i + 20);
                            }
                            if (c[i + 9].IsContainsShip || c[i + 19].IsContainsShip)
                                return false;
                        }
                        else
                        {
                            shipCount[0]--;
                            busyCells.Add(i);
                            busyCells.Add(i - 1);
                            busyCells.Add(i + 9);
                            busyCells.Add(i + 10);
                            if (c[i + 9].IsContainsShip)
                                return false;
                        }
                    }
                }
            }

            for (int i = 0; i < field.Cells.Count; i++)
            {
                if (c[i].IsContainsShip && IsCellFree(i))
                {
                    if (c[i + 1].IsContainsShip)
                    {
                        if (c[i + 2].IsContainsShip)
                        {
                            if (c[i + 3].IsContainsShip)
                            {
                                if (c[i + 4].IsContainsShip)
                                    return false;
                                else
                                {
                                    shipCount[3]--;
                                    busyCells.Add(i);
                                    busyCells.Add(i - 11);
                                    busyCells.Add(i - 10);
                                    busyCells.Add(i - 9);
                                    busyCells.Add(i - 8);
                                    busyCells.Add(i - 7);
                                    busyCells.Add(i - 6);
                                    busyCells.Add(i - 1);
                                    busyCells.Add(i + 1);
                                    busyCells.Add(i + 2);
                                    busyCells.Add(i + 3);
                                    busyCells.Add(i + 4);
                                    busyCells.Add(i + 9);
                                    busyCells.Add(i + 10);
                                    busyCells.Add(i + 11);
                                    busyCells.Add(i + 12);
                                    busyCells.Add(i + 13);
                                    busyCells.Add(i + 14);
                                }
                                if (c[i + 14].IsContainsShip || c[i - 6].IsContainsShip)
                                    return false;
                            }
                            else
                            {
                                shipCount[2]--;
                                busyCells.Add(i);
                                busyCells.Add(i - 11);
                                busyCells.Add(i - 10);
                                busyCells.Add(i - 9);
                                busyCells.Add(i - 8);
                                busyCells.Add(i - 7);
                                busyCells.Add(i - 1);
                                busyCells.Add(i + 1);
                                busyCells.Add(i + 2);
                                busyCells.Add(i + 3);
                                busyCells.Add(i + 9);
                                busyCells.Add(i + 10);
                                busyCells.Add(i + 11);
                                busyCells.Add(i + 12);
                                busyCells.Add(i + 13);
                            }
                            if (c[i + 13].IsContainsShip || c[i - 7].IsContainsShip)
                                return false;
                        }
                        else
                        {
                            shipCount[1]--;
                            busyCells.Add(i);
                            busyCells.Add(i - 11);
                            busyCells.Add(i - 10);
                            busyCells.Add(i - 9);
                            busyCells.Add(i - 8);
                            busyCells.Add(i - 1);
                            busyCells.Add(i + 1);
                            busyCells.Add(i + 2);
                            busyCells.Add(i + 9);
                            busyCells.Add(i + 10);
                            busyCells.Add(i + 11);
                            busyCells.Add(i + 12);
                        }
                        if (c[i + 9].IsContainsShip || c[i + 10].IsContainsShip || c[i + 11].IsContainsShip || c[i + 12].IsContainsShip
                            || c[i - 11].IsContainsShip || c[i - 10].IsContainsShip || c[i - 9].IsContainsShip || c[i - 8].IsContainsShip)
                            return false;
                    }
                    else if (c[i + 10].IsContainsShip)
                    {
                        if (c[i + 20].IsContainsShip)
                        {
                            if (c[i + 30].IsContainsShip)
                            {
                                if (c[i + 40].IsContainsShip)
                                    return false;
                                else
                                {
                                    shipCount[3]--;
                                    busyCells.Add(i);
                                    busyCells.Add(i - 11);
                                    busyCells.Add(i - 10);
                                    busyCells.Add(i - 9);
                                    busyCells.Add(i - 1);
                                    busyCells.Add(i + 1);
                                    busyCells.Add(i + 9);
                                    busyCells.Add(i + 10);
                                    busyCells.Add(i + 11);
                                    busyCells.Add(i + 19);
                                    busyCells.Add(i + 20);
                                    busyCells.Add(i + 21);
                                    busyCells.Add(i + 29);
                                    busyCells.Add(i + 30);
                                    busyCells.Add(i + 31);
                                    busyCells.Add(i + 39);
                                    busyCells.Add(i + 40);
                                    busyCells.Add(i + 41);
                                }
                                if (c[i + 39].IsContainsShip || c[i + 41].IsContainsShip)
                                    return false;
                            }
                            else
                            {
                                shipCount[2]--;
                                busyCells.Add(i);
                                busyCells.Add(i - 11);
                                busyCells.Add(i - 10);
                                busyCells.Add(i - 9);
                                busyCells.Add(i - 1);
                                busyCells.Add(i + 1);
                                busyCells.Add(i + 9);
                                busyCells.Add(i + 10);
                                busyCells.Add(i + 11);
                                busyCells.Add(i + 19);
                                busyCells.Add(i + 20);
                                busyCells.Add(i + 21);
                                busyCells.Add(i + 29);
                                busyCells.Add(i + 30);
                                busyCells.Add(i + 31);
                            }
                            if (c[i + 29].IsContainsShip || c[i + 31].IsContainsShip)
                                return false;
                        }
                        else
                        {
                            shipCount[1]--;
                            busyCells.Add(i);
                            busyCells.Add(i - 11);
                            busyCells.Add(i - 10);
                            busyCells.Add(i - 9);
                            busyCells.Add(i - 1);
                            busyCells.Add(i + 1);
                            busyCells.Add(i + 9);
                            busyCells.Add(i + 10);
                            busyCells.Add(i + 11);
                            busyCells.Add(i + 19);
                            busyCells.Add(i + 20);
                            busyCells.Add(i + 21);
                        }
                        if (c[i + 9].IsContainsShip || c[i + 11].IsContainsShip || c[i + 19].IsContainsShip || c[i + 21].IsContainsShip
                            || c[i - 9].IsContainsShip || c[i - 10].IsContainsShip || c[i - 11].IsContainsShip)
                            return false;
                    }
                    else
                    {
                        shipCount[0]--;
                        busyCells.Add(i);
                        busyCells.Add(i - 11);
                        busyCells.Add(i - 10);
                        busyCells.Add(i - 9);
                        busyCells.Add(i - 1);
                        busyCells.Add(i + 1);
                        busyCells.Add(i + 9);
                        busyCells.Add(i + 10);
                        busyCells.Add(i + 11);
                        if (c[i + 9].IsContainsShip || c[i + 11].IsContainsShip
                            || c[i - 9].IsContainsShip || c[i - 10].IsContainsShip || c[i - 11].IsContainsShip)
                            return false;
                    }
                }

                for (int j = 0; j < shipCount.Length; j++)
                    if (shipCount[j] < 0)
                        return false;
            }

            for (int j = 0; j < shipCount.Length; j++)
                if (shipCount[j] != 0)
                    return false;

            return true;
        }
    }
}
