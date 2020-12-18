using Dio.Vendas.Data;
using Microsoft.Azure.ServiceBus;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Dio.Vendas.Consumers
{
    public static class ConsumerProdutoCriado
    {
        public static void receber()
        {
            var connectionStr = Utils.ServicoConsumer;
            var topic = "produtocriado";
            var subscription = "produtoCriadoConsumidor00";

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

            DioVendasContext _context = new DioVendasContext();
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

        private static async void Salvar(DioVendasContext c, Produto p, int id)
        {
            

            var produto =  c.Produto.FirstOrDefault(m => m.Id == id);
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
