namespace ValidarIntegracaoAPI.Models
{
    public class EventosFiscais
    {
        public DateTime Data { get; set; }

        public string Loja { get; set; }

        public long eventosFirebird { get; set; }

        public long eventosRetail { get; set; }
    }
}