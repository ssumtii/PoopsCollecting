using System;
using System.Collections.Generic;
using System.Text;
using System.Data.Entity;
using System.Linq;

namespace PoopCollecting
{
    public class Player
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public int Score { get; set; }
    }
    public class Context : DbContext
    {
        public Context() : base ("PlayersData")
        {
            var initializer = new CreateDatabaseIfNotExists<Context>();
            Database.SetInitializer(initializer);
        }
        public DbSet<Player> Players { get; set; }
    }
    
}
