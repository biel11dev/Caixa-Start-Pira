using PDV.Negocios;
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
        }

        #region Instâncias

        CaixaNegocios caixaNegocios = new CaixaNegocios();
        GastosNegocios gastosNegocios = new GastosNegocios();

        #endregion

        #region Variáveis

        decimal suprimento = 0;

        #endregion

        #region Métodos

        private void CarregarCampos()
        {
            int ultimaAbertura = caixaNegocios.VerificarSeCaixaEstaAberto();
            DataRow drUltimaAbertura = caixaNegocios.PesquisarPorCodigo(ultimaAbertura).Rows[0];
            if (drUltimaAbertura != null)
            {
                lblValorAbertura.Text = drUltimaAbertura["Valor"].ToString();
                lblAbertura.Text = drUltimaAbertura["Abertura"].ToString();
                lblFechamento.Text = DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss");
            }

            DataTable dtFluxo = gastosNegocios.PesquisarMovimentosCaixa(Convert.ToDateTime(lblAbertura.Text), Convert.ToDateTime(lblFechamento.Text), ultimaAbertura);

            if (dtFluxo.Rows.Count > 0)
            {
                decimal TotalEntrada = 0; decimal TotalSaida = 0; decimal Saldo = 0; decimal TotalGastos = 0; decimal TotalSuprimento = 0;

                //for (int i = 0; i < dtFluxo.Rows.Count; i++)
                //{
                //    if (dtFluxo.Rows[i]["Tipo"].Equals(1))
                //        TotalEntrada += Convert.ToDecimal(dtFluxo.Rows[i]["ValorGastos"].ToString());

                //    if (dtFluxo.Rows[i]["Tipo"].Equals(1))
                //        TotalSaida += Convert.ToDecimal(dtFluxo.Rows[i]["ValorGastos"].ToString());

                //    if (dtFluxo.Rows[i]["MoviObse"].ToString().Contains("Gastos"))
                //        TotalGastos += Convert.ToDecimal(dtFluxo.Rows[i]["MoviValo"].ToString());

                //    if (dtFluxo.Rows[i]["MoviObse"].ToString().Contains("Suprimento"))
                //        lblEntradas.Text += Convert.ToDecimal(dtFluxo.Rows[i]["MoviValo"].ToString());

                //    Saldo = TotalEntrada - TotalSaida;
                //}

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

        private void FrmFecharCaixa_Load(object sender, EventArgs e)
        {
            CarregarCampos();
        }
    }
}
