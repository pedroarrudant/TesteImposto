using Imposto.Core.Domain;
using Imposto.Core.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Imposto.Core.Service
{
    public class NotaFiscalService
    {
        /// <summary>
        /// Gera a nota fiscal, persistindo os dados no XML e no banco de dados.
        /// </summary>
        /// <param name="pedido">Pedido a ser gerado a nota fiscal</param>
        public void GerarNotaFiscal(Domain.Pedido pedido)
        {
            NotaFiscal notaFiscal = new NotaFiscal();
            TrataXML XML = new Data.TrataXML();
            DBController bancoDados = new Data.DBController();

            notaFiscal.EmitirNotaFiscal(pedido);

            XML.serializador(notaFiscal);

            bancoDados.insereNF(notaFiscal);

            bancoDados.insereNFItem(notaFiscal, notaFiscal.notaFiscalItem);
        }
    }
}
