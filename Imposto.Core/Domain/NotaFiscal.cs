using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.IO;
using System.Configuration;
using Imposto.Core.Data;

namespace Imposto.Core.Domain
{
    [Serializable]
    public class NotaFiscal
    {
        public int Id { get; set; }
        public int NumeroNotaFiscal { get; set; }
        public int Serie { get; set; }
        public string NomeCliente { get; set; }

        public string EstadoDestino { get; set; }
        public string EstadoOrigem { get; set; }

        public NotaFiscalItem notaFiscalItem = new NotaFiscalItem();

        [XmlIgnoreAttribute]
        public IEnumerable<NotaFiscalItem> ItensDaNotaFiscal { get; set; }

        public NotaFiscal()
        {
            ItensDaNotaFiscal = new List<NotaFiscalItem>();
        }
        /// <summary>
        /// Faz a emissao da nota fical de acordo com o pedido enviado
        /// </summary>
        /// <param name="pedido">Pedido para gerar a nota fiscal</param>
        public void EmitirNotaFiscal(Pedido pedido)
        {
            NumeroNotaFiscal = 99999;
            Serie = new Random().Next(Int32.MaxValue);
            NomeCliente = pedido.NomeCliente;

            EstadoOrigem = pedido.EstadoOrigem;
            EstadoDestino = pedido.EstadoDestino;

            DBController bancoDados = new DBController();

            foreach (PedidoItem itemPedido in pedido.ItensDoPedido)
            {
                string CFOP = bancoDados.retornaCFOP(EstadoOrigem, EstadoDestino);

                notaFiscalItem.Cfop = CFOP;

                if (EstadoDestino == EstadoOrigem)
                {
                    notaFiscalItem.TipoIcms = "60";
                    notaFiscalItem.AliquotaIcms = 0.18;
                }
                else
                {
                    notaFiscalItem.TipoIcms = "10";
                    notaFiscalItem.AliquotaIcms = 0.17;
                }
                if (notaFiscalItem.Cfop == "6.009")
                {
                    notaFiscalItem.BaseIcms = itemPedido.ValorItemPedido * 0.90; //redução de base
                }
                else
                {
                    notaFiscalItem.BaseIcms = itemPedido.ValorItemPedido;
                }
                notaFiscalItem.ValorIcms = notaFiscalItem.BaseIcms * notaFiscalItem.AliquotaIcms;

                if (itemPedido.Brinde)
                {
                    notaFiscalItem.TipoIcms = "60";
                    notaFiscalItem.AliquotaIcms = 0.18;
                    notaFiscalItem.ValorIcms = notaFiscalItem.BaseIcms * notaFiscalItem.AliquotaIcms;

                    notaFiscalItem.AliquotaIpi = 0;
                }
                else
                {
                    notaFiscalItem.AliquotaIpi = 0.10;
                }

                if (EstadoDestino == "SP" || EstadoDestino == "MG" || EstadoDestino == "ES" || EstadoDestino == "RJ")
                {
                    notaFiscalItem.Desconto = 0.10;

                    itemPedido.ValorItemPedido = itemPedido.ValorItemPedido - (itemPedido.ValorItemPedido * notaFiscalItem.Desconto);
                }

                notaFiscalItem.NomeProduto = itemPedido.NomeProduto;
                notaFiscalItem.CodigoProduto = itemPedido.CodigoProduto;

                notaFiscalItem.BaseIpi = itemPedido.ValorItemPedido;
                notaFiscalItem.ValorIpi = notaFiscalItem.BaseIpi * notaFiscalItem.AliquotaIpi;

            }
        }
    }
}
