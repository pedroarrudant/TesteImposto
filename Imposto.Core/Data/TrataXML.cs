using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Xml.Serialization;
using Imposto.Core.Domain;
using System.Configuration;

namespace Imposto.Core.Data
{
    public class TrataXML
    {
        /// <summary>
        /// Serializa a classe Nota Fiscal para criacao do XML.
        /// </summary>
        /// <param name="nf">Nota fiscal a ser gerada no XML</param>
        public void serializador(NotaFiscal nf)
        {
            string pathXML = ConfigurationManager.AppSettings["EnderecoEnvioXML"].ToString();

            StringBuilder pathGravacaoArquivo = new StringBuilder(pathXML);

            pathGravacaoArquivo.Append(@"\NF_");
            pathGravacaoArquivo.Append(nf.NumeroNotaFiscal.ToString());
            pathGravacaoArquivo.Append(DateTime.Now.ToString("yyyyMMdd_HHmmss"));
            pathGravacaoArquivo.Append(".XML");

            try
            {
                XmlSerializer serializadorXML = new XmlSerializer(typeof(NotaFiscal));

                using (TextWriter writer = new StreamWriter(pathGravacaoArquivo.ToString()))
                {
                    serializadorXML.Serialize(writer, nf);
                }
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
