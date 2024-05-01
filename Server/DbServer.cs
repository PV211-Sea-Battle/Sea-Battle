using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server
{
    public class DbServer
    {
        private void CreatingTest()
        {
            var db = new ServerDbContext();
        }

        public void ShowFirst5RowsOfEveryTable()
        {
            using var db = new ServerDbContext();
            var users = db.User.OrderByDescending(u => u.Id).ToList().Take(5);

            Console.WriteLine("\nUsers:\n");
            foreach (var user in users)
            {
                Console.WriteLine($"Id: {user.Id} | Login: {user.Login} | " +
                    $"Password: {user.Password} | Is in game?: {user.IsInGame}"); // конфиденциальность 80 лвла
            }

            var games = db.Game.OrderByDescending(g => g.Id).ToList().Take(5);
            Console.WriteLine("\nGames:\n");
            foreach (var game in games)
            {
                Console.WriteLine($"Id: {game.Id} | Name: {game.Name} | IsPrivate?: {game.IsPrivate}" +
                    $"Password: {game.Password} | Winner: {game.Winner} | HostUserId: {game.HostUserId} | ClientUserId: {game.ClientUserId}");
            }

            var fields = db.Field.OrderByDescending(f => f.Id).ToList().Take(5);
            Console.WriteLine("\nFields:\n");
            foreach (var field in fields)
            {
                Console.WriteLine($"Id: {field.Id} | UserId: {field.UserId} | IsActive?: {field.IsActive}");
            }

            var cells = db.Cell.OrderByDescending(c => c.Id).ToList().Take(5);
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
            using var db = new ServerDbContext();
            var users = db.User.ToList();

            foreach (var user in users)
                if (user.Login == login)
                    check = false;

            if (check)
            {
                try
                {
                    db.User.Add(new()
                    {
                        Login = login,
                        Password = pass,
                        IsInGame = false
                    });
                    db.SaveChanges();
                }
                catch (Exception ex) { Console.WriteLine(ex.Message); return false; }
                return true;
            }
            return false;
        }
        public User SignUp(string login, string pass)
        {
            User res = null!;
            using var db = new ServerDbContext();
            var users = db.User.ToList();

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

        //...
    }
}
