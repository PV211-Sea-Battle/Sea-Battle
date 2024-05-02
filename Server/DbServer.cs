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
                    $"Password: {user.Password} | Is in game?: {user.IsInGame}"); // конфиденциальность 80 лвла
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
                        Password = pass,
                        IsInGame = false
                    });
                    _db.SaveChanges();
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); return false; }
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
                    res = new()
                    {
                        Id = user.Id,
                        Login = login,
                        Password = pass,
                        IsInGame = user.IsInGame
                    };
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

        //...
    }
}
