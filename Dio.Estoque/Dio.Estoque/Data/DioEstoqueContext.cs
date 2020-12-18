using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Dio.Estoque;

namespace Dio.Estoque.Data
{
    public class DioEstoqueContext : DbContext
    {
       
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString: @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\Everton\DioEstoqueContext-4297e64e-12c7-44bc-918e-ded4e4e33aad.mdf;Integrated Security=True;Connect Timeout=30");
        }
        public DbSet<Dio.Estoque.Produto> Produto { get; set; }
    }
}
