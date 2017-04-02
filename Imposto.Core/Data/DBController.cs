using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using Imposto.Core.Domain;

namespace Imposto.Core.Data
{
    public class DBController
    {
        /// <summary>
        /// Insere dados da nota fiscal no banco de dados
        /// </summary>
        /// <param name="nf">Objeto nota fiscal a ser inserido</param>
        /// <returns></returns>
        public Boolean insereNF(NotaFiscal nf)
        {
            try
            {
                SqlConnection conexaoBD = gerenciadorConexaoBD(true);

                SqlCommand sqlProcedure = new SqlCommand("P_NOTA_FISCAL", conexaoBD);

                sqlProcedure.CommandType = CommandType.StoredProcedure;

                sqlProcedure.Parameters.AddWithValue("@pId", nf.Id);
                sqlProcedure.Parameters.AddWithValue("@pNumeroNotaFiscal", nf.NumeroNotaFiscal);
                sqlProcedure.Parameters.AddWithValue("@pSerie", nf.Serie);
                sqlProcedure.Parameters.AddWithValue("@pNomeCliente", nf.NomeCliente);
                sqlProcedure.Parameters.AddWithValue("@pEstadoDestino", nf.EstadoDestino);
                sqlProcedure.Parameters.AddWithValue("@pEstadoOrigem", nf.EstadoOrigem);

                sqlProcedure.ExecuteReader();

                return true;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                gerenciadorConexaoBD(false);
            }
        }

        /// <summary>
        /// Insere os itens da nota fiscal no banco de dados.
        /// </summary>
        /// <param name="nf">Notafiscal da qual os itens pertencem</param>
        /// <param name="nfItem">Itens da nota fiscal</param>
        /// <returns></returns>
        public Boolean insereNFItem(NotaFiscal nf, NotaFiscalItem nfItem)
        {
            try
            {
                SqlConnection conexaoBD = gerenciadorConexaoBD(true);

                SqlCommand sqlProcedure = new SqlCommand("P_NOTA_FISCAL_ITEM", conexaoBD);

                sqlProcedure.CommandType = CommandType.StoredProcedure;

                sqlProcedure.Parameters.AddWithValue("@pId", nfItem.Id);
                sqlProcedure.Parameters.AddWithValue("@pIdNotaFiscal", nfItem.IdNotaFiscal);
                sqlProcedure.Parameters.AddWithValue("@pCfop", nfItem.Cfop);
                sqlProcedure.Parameters.AddWithValue("@pTipoIcms", nfItem.TipoIcms);
                sqlProcedure.Parameters.AddWithValue("@pBaseIcms", nfItem.BaseIcms);
                sqlProcedure.Parameters.AddWithValue("@pAliquotaIcms", nfItem.AliquotaIcms);
                sqlProcedure.Parameters.AddWithValue("@pValorIcms", nfItem.ValorIcms);
                sqlProcedure.Parameters.AddWithValue("@pNomeProduto", nfItem.NomeProduto);
                sqlProcedure.Parameters.AddWithValue("@pCodigoProduto", nfItem.CodigoProduto);
                sqlProcedure.Parameters.AddWithValue("@pBaseIpi", nfItem.BaseIpi);
                sqlProcedure.Parameters.AddWithValue("@pAliquotaIpi", nfItem.AliquotaIpi);
                sqlProcedure.Parameters.AddWithValue("@pValorIpi", nfItem.ValorIpi);
                sqlProcedure.Parameters.AddWithValue("@pDesconto", nfItem.Desconto);

                sqlProcedure.ExecuteReader();

                return true;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                gerenciadorConexaoBD(false);
            }
        }

        /// <summary>
        /// Executa query diretamente no banco, sem possuir retorno.
        /// </summary>
        /// <param name="strQuery">Query a ser executada</param>
        /// <returns>Booleano com o sucesso ou falha da execucao da string</returns>
        public Boolean executaQuery(string pQuery)
        {
            try
            {
                SqlConnection conexaoBD = gerenciadorConexaoBD(true);

                SqlCommand sqlQuery = new SqlCommand(pQuery, conexaoBD);

                gerenciadorConexaoBD(false);

                return true;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                gerenciadorConexaoBD(false);
            }
        }

        /// <summary>
        /// Retorna o CFOP diretamente do banco de dados de acordo com o estado origem e o estado destino.
        /// </summary>
        /// <param name="estadoOrigem">Estado origem da Nota Fiscal</param>
        /// <param name="estadoDestino">Estado destino da Nota Fiscal</param>
        /// <returns>String com o CFOP</returns>
        public string retornaCFOP(string estadoOrigem, string estadoDestino)
        {
            StringBuilder sqlQuery = new StringBuilder("SELECT CFOP FROM CFOP_ESTADO(NOLOCK) WHERE ");
            sqlQuery.Append("ESTADOORIGEM = '");
            sqlQuery.Append(estadoOrigem);
            sqlQuery.Append("' AND ESTADODESTINO = '");
            sqlQuery.Append(estadoDestino);
            sqlQuery.Append("' ORDER BY CFOP");

            try
            {
                DataTable dataTable = new DataTable();

                SqlConnection conexaoBD = gerenciadorConexaoBD(true);

                SqlCommand sqlCmd = new SqlCommand(sqlQuery.ToString(), conexaoBD);

                SqlDataReader sqlReader = sqlCmd.ExecuteReader();

                if (sqlReader.HasRows)
                {
                    sqlReader.Read();
                    return sqlReader[0].ToString();
                }
                else
                {
                    throw new NotImplementedException();
                }
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                gerenciadorConexaoBD(false);
            }
        }

        /// <summary>
        /// Gerencia a conexao com o banco de dados conectando e desconectando quando necessario
        /// </summary>
        /// <param name="opcao"> True conecta ao banco, false o desconecta </param>
        private SqlConnection gerenciadorConexaoBD(Boolean pOpcao)
        {
            string strConexao = ConfigurationManager.ConnectionStrings["StringConexao"].ToString();

            SqlConnection conexaoBD = new SqlConnection(strConexao);

            try
            {
                if ((conexaoBD.State == ConnectionState.Closed && pOpcao == true) || (conexaoBD.State == ConnectionState.Open && pOpcao == false))
                {
                    conexaoBD.Open();
                }
                else if ((conexaoBD.State == ConnectionState.Open && pOpcao == false) || (conexaoBD.State == ConnectionState.Closed && pOpcao == false))
                {
                    conexaoBD.Close();
                }
                else
                {
                    throw new NotImplementedException();
                }

                return conexaoBD;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
