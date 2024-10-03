using System;

namespace PDV.ObjetoTransferencia
{
    public class Gastos
    {
        public int GastosId { get; set; }
        public int CaixaId { get; set; }
        public int UsuarioId { get; set; }
        public decimal ValorCaixa { get; set; }
        public decimal ValorGastos { get; set; }
        public decimal ValorAposGastos { get; set; }
        public DateTime DataHora { get; set; }
        public int Tipo { get; set; }
        public string Observacao { get; set; }
    }
}
