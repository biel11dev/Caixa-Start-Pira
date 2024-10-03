using PDV.AcessoBancoDados;
using PDV.ObjetoTransferencia;
using PFSoftwares.AcessoBancoDados;
using System;
using System.Collections.Generic;
using System.Data;

namespace PDV.Negocios
{
    public class GastosNegocios
    {
        #region Instancias

        Conexao conexao = new Conexao();

        #endregion

        #region Variáveis Default

        string nomeTabela = "Gastos";

        string sqlDefault = "SELECT GastosId,CaixaId,S.UsuarioId,U.NomeLogin AS Usuario,ValorCaixa,ValorGastos,ValorAposGastos,DataHora,CASE WHEN Tipo='1' THEN 'Gastos' ELSE 'Suprimento' END AS Tipo,Observacao FROM Gastos S LEFT JOIN Usuarios U ON U.UsuarioId =S.UsuarioId ";

        #endregion

        #region Métodos Publicos

        public Boolean Inserir(Gastos gastos)
        {
            return conexao.Inserir(nomeTabela, PreencheParametros(gastos));
        }

        public Boolean Alterar(Gastos gastos)
        {
            return conexao.Atualizar(nomeTabela, PreencheParametros(gastos), PreencheCondicoes(gastos));
        }

        public Boolean Excluir(Gastos gastos)
        {
            return conexao.Excluir(nomeTabela, PreencheCondicoes(gastos));
        }

        public DataTable PesquisarPorCodigo(int caixaId)
        {
            return conexao.Pesquisar(string.Format("{0} WHERE CaixaId = {1}", sqlDefault, caixaId));
        }

        public DataTable CarregarGrid(DateTime dataInicio, DateTime dataFim)
        {
            return conexao.Pesquisar(string.Format("{0} WHERE DataHora BETWEEN '{1} 00:00:00' AND '{2} 23:59:59' ORDER BY DataHora DESC", sqlDefault, dataInicio.ToString("yyyy-MM-dd"), dataFim.ToString("yyyy-MM-dd")));
        }

        public DataTable PesquisarMovimentosCaixa(DateTime? abertura, DateTime? fechamento, int caixaId)
        {
            string sql = sqlDefault; ;

            if (caixaId > 0 && abertura != null)
            {
                sql += ValidarString(sql) + string.Format("DataHora between '{0}' and '{1}'", abertura.Value.ToString("yyyy-MM-dd 00:00:00"), fechamento.Value.ToString("yyyy-MM-dd 23:59:59"));
            }

            sql += " ORDER BY DataHora";

            return conexao.Pesquisar(sql);
        }

        private string ValidarString(string str)
        {
            if (str.Contains("WHERE"))
                return " AND ";
            else
                return " WHERE ";
        }

        #endregion 

        #region Metodos Privados

        private List<SqlParametros> PreencheParametros(Gastos gastos)
        {
            List<SqlParametros> lstParametros = new List<SqlParametros>();

            if (gastos.CaixaId > 0)
                lstParametros.Add(new SqlParametros("CaixaId", gastos.CaixaId));
            if (gastos.UsuarioId > 0)
                lstParametros.Add(new SqlParametros("UsuarioId", gastos.UsuarioId));
            lstParametros.Add(new SqlParametros("ValorCaixa", gastos.ValorCaixa.ToString().Replace(".", "").Replace(",", ".")));
            lstParametros.Add(new SqlParametros("ValorGastos", gastos.ValorGastos.ToString().Replace(".", "").Replace(",", ".")));
            lstParametros.Add(new SqlParametros("ValorAposGastos", gastos.ValorAposGastos.ToString().Replace(".", "").Replace(",", ".")));
            lstParametros.Add(new SqlParametros("DataHora", gastos.DataHora.ToString("yyyy-MM-dd HH:mm:ss")));
            lstParametros.Add(new SqlParametros("Tipo", gastos.Tipo));
            lstParametros.Add(new SqlParametros("Observacao", gastos.Observacao));

            return lstParametros;
        }

        private List<SqlParametros> PreencheCondicoes(Gastos gastos)
        {
            List<SqlParametros> lstParametrosCondicionais = new List<SqlParametros>();

            lstParametrosCondicionais.Add(new SqlParametros("GastosId", gastos.GastosId));

            return lstParametrosCondicionais;
        }

        #endregion

    }
}
