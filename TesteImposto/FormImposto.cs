using Imposto.Core.Service;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Imposto.Core.Domain;

namespace TesteImposto
{
    public partial class FormImposto : Form
    {
        private Pedido pedido = new Pedido();
        private void FormImposto_Load(object sender, EventArgs e)
        {
            cboEstadoOrigem.SelectedIndex = 0;
            cboEstadoDestino.SelectedIndex = 0;
        }
        public FormImposto()
        {
            InitializeComponent();
            dataGridViewPedidos.AutoGenerateColumns = true;
            dataGridViewPedidos.DataSource = GetTablePedidos();
            ResizeColumns();
        }
        /// <summary>
        /// Realinha as colunas do DataGridView
        /// </summary>
        private void ResizeColumns()
        {
            double mediaWidth = dataGridViewPedidos.Width / dataGridViewPedidos.Columns.GetColumnCount(DataGridViewElementStates.Visible);

            for (int i = dataGridViewPedidos.Columns.Count - 1; i >= 0; i--)
            {
                var coluna = dataGridViewPedidos.Columns[i];
                coluna.Width = Convert.ToInt32(mediaWidth);
            }
        }
        /// <summary>
        /// Retorna os pedidos que compoe o DataGriedView
        /// </summary>
        /// <returns>Retorna um objeto table com os pedidos listados</returns>
        private object GetTablePedidos()
        {
            DataTable table = new DataTable("pedidos");
            table.Columns.Add(new DataColumn("Nome do produto", typeof(string)));
            table.Columns.Add(new DataColumn("Codigo do produto", typeof(string)));
            table.Columns.Add(new DataColumn("Valor", typeof(decimal)));
            table.Columns.Add(new DataColumn("Brinde", typeof(bool)));

            return table;
        }

        /// <summary>
        /// Acao do botao para emitir nota fiscal.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonGerarNotaFiscal_Click(object sender, EventArgs e)
        {
            if (validaCamposCliente() == true)
            {
                NotaFiscalService service = new NotaFiscalService();
                pedido.EstadoOrigem = cboEstadoOrigem.Text;
                pedido.EstadoDestino = cboEstadoDestino.Text;
                pedido.NomeCliente = textBoxNomeCliente.Text;

                DataTable table = (DataTable)dataGridViewPedidos.DataSource;

                if (validaCampoItens() == true)
                {

                    foreach (DataRow row in table.Rows)
                    {
                        pedido.ItensDoPedido.Add(
                            new PedidoItem()
                            {
                                Brinde = Convert.ToBoolean(row["Brinde"]),
                                CodigoProduto = row["Codigo do produto"].ToString(),
                                NomeProduto = row["Nome do produto"].ToString(),
                                ValorItemPedido = Convert.ToDouble(row["Valor"].ToString())
                            });
                    }

                    try
                    {
                        service.GerarNotaFiscal(pedido);
                        MessageBox.Show("Operação efetuada com sucesso");
                        limpaCampos();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Erro na execução do comando. Mesagem: " + ex.Message);
                    }
                }

            }
        }

        /// <summary>
        /// Valida os campos do cabecalho, referente aos dados do cliente.
        /// </summary>
        /// <returns>Retorna verdadeiro em caso de sucesso no preenhimento e falha caso contrario</returns>
        private Boolean validaCamposCliente()
        {
            if (cboEstadoOrigem.Text == "Selecione...")
            {
                MessageBox.Show("Por favor, preencha o estado de origem.");
                return false;
            }
            else if (cboEstadoDestino.Text == "Selecione...")
            {
                MessageBox.Show("Por favor, preencha o estado de destino.");
                return false;
            }
            else if (textBoxNomeCliente.Text == "")
            {
                MessageBox.Show("Por favor, preencha o nome do cliente.");
                return false;
            }
            else
            {
                return true;
            }

        }
        /// <summary>
        /// Valida os campos dos itens pedido
        /// </summary>
        /// <returns>Retorna verdadeiro em caso de sucesso no preenhimento e falha caso contrario</returns>
        private Boolean validaCampoItens()
        {

            DataTable table = (DataTable)dataGridViewPedidos.DataSource;

            if (table.Rows.Count == 0)
            {
                MessageBox.Show("Por favor, preencha pelo menos um produto.");
                return false;
            }

            foreach (DataRow row in table.Rows)
            {
                if (row["Nome do produto"].ToString() == "")
                {
                    MessageBox.Show("Por favor, preencha o nome do produto.");
                    return false;
                }
                else if (row["Codigo do produto"].ToString() == "")
                {
                    MessageBox.Show("Por favor, preencha o codigo do produto.");
                    return false;
                }
                else if (row["Valor"].ToString() == "")
                {
                    MessageBox.Show("Por favor, preencha o valor do(s) produto(s).");
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Limpa os campos do formulario
        /// </summary>
        private void limpaCampos()
        {
            cboEstadoOrigem.SelectedIndex = 0;
            cboEstadoDestino.SelectedIndex = 0;
            textBoxNomeCliente.Text = "";
            this.dataGridViewPedidos.DataSource = null;
            this.dataGridViewPedidos.Rows.Clear();
            this.dataGridViewPedidos.DataSource = GetTablePedidos();
            ResizeColumns();
        }
    }
}
