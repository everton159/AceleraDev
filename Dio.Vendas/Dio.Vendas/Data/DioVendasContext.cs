using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Dio.Vendas;

namespace Dio.Vendas.Data
{
    public class DioVendasContext : DbContext
    {
       // private string _connectionString = "Server=(localdb)\\mssqllocaldb;Database=DioVendasContext-c8444df2-8f11-48a8-8bc8-2a02e9a2f386;Trusted_Connection=True;MultipleActiveResultSets=true";

       //public DioVendasContext (DbContextOptions<DioVendasContext> options)
       //    : base(options)
       //{
       //}
        //public DioVendasContext(string connectionString)
        //{
        //    _connectionString = connectionString;

        //}
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString: @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Everton\DioVendasContext-c8444df2-8f11-48a8-8bc8-2a02e9a2f386.mdf;Integrated Security=True;Connect Timeout=30");
        }

        public DbSet<Dio.Vendas.Produto> Produto { get; set; }
    }
}
