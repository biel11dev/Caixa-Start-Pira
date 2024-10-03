using PDV.AcessoBancoDados;
using PDV.Negocios;
using PDV.ObjetoTransferencia;
using System;
using System.Data;
using System.Windows.Forms;

namespace PDV.Apresentacao.MovimentacaoCaixa
{
    public partial class FrmGastos : Form
    {
        public FrmGastos()
        {
            InitializeComponent();
        }

        #region Instâncias

        GastosNegocios gastosNegocios = new GastosNegocios();
        CaixaNegocios caixaNegocios = new CaixaNegocios();

        #endregion

        #region Propriedades

        public enumGastosOuSuprimento tipoOperacao { get; set; }

        #endregion

        #region Variáveis

        DataRow drUltimaAbertura;
        DataTable dtGrid = new DataTable();

        #endregion

        #region Método

        private void CarregarGrid()
        {
            try
            {
                dtGrid = gastosNegocios.CarregarGrid(DateTime.Now.AddYears(-1), DateTime.Now);
                grid.DataSource = dtGrid;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Erro ao tentar carregar as gastoss realizadas!\n\n" + ex.Message, "Aviso do Sistema", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void PesquisarSaldoEmCaixa()
        {
            int ultimaAbertura = caixaNegocios.VerificarSeCaixaEstaAberto();

            if (ultimaAbertura > 0)
            {
                drUltimaAbertura = caixaNegocios.PesquisarPorCodigo(ultimaAbertura).Rows[0];
                if (drUltimaAbertura != null)
                {
                    txtSaldoAtual.Text = drUltimaAbertura["Valor"].ToString();
                }
            }
            else
            {
                MessageBox.Show("Caixa fechado!\n\nPor favor efetue a abertura do caixa!", "Aviso do Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);
                txtSaldoAtual.Enabled = false;
                txtValorGastos.Enabled = false;
                txtSaldoApos.Enabled = false;
                txtObservacao.Enabled = false;
                btnSalvar.Enabled = false;
            }
        }


        #endregion

        private void FrmGastos_Load(object sender, EventArgs e)
        {
            try
            {
                PesquisarSaldoEmCaixa();
                CarregarGrid();

                txtValorGastos.Focus();
                if (tipoOperacao == enumGastosOuSuprimento.Gastos)
                {
                    lblValor.Text = "Valor da gastos"; 
                    lblSaldoApos.Text = "Saldo após gastos";
                    this.Text = "Retirar dinheiro do Caixa";
                }
                else
                {
                    lblValor.Text = "Valor de suprimento";
                    lblSaldoApos.Text = "Saldo após suprimento";
                    this.Text = "Entrar com dinheiro no Caixa";
                }
            }
            catch
            { }
        }

        private void txtSaldoAtual_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsNumber(e.KeyChar) || e.KeyChar.ToString() == "\b" || e.KeyChar.ToString().Equals(","))
                base.OnKeyPress(e);
            else
                e.Handled = true;
        }

        private void txtValorGastos_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsNumber(e.KeyChar) || e.KeyChar.ToString() == "\b" || e.KeyChar.ToString().Equals(","))
                base.OnKeyPress(e);
            else
                e.Handled = true;
        }

