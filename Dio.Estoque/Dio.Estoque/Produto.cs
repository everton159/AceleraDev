using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Dio.Estoque
{
    public class Produto
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="é necesário informar uma descrição")]
        public string Descricao { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Quantidade de produto inválida")]
        public int Quantidade { get; set; }
        [Range(0, double.MaxValue,ErrorMessage ="Valor de produto inválido") ]
        public decimal Preco { get; set; }

    }
}
