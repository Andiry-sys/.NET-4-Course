using System.Globalization;
using System.Text;
using Npgsql;

namespace Lab1;

class Program
{
    private static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.InputEncoding  = Encoding.UTF8;
        var ua = CultureInfo.GetCultureInfo("uk-UA");

        var cs = "Host=localhost;Port=5432;Username=postgres;Database=DbForUniversity;Password=andy;";

        using var conn = new NpgsqlConnection(cs);
        conn.Open();

        const string sqlClients = @"
            SELECT
              clientid      ,
              clientname    ,
              phone         ,
              orderscount  ,
              firstorderdate ,
              lastorderdate  
            FROM v_clients_2plus_orders
            ORDER BY orderscount DESC, clientname;";

        using (var cmd = new NpgsqlCommand(sqlClients, conn))
        using (var rdr = cmd.ExecuteReader())
        {
            Console.WriteLine("=== Клієнти (>=2 замовлення) ===");
            while (rdr.Read())
            {
                var name      = rdr["clientname"];
                var phone     = rdr["phone"];
                var cnt       = rdr["orderscount"]; 
                var firstDate = rdr["firstorderdate"];
                var lastDate  = rdr["lastorderdate"];

                Console.WriteLine(
                    $"{name} | {phone} | Замовлень: {cnt} | " +
                    $"Перше: {firstDate} | Останнє: {lastDate} |"
                );
            }
        }

        const string sqlLines = @"
            SELECT
              orderdate   ,
              clientid    ,
              client_name ,
              pizza_name  ,
              quantity    ,
              price       ,
              discount   ,
              linetotal   
            FROM v_order_lines
            ORDER BY orderdate, orderid;";

        using (var cmd2 = new NpgsqlCommand(sqlLines, conn))
        using (var rdr2 = cmd2.ExecuteReader())
        {
            Console.WriteLine("\n=== Таблиця замовлень ===");
            while (rdr2.Read())
            {
                var orderDate = rdr2["orderdate"];
                var client    = rdr2["client_name"];
                var pizza     = rdr2["pizza_name"];
                var qty       = rdr2["quantity"];
                var price     = rdr2["price"];
                var discount  = rdr2["discount"];
                var total     = rdr2["linetotal"];

                Console.WriteLine(
                    $"{orderDate} | {client} | {pizza} x{qty} | " +
                    $"{price} (-{discount:0.#}% ) → {total} |"
                );
            }
        }
    }
}
