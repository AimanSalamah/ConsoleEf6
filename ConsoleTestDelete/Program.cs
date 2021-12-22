// See https://aka.ms/new-console-template for more information
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

var db = new Db(new DbContextOptions<Db>
{
});
bool loop = true;
while(loop)
{
    Console.WriteLine("create db\ndelete db\nfill db\ntest join\ntest delete\nexit");
    switch(Console.ReadLine())
    {
        case "create db":
            db.Database.EnsureCreated();
            break;
        case "delete db":
            db.Database.EnsureDeleted();
            break;
        case "fill db":
            for (int i = 0; i < 50; i++)
            {
                db.Customers.Add(new Customers { Name = $"Customer {i}", UnitId = new Random().Next(20) });
            }
            for (int i = 0; i < 30; i++)
            {
                db.Units.Add(new Units { Name = $"Unit {i}" });
            }
            Console.WriteLine(db.SaveChanges());           
            break;
        case "test join":
            var units = new List<Units>
            {
                new(){ Id = 0, Name = "test unut 0" },
                new(){ Id = 1, Name = "test unut 1" },
                new(){ Id = 2, Name = "test unut 2" },
                new(){ Id = 3, Name = "test unut 3" },
                new(){ Id = 4, Name = "test unut 4" },
            };
            var data = (from c in db.Customers.ToList()
                        join u in units on c.UnitId equals u.Id
                        select new Customers { Id = c.Id, Name = c.Name, UnitId = c.UnitId, UnitName = u.Name}).ToList();
            break;
        case "show data":
            foreach (var custoemr in db.Customers)
            {
                Console.WriteLine($"id:{custoemr.Id}\tname:{custoemr.Name}\tunitid:{custoemr.UnitId}");
            }
            foreach (var unit in db.Units)
            {
                Console.WriteLine($"id:{unit.Id}\tname:{unit.Name}");
            }
            break;
        case "test delete":
            var customers = new List<Customers>
            {
                new Customers{ Id = 9},
                new Customers{ Id = 10},
                new Customers{ Id = 11},
            };
            db.Customers.RemoveRange(customers);
            Console.WriteLine(db.SaveChanges());           
            break;
        case "exit":
            loop = false;
            break;
    }
}
Console.ReadLine();




public class Db : DbContext
{
    public Db(DbContextOptions<Db> options) : base(options)
    {

    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ConsoleTestDb;Trusted_Connection=True;MultipleActiveResultSets=true");
        base.OnConfiguring(optionsBuilder);
    }
    public DbSet<Customers> Customers { get; set; }
    public DbSet<Units> Units { get; set; }
}
public class Customers
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int UnitId { get; set; }
    [NotMapped]
    public string UnitName { get; set; }
}
public class Units
{
    public int Id { get; set; }
    public string Name { get; set; }
}