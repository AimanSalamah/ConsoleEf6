using Microsoft.EntityFrameworkCore;

bool loop = true;
Help();
while(loop)
{
    using (var db = new Db(new DbContextOptions<Db>()))
    {
        switch(Console.ReadLine())
        {
            case "clear":
                Console.Clear();
                break;
            case "help":
                Help();
                break;
            case "create db":
                Console.WriteLine(String.Format("Database {0}", db.Database.EnsureCreated() ? "Created" : "Not created"));
                break;
            case "delete db":
                Console.WriteLine(String.Format("Database {0}", db.Database.EnsureDeleted() ? "Deleted" : "Not deleted"));
                break;
            case "from query":
                var result = (from unit in db.Units
                            join customer in db.Customers on unit.Id equals customer.UnitId
                            select new
                            {
                                Unit = unit.Name,
                                Customer = customer.Name
                            }).ToList();

                foreach (var item in result)
                {
                    Console.WriteLine(item.Unit + " " + item.Customer);
                }
                break;
            case "fill db":
                for (int i = 0; i < 21; i++)
                {
                    db.Units.Add(new Units { Name = $"Unit {i}" });
                }
                Console.WriteLine(db.SaveChanges() + "Units created");
                var CustomerList = new List<Customers>();
                for (int i = 1; i < 31; i++)
                {
                    CustomerList.Add(new Customers { Name = $"Customer {i}", UnitId = new Random().Next(1,20), Number = i });
                }
                db.Customers.AddRange(CustomerList);
                Console.WriteLine(db.SaveChanges() + "Customers created");
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
                            select new Customers { Id = c.Id, Name = c.Name, UnitId = c.UnitId, Unit = u}).ToList();
                break;
            case "test update":
                var UpdateModel = new Customers { Id = 9, Number = 121212 };
                db.Entry(UpdateModel).Property("Number").IsModified = true;
                Console.WriteLine($"Save result: {db.SaveChanges()}");
                break;
            case "show data":
                foreach (var custoemr in db.Customers)
                {
                    Console.WriteLine($"id:{custoemr.Id}\tname:{custoemr.Name}\tnumber:{custoemr.Number}\tunitid:{custoemr.UnitId}");
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
}

static void Help()
{
    Console.WriteLine(
        "create db\n" +
        "delete db\n" +
        "fill db\n" +
        "show data\n" +
        "test join\n" +
        "test update\n" +
        "test delete\n" +
        "from query\n" +
        "clear\n" +
        "help\n" +
        "exit\n");
}



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
    public int Number { get; set; }
    public int UnitId { get; set; }
    public virtual Units Unit { get; set; }
}
public class Units
{
    public int Id { get; set; }
    public string Name { get; set; }
    public virtual ICollection<Customers> Customers { get; set; }
}