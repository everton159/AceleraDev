using Dio.Estoque.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.ServiceBus;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dio.Estoque.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProdutosController : ControllerBase
    {
        private readonly DioEstoqueContext _context;

        public ProdutosController(DioEstoqueContext context)
        {
            _context = context;
        }

        // GET: api/Produtos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Produto>>> GetProduto()
        {
            return await _context.Produto.ToListAsync();
        }

        // GET: api/Produtos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Produto>> GetProduto(int id)
        {
            var produto = await _context.Produto.FindAsync(id);

            if (produto == null)
            {
                return NotFound();
            }

            return produto;
        }

        // PUT: api/Produtos/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutProduto(int id, Produto produto)
        {
            if (id != produto.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                _context.Produto.Where(b => b.Descricao == produto.Descricao);
                return ValidationProblem("Erro ao validar os dados do produto", title: "Parâmetro invalido");

            }

            if (_context.Produto.Where(b => b.Descricao == produto.Descricao && produto.Id != id).Count() >0)
            {
                return ValidationProblem("Já existe um produto com essa descrição", title: "Cadastro já existente");

            }

            _context.Entry(produto).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProdutoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            await EscrevenoTopicoAsync(produto, "produtoeditado");

            return Ok("Produto alterado");
        }

        // POST: api/Produtos
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Produto>> PostProduto(Produto produto)
        {

            if(!ModelState.IsValid)
            {
                return ValidationProblem("Erro ao validar os dados do produto",  title: "Parâmetro invalido");
            }

            var x = _context.Produto.Where(b => b.Descricao == produto.Descricao);
            if (_context.Produto.Where(b => b.Descricao == produto.Descricao).Count() > 0)
            {
                return ValidationProblem("Já existe um produto com essa descrição",title: "Cadastro já existente");

            }
            _context.Produto.Add(produto);

            await _context.SaveChangesAsync();

            await EscrevenoTopicoAsync(produto, "produtocriado");

            return CreatedAtAction("GetProduto", new { id = produto.Id }, produto);
        }

       
        private bool ProdutoExists(int id)
        {
            return _context.Produto.Any(e => e.Id == id);
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
