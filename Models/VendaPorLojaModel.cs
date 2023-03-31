namespace ValidarIntegracaoAPI.Models
{
    public class VendaPorLojaModel
    {

        private int centro;
        private string loja;
        private string regional;
        private string uf;
        private string data;
        private int qtdTickets;
        private decimal venda;

        public VendaPorLojaModel(int centro, string loja, string regional, string uf, string data, int qtdTickets, decimal venda)
        {
            this.Centro = centro;
            this.Loja = loja;
            this.Regional = regional;
            this.Uf = uf;
            this.Data = data;
            this.QtdTickets = qtdTickets;
            this.Venda = venda;
        }

        public int Centro { get => centro; set => centro = value; }
        public string Loja { get => loja; set => loja = value; }
        public string Regional { get => regional; set => regional = value; }
        public string Uf { get => uf; set => uf = value; }
        public string Data { get => data; set => data = value; }
        public int QtdTickets { get => qtdTickets; set => qtdTickets = value; }
        public decimal Venda { get => venda; set => venda = value; }
    }
}
