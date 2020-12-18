using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Dio.Vendas;
using Dio.Vendas.Data;
using Microsoft.Azure.ServiceBus;
using Microsoft.Extensions.Configuration;
using Dio.Vendas.Consumers;

namespace Dio.Vendas.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VendasController : ControllerBase
    {
        private readonly DioVendasContext _context;
    

        public VendasController(DioVendasContext context)
        {
            _context = context;
        }

        // GET: api/Vendas
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProduto()
        {
               
            return await _context.Produto.Where(x => x.Quantidade >0).ToListAsync();
        }

         // POST: api/Vendas
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Venda>> PostProduto(Venda venda)
        {

            if (!ModelState.IsValid)
                return ValidationProblem("Dados inválido");

            if (_context.Produto.Where(x => x.Id == venda.CodigoProduto).Count() == 0)
                return ValidationProblem("Código de produto inválido");

            if (_context.Produto.Where(x=> x.Id == venda.CodigoProduto).FirstOrDefault().Quantidade < venda.Quantidade)
                return ValidationProblem("Quantidade superior ao estoque");
            

            var produto = _context.Produto.Where(x => x.Id == venda.CodigoProduto).FirstOrDefault();
            produto.Quantidade -=venda.Quantidade;

            await _context.SaveChangesAsync();
            await EscrevenoTopicoAsync(produto, "produtovendido");


           return Ok();
        }

        private async Task EscrevenoTopicoAsync(object mensagem, string fila)
        {

            var serviceBusClient = new TopicClient(Utils.ServicoConsumer, fila);

            var message = new Message(mensagem.ToJsonBytes());
            message.ContentType = "application/json";

            await serviceBusClient.SendAsync(message);

        }
    }
}
