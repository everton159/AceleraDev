using Dio.Estoque;
using Dio.Estoque.Data;
using Microsoft.Azure.ServiceBus;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dio.Estoque.Consumers
{
    public static class ConsumerProdutoVendido
    {
        public static void receber()
        {
            var connectionStr = Utils.ServicoConsumer;
            var topic = "produtovendido";
            var subscription = "consumidor00";

            var serviceBusClient = new SubscriptionClient(connectionStr, topic, subscription);

            var messageHandlerOptions = new MessageHandlerOptions(ExceptionReceivedHandler)
            {
                MaxConcurrentCalls = 1,
                AutoComplete = false
            };

            serviceBusClient.RegisterMessageHandler(ProcessMessageAsync, messageHandlerOptions);
        }

        private static Task ProcessMessageAsync(Message message, CancellationToken arg2)
        {
            var mensagem = message.Body.ParseJson<Produto>();

            DioEstoqueContext _context = new DioEstoqueContext();
            Produto p = new Produto
            {
                Id = mensagem.Id,
                Descricao = mensagem.Descricao,
                Preco = mensagem.Preco,
                Quantidade = mensagem.Quantidade
            };

            Salvar(_context, p, mensagem.Id);

            return Task.CompletedTask;
        }

        private static Task ExceptionReceivedHandler(ExceptionReceivedEventArgs arg)
        {
            throw new NotImplementedException();
        }

        private static async void Salvar(DioEstoqueContext c, Produto p, int ProdCod)
        {
            

            var produto =  c.Produto.FirstOrDefault(m => m.Id == ProdCod);
              if (produto == null)
            {
                c.Add(p);
            }
            else
            {
                produto.Id = p.Id;
                produto.Descricao = p.Descricao;
                produto.Preco = p.Preco;
                produto.Quantidade = p.Quantidade;
                c.Update(produto);
            }
            await c.SaveChangesAsync();
        }
    }
}
