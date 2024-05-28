using Microsoft.EntityFrameworkCore;
using Models;

namespace Server
{
    public class DbServer
    {
        private ServerDbContext _db = null!;
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

            var games = _db.Game.OrderByDescending(g => g.Id).ToList().Take(5);
            Console.WriteLine("\nGames:\n");
            foreach (var game in games)
            {
                Console.WriteLine($"Id: {game.Id} | Name: {game.Name} | IsPrivate?: {game.IsPrivate}" +
                    $"Password: {game.Password} | HostUserId: {game.HostUserId} | ClientUserId: {game.ClientUserId}");
            }

            var cgames = _db.CompletedGame.OrderByDescending(cg => cg.Id).ToList().Take(5);
            Console.WriteLine("\nCompleted Games:\n");
            foreach (var game in cgames)
            {
                Console.WriteLine($"Id: {game.Id} | Name: {game.Name} | IsPrivate?: {game.IsPrivate}" +
                    $"Password: {game.Password} | Winner: {game.Winner} | HostUserId: {game.HostUserId} | ClientUserId: {game.ClientUserId}");
            }

            var fields = _db.Field.OrderByDescending(f => f.Id).ToList().Take(5);
            Console.WriteLine("\nFields:\n");
            foreach (var field in fields)
            {
                Console.WriteLine($"Id: {field.Id}");
            }

            var cells = _db.Cell.OrderByDescending(c => c.Id).ToList().Take(5);
            Console.WriteLine("\nCells:\n");
            foreach (var cell in cells)
            {
                Console.WriteLine($"Id: {cell.Id} | FieldId: {cell.FieldId} | " +
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
                .Where(item => item.ClientUserId == null)
                .ToList();
            return games;
        }

        public Game GetGame(int gameId)
        {
            try { _db = new ServerDbContext(); }
            catch (Exception ex) { Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Runtime database-releated error: " + ex.Message); return null!; }
            Game? game = (from g in _db.Game
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
            game.ClientUserId = user.Id;
            _db.SaveChanges();

            game.HostUser.IsReady = false;
            return game;
        }

        public Game CreateGame(Game game, int userId)
        {
            try { _db = new ServerDbContext(); }
            catch (Exception ex) { Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Runtime database-releated error: " + ex.Message); return null!; }
            bool check = true;
            var games = _db.Game.Where(item => item.ClientUserId == null).ToList();

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
                    HostUserId = userId
                });
                _db.SaveChanges();
                return (from g in _db.Game
                       where g.Name.Equals(game.Name) && g.ClientUserId == null
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

                field.GameId = gameId;
                if (userId == game.HostUserId)
                {
                    if (game.HostFieldId == null)
                    {
                        game.HostField = field;
                        _db.SaveChanges();
                        game.HostFieldId = game.HostField.Id;
                        _db.SaveChanges();
                        fieldId = -1;
                    }
                    else fieldId = (int)game.HostFieldId;
                }
                else if (userId == game.ClientUserId)
                {
                    if (game.ClientFieldId == null)
                    {
                        game.ClientField = field;
                        _db.SaveChanges();
                        game.ClientFieldId = game.ClientField.Id;
                        _db.SaveChanges();
                        fieldId = -1;
                    }
                    else fieldId = (int)game.ClientFieldId;
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
                    for (int x = 0; x < game.ClientField.Cells.Count; x++)
                    {
                        if (game.ClientField.Cells[x].IsHit == false)
                        {
                            game.ClientField.Cells[x].IsContainsShip = false;
                        }
                    }
                }
                else
                {
                    for (int x = 0; x < game.HostField.Cells.Count; x++)
                    {
                        if (game.HostField.Cells[x].IsHit == false)
                        {
                            game.HostField.Cells[x].IsContainsShip = false;
                        }
                    }
                }
            }

            return game;
        }

        public static async Task<string?> Shoot(int cellId, int gameId, int userId)
        {
            try
            {
                await using ServerDbContext context = new();

                Cell? cell = context.Cell.FirstOrDefault(item => item.Id == cellId);
                Game? game = context.Game
                    .Include(item => item.HostUser)
                    .Include(item => item.ClientUser)
                    .FirstOrDefault(item => item.Id == gameId);
                User? user = context.User.FirstOrDefault(item => item.Id == userId);

                if (cell is null || game is null || user is null)
                {
                    return "cell or game or user in null";
                }

                if (user.IsTurn == false)
                {
                    return "not your turn";
                }

                cell.IsHit = true;

                game.HostUser.IsTurn = user.Login != game.HostUser.Login;
                game.ClientUser.IsTurn = user.Login == game.HostUser.Login;

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
