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
                Console.WriteLine($"Id: {user.Id} | Login: {user.Login} | " +
                    $"Password: {user.Password} | Victories: {user.Victories} | Defeats: {user.Defeats}"); // конфиденциальность 80 лвла
            }

            var games = _db.Game.OrderByDescending(g => g.Id).ToList().Take(5);
            Console.WriteLine("\nGames:\n");
            foreach (var game in games)
            {
                Console.WriteLine($"Id: {game.Id} | Name: {game.Name} | IsPrivate?: {game.IsPrivate}" +
                    $"Password: {game.Password} | Winner: {game.Winner} | HostUserId: {game.HostUserId} | ClientUserId: {game.ClientUserId}");
            }

            var fields = _db.Field.OrderByDescending(f => f.Id).ToList().Take(5);
            Console.WriteLine("\nFields:\n");
            foreach (var field in fields)
            {
                Console.WriteLine($"Id: {field.Id} | UserId: {field.UserId} | IsActive?: {field.IsActive}");
            }

            var cells = _db.Cell.OrderByDescending(c => c.Id).ToList().Take(5);
            Console.WriteLine("\nCells:\n");
            foreach (var cell in cells)
            {
                Console.WriteLine($"Id: {cell.Id} | FieldId: {cell.FieldId} | " +
                    $"IsContainsShip?: {cell.IsContainsShip} | IsHit?: {cell.IsHit}");
            }

            var cgames = _db.CompletedGames.OrderByDescending(cg => cg.Id).ToList().Take(5);
            Console.WriteLine("\nCompleted Games:\n");
            foreach (var game in cgames)
            {
                Console.WriteLine($"Id: {game.Id} | Name: {game.Name} | IsPrivate?: {game.IsPrivate}" +
                    $"Password: {game.Password} | Winner: {game.Winner} | HostUserId: {game.HostUserId} | ClientUserId: {game.ClientUserId}");
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
                .Include(item => item.User)
                .Where(item => item.ClientUserId == -1).ToList();
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
                       where g.Id == gameId
                       select g).ToList().FirstOrDefault();
            User? user = (from u in _db.User
                          where u.Id == userId
                          select u).ToList().FirstOrDefault();
            if (game is null || user is null)
                return null!;
            game.ClientUserId = user.Id;
            _db.SaveChanges();
            return game;
        }

        public Game CreateGame(Game game, int userId)
        {
            try { _db = new ServerDbContext(); }
            catch (Exception ex) { Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Runtime database-releated error: " + ex.Message); return null!; }
            bool check = true;
            var games = _db.Game.Where(item => item.ClientUserId == -1).ToList();

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
                    Winner = null,
                    HostUserId = userId,
                    ClientUserId = -1
                });
                _db.SaveChanges();
                return (from g in _db.Game
                       where g.Name.Equals(game.Name) && g.ClientUserId == -1
                       select g).ToList().FirstOrDefault()??null!;
            }
            return null!;
        }

        public string Ready(Field field)
        {
            try { _db = new ServerDbContext(); }
            catch (Exception ex) { Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Runtime database-releated error: " + ex.Message); return ex.Message; }

            var user = (from u in _db.User
                        where u.Id == field.UserId
                        select u).ToList().FirstOrDefault();
            if (user is null)
                return "Incorrect user ID";

            try
            {
                bool check = true;
                foreach(var f in _db.Field)
                {
                    if (f.UserId == field.UserId && f.IsActive)
                        check = false;
                }
                if(check)
                {
                    field.IsActive = true;
                    _db.Field.Add(field);
                    _db.SaveChanges();
                }

                field.Id = (from f in _db.Field
                            where f.UserId == field.UserId && f.IsActive
                            select f.Id).ToList().FirstOrDefault();
                foreach (var c in field.Cells)
                {
                    c.FieldId = field.Id;
                    _db.Cell.Add(c);
                }
                _db.SaveChanges();
            }
            catch (Exception ex) { Console.WriteLine($"[{DateTime.Now.ToLongTimeString()}] Runtime error: " + ex.Message); return ex.Message; }

            return "SUCCESS";
        }
    }
}
