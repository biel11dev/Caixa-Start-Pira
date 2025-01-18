using PDV.AcessoBancoDados;
using PDV.Negocios;
using PDV.ObjetoTransferencia;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PDV.Apresentacao.MovimentacaoCaixa
{
    public partial class FrmFecharCaixa : Form
    {
        public FrmFecharCaixa()
        {
            InitializeComponent();
            this.btnFecharCaixa.Click += new EventHandler(this.btnFecharCaixa_Click); // Associando evento

        }

        #region Instâncias

        CaixaNegocios caixaNegocios = new CaixaNegocios();
        GastosNegocios gastosNegocios = new GastosNegocios();
        Conexao conexao  = new Conexao();

        #endregion

        #region Variáveis

        decimal suprimento = 0;

        #endregion

        #region Métodos

        private void CarregarCampos()
        {
            int ultimaAbertura = caixaNegocios.VerificarSeCaixaEstaAberto();
            if (ultimaAbertura == 0)
            {
                MessageBox.Show("Caixa já se encontra fechado seu nóia", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DataRow drUltimaAbertura = caixaNegocios.PesquisarPorCodigo(ultimaAbertura).Rows[0];
            if (drUltimaAbertura != null)
            {
                lblValorAbertura.Text = drUltimaAbertura["Valor"].ToString();
                DateTime abertura = Convert.ToDateTime(drUltimaAbertura["Abertura"]);
                lblAbertura.Text = abertura.ToString("yyyy-MM-dd HH:mm:ss.fff");
                lblFechamento.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss.fff");

            }
            DateTime dataInicio = Convert.ToDateTime(lblAbertura.Text);
            DateTime dataFim = Convert.ToDateTime(lblFechamento.Text);


            DataTable dtFluxo = gastosNegocios.PesquisarMovimentosCaixa(Convert.ToDateTime(lblAbertura.Text), Convert.ToDateTime(lblFechamento.Text), ultimaAbertura);

            if (dtFluxo.Rows.Count > 0)
            {
                decimal TotalEntrada = 0; decimal TotalSaida = 0; decimal Saldo = 0; decimal TotalGastos = 0; decimal TotalSuprimento = 0;

                for (int i = 0; i < dtFluxo.Rows.Count; i++)
                {
                    if (dtFluxo.Rows[i]["Tipo"].Equals(1))
                        TotalEntrada += Convert.ToDecimal(dtFluxo.Rows[i]["ValorAposGastos"].ToString().ToList());

                    if (dtFluxo.Rows[i]["Tipo"].Equals(1))
                        TotalSaida += Convert.ToDecimal(dtFluxo.Rows[i]["ValorGastos"].ToString().ToList());

                    //if (dtFluxo.Rows[i]["MoviObse"].ToString().Contains("Gastos"))
                    //    TotalGastos += Convert.ToDecimal(dtFluxo.Rows[i]["MoviValo"].ToString());

                    //if (dtFluxo.Rows[i]["MoviObse"].ToString().Contains("Suprimento"))
                    //    lblEntradas.Text += Convert.ToDecimal(dtFluxo.Rows[i]["MoviValo"].ToString());

                    Saldo = TotalEntrada - TotalSaida;
                }

                lblTotalEntradas.Text = (TotalEntrada - TotalSuprimento).ToString("N2");
                lblTotalSaidas.Text = (TotalSaida - TotalGastos).ToString("N2");
                lblSaldo.Text = Saldo.ToString("N2");
                lblGastos.Text = TotalGastos.ToString("N2");
                lblSuprimento.Text = TotalSuprimento.ToString("N2");

                btnFecharCaixa.Enabled = true;
            }
            else
            {
                MessageBox.Show("Nenhum resultado encontrado!", "Informação do sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }


        #endregion

        private void btnFecharCaixa_Click(object sender, EventArgs e)
        {
            int ultimaAbertura = caixaNegocios.VerificarSeCaixaEstaAberto();
            if (ultimaAbertura == 0)
            {
                MessageBox.Show("Caixa já se encontra fechado seu nóia", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            DataRow drUltimaAbertura = caixaNegocios.PesquisarPorCodigo(ultimaAbertura).Rows[0];
            if (drUltimaAbertura != null)
                try
                {
                    DataTable dtFluxo = gastosNegocios.PesquisarMovimentosCaixa(Convert.ToDateTime(lblAbertura.Text), Convert.ToDateTime(lblFechamento.Text), ultimaAbertura);

                    // Atualizando o campo "Fechamento" na tabela Caixa com a data atual
                    DateTime dataFechamento = DateTime.Now;

                    // Inicializando as variáveis de totais
                    decimal TotalEntrada = 0;
                    decimal TotalSaida = 0;
                    decimal Saldo = 0;
                    decimal TotalGastos = 0;
                    decimal TotalSuprimento = 0;

                     // Lógica para percorrer as linhas do DataTable e somar os valores de acordo com as condições
                    for (int i = 1; i < dtFluxo.Rows.Count; i++)
                    {
                        // Verificando se o Tipo é igual a 0 e somando os valores
                        if (dtFluxo.Rows[i]["Tipo"] != null) // Tipo igual a 0 para Entrada
                        {
                            TotalEntrada += Convert.ToDecimal(dtFluxo.Rows[i]["ValorAposGastos"].ToString());
                        }

                        if (dtFluxo.Rows[i]["Tipo"] != null) // Tipo igual a 0 para Saída
                        {
                        TotalSaida += Convert.ToDecimal(dtFluxo.Rows[i]["ValorGastos"].ToString());
                        }

                        // Cálculo do saldo
                        Saldo = TotalEntrada - TotalSaida;
                    }
                    int g = 0;
                    lblTotalEntradas.Text = (TotalEntrada - TotalSuprimento).ToString("N2");
                    lblTotalSaidas.Text = (TotalSaida - TotalGastos).ToString("N2");
                    lblSaldo.Text = Saldo.ToString("N2");
                    lblGastos.Text = TotalGastos.ToString("N2");
                    lblSuprimento.Text = TotalSuprimento.ToString("N2");
                    btnFecharCaixa.Enabled = true;

                    int maiorCaixaId = dtFluxo.AsEnumerable()
                                              .Max(row => row.Field<int>("CaixaId"));

                    Caixa caixa = caixaNegocios.PesquisarSaldoCaixaFXD(maiorCaixaId);
                    caixaNegocios.Alterar(caixa);

                    // Mensagem de sucesso, se necessário
                    MessageBox.Show("Caixa fechado com sucesso!", "Informação", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            catch (Exception ex)
                {
                // Em caso de erro, exibir mensagem
                MessageBox.Show($"Erro ao fechar o caixa: {ex.Message}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
        }


        private void FrmFecharCaixa_Load(object sender, EventArgs e)
        {
            CarregarCampos();
        }
    }
}
