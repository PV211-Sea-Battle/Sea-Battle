using Microsoft.EntityFrameworkCore;
using Models;

namespace Server
{
    public class DbServer
    {
        private ServerDbContext _db = null!;
        private int shipsDestroyed = 0;
        public void ShowFirst5RowsOfEveryTable()
        {
            try { _db = new ServerDbContext(); }
            catch (Exception ex) { Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Runtime database-releated error: " + ex.Message); return; }
            var users = _db.User.OrderByDescending(u => u.Id).ToList().Take(5);

            Console.WriteLine("\nUsers:\n");
            foreach (var user in users)
            {
                Console.WriteLine($"Id: {user.Id} | Login: {user.Login} | Password: {user.Password}");
            }

            var games = _db.Game
                .Include(item => item.HostUser)
                .Include(item => item.ClientUser)
                .OrderByDescending(g => g.Id).ToList().Take(5);
            Console.WriteLine("\nGames:\n");
            foreach (var game in games)
            {
                Console.WriteLine($"Id: {game.Id} | Name: {game.Name} | IsPrivate?: {game.IsPrivate}" +
                    $"Password: {game.Password} | HostUserId: {game.HostUser.Id} | ClientUserId: {game.ClientUser.Id}");
            }

            var fields = _db.Field.OrderByDescending(f => f.Id).ToList().Take(5);
            Console.WriteLine("\nFields:\n");
            foreach (var field in fields)
            {
                Console.WriteLine($"Id: {field.Id}");
            }

            var cells = _db.Cell.Include(item => item.Field).OrderByDescending(c => c.Id).ToList().Take(5);
            Console.WriteLine("\nCells:\n");
            foreach (var cell in cells)
            {
                Console.WriteLine($"Id: {cell.Id} | FieldId: {cell.Field.Id} | " +
                    $"IsContainsShip?: {cell.IsContainsShip} | IsHit?: {cell.IsHit}");
            }
        }

        public bool RegisterUser(string login, string pass)
        {
            bool check = true;
            try { _db = new ServerDbContext(); }
            catch (Exception ex) { Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Runtime database-releated error: " + ex.Message); return false; }
            var users = _db.User.ToList();

            foreach (var user in users)
                if (user.Login == login)
                    check = false;

            if (check)
            {
                try
                {
                    _db.User.Add(new()
                    {
                        Login = login,
                        Password = pass
                    });
                    _db.SaveChanges();
                }
                catch (Exception ex) { Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Runtime error: " + ex.Message); return false; }
                return true;
            }
            return false;
        }
        public User SignUp(string login, string pass)
        {
            User res = null!;
            try { _db = new ServerDbContext(); }
            catch (Exception ex) { Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Runtime database-releated error: " + ex.Message); return res; }
            
            var users = _db.User.ToList();

            foreach (var user in users)
            {
                if (user.Login == login && user.Password == pass)
                {
                    res = user;
                }
            }
            return res;
        }

        public List<Game> GetGameList()
        {
            try { _db = new ServerDbContext(); }
            catch (Exception ex) { Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Runtime database-releated error: " + ex.Message); return null!; }
            var games = _db.Game
                .Include(item => item.HostUser)
                .Include(item => item.ClientUser)
                .Where(item => item.ClientUser == null)
                .ToList();
            return games;
        }

        public Game GetGame(int gameId)
        {
            try { _db = new ServerDbContext(); }
            catch (Exception ex) { Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Runtime database-releated error: " + ex.Message); return null!; }
            Game? game = (from g in _db.Game.Include(item => item.ClientUser)
                          where g.Id == gameId
                          select g).ToList().FirstOrDefault();
            return game??null!;
        }

        public Game JoinGame(int gameId, int userId)
        {
            try { _db = new ServerDbContext(); }
            catch (Exception ex) { Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Runtime database-releated error: " + ex.Message); return null!; }
            Game? game = (from g in _db.Game
                          .Include(item => item.HostUser)
                       where g.Id == gameId
                       select g).ToList().FirstOrDefault();
            User? user = (from u in _db.User
                          where u.Id == userId
                          select u).ToList().FirstOrDefault();
            if (game is null || user is null)
                return null!;
            game.ClientUser = user;
            _db.SaveChanges();

            game.HostUser.IsReady = false;
            return game;
        }

        public Game CreateGame(Game game, int userId)
        {
            try { _db = new ServerDbContext(); }
            catch (Exception ex) { Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Runtime database-releated error: " + ex.Message); return null!; }
            bool check = true;
            var games = _db.Game.Include(item => item.ClientUser).Where(item => item.ClientUser == null).ToList();

            foreach (var g in games)
                if (g.Name == game.Name)
                    check = false;
            var user = (from u in _db.User
                        where u.Id == userId
                        select u).ToList().FirstOrDefault();

            if (check && user is not null)
            {
                _db.Game.Add(new Game()
                {
                    Name = game.Name,
                    IsPrivate = game.IsPrivate,
                    Password = game.Password,
                    HostUser = user
                });
                _db.SaveChanges();
                return (from g in _db.Game.Include(item => item.ClientUser)
                       where g.Name.Equals(game.Name) && g.ClientUser == null
                       select g).ToList().FirstOrDefault()??null!;
            }
            return null!;
        }

        public string Ready(Field field, int userId, int gameId)
        {
            try { _db = new ServerDbContext(); }
            catch (Exception ex) { Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Runtime database-releated error: " + ex.Message); return ex.Message; }

            int fieldId = 0;

            var user = (from u in _db.User
                        where u.Id == userId
                        select u).ToList().FirstOrDefault();
            var game = (from g in _db.Game
                        .Include(item => item.HostUser)
                        .Include(item => item.ClientUser)
                        .Include(item => item.HostField)
                        .Include(item => item.ClientField)
                        where g.Id == gameId
                        select g).ToList().FirstOrDefault();
            if (user is null)
                return "Incorrect user ID";
            if (game is null)
                return "Incorrect game ID";
            try
            {
                if(user.IsReady)
                {
                    user.IsReady = false;
                    _db.SaveChanges();
                    return "SUCCESS";
                }

                if (!CheckField(field))
                    return "Incorrect ships placement";

                if (userId == game.HostUser.Id)
                {
                    if (game.HostField == null)
                    {
                        game.HostField = field;
                        _db.SaveChanges();
                        fieldId = -1;
                    }
                    else fieldId = game.HostField.Id;
                }
                else if (userId == game.ClientUser?.Id)
                {
                    if (game.ClientField == null)
                    {
                        game.ClientField = field;
                        _db.SaveChanges();
                        fieldId = -1;
                    }
                    else fieldId = game.ClientField.Id;
                }
                else
                    return "Incorrect user ID";

                if(fieldId != -1)
                {
                    var cells = (from f in _db.Field
                                     where f.Id == fieldId
                                     select f.Cells).ToList().FirstOrDefault() ?? null!;
                    for (int i = 0; i < cells.Count; i++)
                    {
                        cells[i].IsContainsShip = field.Cells[i].IsContainsShip;
                        cells[i].IsHit = field.Cells[i].IsHit;
                    }
                    _db.SaveChanges();
                }
            }
            catch (Exception ex) { Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Runtime error: " + ex.Message); return ex.Message; }

            user.IsReady = true;

            if (game.ClientUser is not null && game.HostUser.IsReady && game.ClientUser.IsReady)
            {
                game.HostUser.IsTurn = true;
                game.ClientUser.IsTurn = false;
            }

            _db.SaveChanges();
            return "SUCCESS";
        }
        public Game EnemyWait(int gameId, int userId)
        {
            try { _db = new ServerDbContext(); }
            catch (Exception ex) { Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Runtime database-releated error: " + ex.Message); return null!; }
            var user = (from u in _db.User
                        where u.Id == userId
                        select u).FirstOrDefault();
            var game = (from g in _db.Game
                        .Include(item => item.HostUser)
                        .Include(item => item.ClientUser)
                        .Include(item => item.HostField)
                            .ThenInclude(item => item.Cells)
                        .Include(item => item.ClientField)
                            .ThenInclude(item => item.Cells)
                        .Include(item => item.Winner)
                        where g.Id == gameId
                        select g).FirstOrDefault();
            if (user is null || game is null)
                return null!;

            if (game.ClientUser is not null && game.HostUser.IsReady && game.ClientUser.IsReady)
            {
                if (user.Login == game.HostUser.Login)
                {
                    for (int x = 0; x < game.ClientField?.Cells.Count; x++)
                    {
                        if (game.ClientField.Cells[x].IsHit == false)
                        {
                            game.ClientField.Cells[x].IsContainsShip = false;
                        }
                    }
                }
                else
                {
                    for (int x = 0; x < game.HostField?.Cells.Count; x++)
                    {
                        if (game.HostField.Cells[x].IsHit == false)
                        {
                            game.HostField.Cells[x].IsContainsShip = false;
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

        public static async Task<string?> Shoot(int fieldId, int gameId, int userId, int index)
        {
            try
            {
                await using ServerDbContext context = new();

                Field? field = context.Field
                    .Include(item => item.Cells)
                    .FirstOrDefault(item => item.Id == fieldId);
                Game? game = context.Game
                    .Include(item => item.HostUser)
                    .Include(item => item.ClientUser)
                    .FirstOrDefault(item => item.Id == gameId);
                User? user = context.User.FirstOrDefault(item => item.Id == userId);

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
                    game.ClientUser.IsTurn = user.Login == game.ClientUser.Login;

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
                    game.ClientUser.IsTurn = user.Login == game.HostUser.Login;
                }

                await context.SaveChangesAsync();

                return null;
            }
            catch (Exception ex)
            {
                return ex.Message;
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
