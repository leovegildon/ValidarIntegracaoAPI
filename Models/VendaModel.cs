using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace IntegracaoApiNetCore6.Models
{
    public class VendaModel
    {
        private int centro;
        private string vendaRetail;
        private string vendaFirebird;
        private string nomeCentro;
        private string data;
        private string diferenca;
        private string excecao;
 
 
        public VendaModel(int centro, string nomeCentro, string Data, string vendaRetail, string vendaFirebird, string diferenca, string excecao)
        {
            this.Centro = centro;
            this.NomeCentro = nomeCentro;
            this.Data = data;
            this.VendaRetail = vendaRetail;
            this.VendaFirebird = vendaFirebird;
            this.Diferenca = diferenca;
            this.excecao = excecao;
        }
 
        public int Centro
        {
            get
            {
                return centro;
            }
 
            set
            {
                centro = value;
            }
        }
        public string NomeCentro
        {
            get
            {
                return nomeCentro;
            }

            set
            {
                nomeCentro = value;
            }
        }
        public string Data
        {
            get
            {
                return data;
            }

            set
            {
                data = value;
            }
        }
        public string VendaRetail
        {
            get
            {
                return vendaRetail;
            }
 
            set
            {
                vendaRetail = value;
            }
        }
 
        public string VendaFirebird
        {
            get
            {
                return vendaFirebird;
            }
 
            set
            {
                vendaFirebird = value;
            }
        }
        public string Diferenca
        {
            get
            {
                return diferenca;
            }

            set
            {
                diferenca = value;
            }
        }

        public string Excecao
        {
            get
            {
                return excecao;
            }

            set
            {
                excecao = value;
            }
        }


    }
    
}