using System.Net;
using IntegracaoApiNetCore6.Models;
using System.Data;
using Microsoft.AspNetCore.Mvc;
using ValidarIntegracaoAPI;
using ValidarIntegracaoAPI.Models;

namespace IntegracaoApiNetCore6.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class VendaController : Controller
    {
        ProtonRetailView protonView = new ProtonRetailView();

        /// <summary>
        /// sdfdsfsdfsdfsdfsdfdsfsdf
        /// </summary>
        /// <param name="centro">Centro a ser consultado</param>
        /// <returns>JSON contendo Data e hora, centro, venda retail, venda no firebird e a diferença</returns>
        /// <exception cref="System.Web.Http.HttpResponseException"></exception>
        [HttpGet]
        [Route("FirebirdRetailporCentro")]
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
        [Route("FirebirdRetailGeralPorData")]
        public IActionResult GetPorRegionalData(string DataInicial, string DataFinal)
        {
            protonView.VendaPorCentroGeral(DataInicial, DataFinal);
            return Ok(protonView.listaVenda);
        }


        [HttpGet]
        [Route("RetailporCentroData")]
        public VendaPorLojaModel VendaPorCentroData(int Centro, string DataInicial, string DataFinal)
        {


            protonView.VendaPorLoja(DataInicial, DataFinal);
            VendaPorLojaModel vendaPorLoja = protonView.listaVendaPorLoja.Where(n => n.Centro == Centro)
                                                .Select(n => n).FirstOrDefault();
                                                


            return vendaPorLoja;
        }



        [HttpGet]
        [Route("RetailGeralPorData")]
        public IActionResult GetGeralPorData(string DataInicial, string DataFinal)
        {
            protonView.VendaPorLoja(DataInicial, DataFinal);
            return Ok(protonView.listaVendaPorLoja);
        }

        [HttpGet]
        [Route("RetailPorUfData")]
        public IActionResult GetPorUfData(string Uf, string DataInicial, string DataFinal)
        {
            protonView.VendaPorDataUf(DataInicial, DataFinal, Uf);
            return Ok(protonView.listaVendaPorLoja);
        }

        [HttpGet]
        [Route("RetailPorRegionalData")]
        public IActionResult GetPorRegionalData(string Regional, string DataInicial, string DataFinal)
        {
            protonView.VendaPorDataRegional(DataInicial, DataFinal, Regional);
            return Ok(protonView.listaVendaPorLoja);
        }
    }
}
