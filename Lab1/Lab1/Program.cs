using System.Globalization;
using System.Text;
using Microsoft.Data.Sqlite;

namespace Lab1;

class Program
{
    static string ResolveDbPath()
    {
        var rootDb = Path.GetFullPath(Path.Combine(
            AppContext.BaseDirectory,  "..", "..", "..", "..", "Db", "Db.db"
        ));
            return rootDb;
        
    }
    
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding  = Encoding.UTF8;
        var ua = CultureInfo.GetCultureInfo("uk-UA");

        var dbPath = ResolveDbPath();
        var cs = $"Data Source={dbPath}";

        using var conn = new SqliteConnection(cs);
        conn.Open();

        var sql = @"
SELECT
  o.Id AS OrderId,
  o.OrderDate,
  c.LastName || ' ' || c.FirstName AS ClientName,
  p.Name AS PizzaName,
  d.Quantity,
  p.Price,
  p.Discount,
  ROUND(d.Quantity * p.Price * (1 - p.Discount/100.0), 2) AS LineTotal
FROM Orders o
JOIN Clients c      ON c.Id = o.ClientId
JOIN OrderDetails d ON d.OrderId = o.Id
JOIN Pizzas p       ON p.Id = d.PizzaId
ORDER BY o.OrderDate, o.Id;";

        using var cmd = new SqliteCommand(sql, conn);
        using var rdr = cmd.ExecuteReader();
        while (rdr.Read())
        {
            var orderDate = Convert.ToString(rdr["OrderDate"]);
            var orderId   = Convert.ToInt32(rdr["OrderId"]);
            var client    = Convert.ToString(rdr["ClientName"]);
            var pizza     = Convert.ToString(rdr["PizzaName"]);
            var qty       = Convert.ToInt32(rdr["Quantity"]);
            var price     = Convert.ToDecimal(rdr["Price"]);
            var discount  = Convert.ToDecimal(rdr["Discount"]);
            var lineTotal = Convert.ToDecimal(rdr["LineTotal"]);

            Console.WriteLine(
                $"{orderDate} | #{orderId} | {client} | {pizza} x{qty} | " +
                $"{price.ToString("C0", ua)} (-{discount:0}% ) → {lineTotal.ToString("C0", ua)}"
            );
        }
    }
}