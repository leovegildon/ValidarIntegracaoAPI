using System;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using IntegracaoApiNetCore6.Models;
using System.Data;
using Oracle.ManagedDataAccess.Client;
using FirebirdSql.Data.FirebirdClient;
using Microsoft.AspNetCore.Mvc;
using ValidarIntegracaoAPI;
using ValidarIntegracaoAPI.Models;
//using System.Web.Http;

namespace IntegracaoApiNetCore6.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class VendaController : Controller
    {

        ProtonRetailView protonView = new ProtonRetailView(0);
        
        [HttpGet]
        [Route("porcentro")]
        public VendaModel VendaPorCentro(int centro)
        {
            protonView.VendaPorCentro(centro);
            VendaModel venda = protonView.listaVenda.Where(n => n.Centro == centro)
                                                .Select(n => n)
                                                .FirstOrDefault();


            if (centro == null)
            {
                throw new System.Web.Http.HttpResponseException(HttpStatusCode.NotFound);
            }

            return venda;
        }

        [HttpGet]
        [Route("porUF")]
        public VendaPorLojaModel VendaPorUF(string UF, string DataInicial, string DataFinal)
        {
            protonView.VendaPorLoja(DataInicial, DataFinal);
            VendaPorLojaModel vendaPorLoja = protonView.listaVendaPorLoja.Where(n => n.Uf == UF)
                                                .Select(n => n)
                                                .FirstOrDefault();


            return vendaPorLoja;
        }
    }
}
