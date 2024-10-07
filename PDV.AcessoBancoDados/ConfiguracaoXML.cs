using System;
using System.Data;
using System.Security.Cryptography;
using System.Text;

namespace PDV.AcessoBancoDados
{
    public class ConfiguracaoXML
    {
        #region INSTANCIAS

        DataSet dtSet = new DataSet();
        DataTable dtTable = new DataTable();
        string patch = AppDomain.CurrentDomain.BaseDirectory + @"\Config.xml";

        #endregion

        #region PROPRIEDADES

        public string Servidor { get; set; }

        public string BancoDados { get; set; }

        public string Usuario { get; set; }

        public string Senha { get; set; }

        #endregion

        #region MÉTODOS

        public void LerConfiguracaoBanco()
        {
            dtSet.ReadXml(patch);

            if (dtSet.Tables[0].Rows.Count > 0)
            {
                Senha = dtSet.Tables[0].Rows[0]["Senha"].ToString();
                Servidor = dtSet.Tables[0].Rows[0]["Servidor"].ToString();
                BancoDados = dtSet.Tables[0].Rows[0]["BancoDados"].ToString();
                Usuario = dtSet.Tables[0].Rows[0]["Usuario"].ToString();
            }
        }

        #endregion
    }
}