        private void txtSaldoApos_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsNumber(e.KeyChar) || e.KeyChar.ToString() == "\b" || e.KeyChar.ToString().Equals(","))
                base.OnKeyPress(e);
            else
                e.Handled = true;
        }

        private void btnSalvar_Click(object sender, EventArgs e)
        {
            if (tipoOperacao == enumGastosOuSuprimento.Gastos)
            {
                if (Convert.ToDecimal(txtSaldoAtual.Text) >= Convert.ToDecimal(txtValorGastos.Text))
                {
                    if (MessageBox.Show("Confirma a gastos(retirada) em dinheiro do caixa no valor de R$ " + txtValorGastos.Text + ".", "Pergunta do Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        Gastos gastos = new Gastos();
                        gastos.CaixaId = Convert.ToInt32(drUltimaAbertura["CaixaId"]);
                        gastos.UsuarioId = FrmLogin.usuarioId;
                        gastos.ValorCaixa = Convert.ToDecimal(txtSaldoAtual.Text);
                        gastos.ValorGastos = Convert.ToDecimal(txtValorGastos.Text);
                        gastos.ValorAposGastos = Convert.ToDecimal(txtSaldoApos.Text);
                        gastos.DataHora = DateTime.Now;
                        gastos.Tipo = 1;//Gastos
                        if (txtObservacao.Text.Trim().Length > 0)
                            gastos.Observacao = "Gastos - " + txtObservacao.Text;
                        else
                            gastos.Observacao = "Gastos";


                        gastosNegocios.Inserir(gastos);

                        //Alterando o saldo do caixa
                        ObjetoTransferencia.Caixa caixa = new ObjetoTransferencia.Caixa();
                        caixa.CaixaId = Convert.ToInt32(drUltimaAbertura["CaixaId"]);
                        caixa.Valor = Convert.ToDecimal(txtSaldoApos.Text);
                        caixaNegocios.AlterarSaldo(caixa);

                        MessageBox.Show("Operação realizada com sucesso!", "Aviso do Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        txtSaldoAtual.Clear();
                        txtValorGastos.Clear();
                        txtSaldoApos.Clear();

                        PesquisarSaldoEmCaixa();
                        CarregarGrid();

                        txtValorGastos.Select();
                        txtValorGastos.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Não é possível fazer uma gastos(retirada) maior do que o valor do caixa!", "Aviso do Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtValorGastos.SelectAll();
                    txtValorGastos.Focus();
                }
            }
            else
            {
                if (txtValorGastos.Text != "0,00")
                {
                    if (MessageBox.Show("Confirma o suprimento em dinheiro no caixa no valor de R$ " + txtValorGastos.Text + "!", "Pergunta do Sistema", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        Gastos gastos = new Gastos();
                        gastos.CaixaId = Convert.ToInt32(drUltimaAbertura["CaixaId"]);
                        gastos.UsuarioId = FrmLogin.usuarioId;
                        gastos.ValorCaixa = Convert.ToDecimal(txtSaldoAtual.Text);
                        gastos.ValorGastos = Convert.ToDecimal(txtValorGastos.Text);
                        gastos.ValorAposGastos = Convert.ToDecimal(txtSaldoApos.Text);
                        gastos.DataHora = DateTime.Now;
                        gastos.Tipo = 2; //Suprimento
                        if (txtObservacao.Text.Trim().Length > 0)
                            gastos.Observacao = "Suprimento - " + txtObservacao.Text;
                        else
                            gastos.Observacao = "Suprimento";

                        gastosNegocios.Inserir(gastos);

                        //Alterando o saldo do caixa
                        ObjetoTransferencia.Caixa caixa = new ObjetoTransferencia.Caixa();
                        caixa.CaixaId = Convert.ToInt32(drUltimaAbertura["CaixaId"]);
                        caixa.Valor = Convert.ToDecimal(txtSaldoApos.Text);
                        caixaNegocios.AlterarSaldo(caixa);

                        MessageBox.Show("Operação realizada com sucesso!", "Aviso do Sistema", MessageBoxButtons.OK, MessageBoxIcon.Information);

                        txtSaldoAtual.Clear();
                        txtValorGastos.Clear();
                        txtSaldoApos.Clear();

                        PesquisarSaldoEmCaixa();
                        CarregarGrid();

                        txtValorGastos.Select();
                        txtValorGastos.Focus();
                    }
                }
                else
                {
                    MessageBox.Show("Informe um valor para o suprimento(entrada) do caixa!", "Aviso do Sistema", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    txtValorGastos.Focus();
                }
            }
        }

        private void txtValorGastos_Leave(object sender, EventArgs e)
        {
            try
            {
                if (tipoOperacao == enumGastosOuSuprimento.Gastos)
                {
                    if (txtValorGastos.Text != "0,00")
                        txtSaldoApos.Text = (Convert.ToDecimal(txtSaldoAtual.Text) - Convert.ToDecimal(txtValorGastos.Text)).ToString("N2");
                    else
                        txtValorGastos.Text = "0,00";
                }
                else
                {
                    if (txtValorGastos.Text != "0,00")
                        txtSaldoApos.Text = (Convert.ToDecimal(txtSaldoAtual.Text) + Convert.ToDecimal(txtValorGastos.Text)).ToString("N2");
                    else
                        txtValorGastos.Text = "0,00";
                }
            }
            catch
            { }
        }

        private void FrmGastos_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
            else if (e.KeyCode == Keys.F5 && btnSalvar.Enabled)
            {
                btnSalvar_Click(sender, e);
            }
        }

        private void txtObservacao_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                btnSalvar.Focus();
            }
        }
    }
}
