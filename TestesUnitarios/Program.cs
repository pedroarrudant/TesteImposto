using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Imposto.Core.Data;
using Imposto.Core.Domain;
using Imposto.Core.Service;

namespace TestesUnitarios
{
    class Program
    {
        static void Main(string[] args)
        {
            NotaFiscalService service = new NotaFiscalService();
            NotaFiscal notaFiscal = new NotaFiscal();
            TrataXML trataXML = new TrataXML();
            DBController bancoDados = new DBController();

            Pedido pedido = RetornaPedido(notaFiscal);

            Console.WriteLine("Pressione um tecla para inicar os testes...");
            Console.ReadLine();

            try
            {
                testaNF(notaFiscal, pedido);
                Console.WriteLine("Sucesso ao emitir notas fiscais");

                trataXML.serializador(notaFiscal);
                Console.WriteLine("Sucesso ao serializar o XML");

                bancoDados.insereNF(notaFiscal);
                Console.WriteLine("Sucesso ao inserir a NF no BD");
                
                string CFOP = bancoDados.retornaCFOP(pedido.EstadoOrigem, pedido.EstadoDestino);
                Console.WriteLine("Sucesso ao obter o retorno do CFOP. Numero: " + CFOP);

                bancoDados.insereNFItem(notaFiscal, notaFiscal.notaFiscalItem);
                Console.WriteLine("Sucesso ao inserir os itens da NF no BD");

                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro no processo. Verifique o callstack. Erro:" + ex.Message);
            }
        }
        static Pedido RetornaPedido(NotaFiscal notaFiscal)
        {
            Pedido pedido = new Pedido();

            pedido.EstadoOrigem = "SP";
            pedido.EstadoDestino = "MG";
            pedido.NomeCliente = "Nome Cliente";

            pedido.ItensDoPedido.Add(
            new PedidoItem()
            {
                Brinde = true,
                CodigoProduto = "123456",
                NomeProduto = "ProdutoNome",
                ValorItemPedido = Convert.ToDouble("123.00")
            });

            return pedido;
        }
        static void testaNF(NotaFiscal notaFiscal, Pedido pedido)
        {
            try
            {
                notaFiscal.EmitirNotaFiscal(pedido);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Erro ao gerar a nota fiscal. Erro: " + ex.Message);
            }
        }
    }
}
