using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dio.Vendas
{
    public class Venda
    {
        [Range(1, int.MaxValue)]
        public int CodigoProduto { get; set; }
        [Range(1, int.MaxValue)]

        public int Quantidade { get; set; }
    }
}
