﻿using System;
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
using System.Security.Cryptography.X509Certificates;
//using System.Web.Http;

namespace IntegracaoApiNetCore6.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class VendaController : Controller
    {
        //private readonly IVendaRepository _repository;
        //public VendaController(IVendaRepository repository)
        //{
        //    _repository = repository ??
        //     throw new ArgumentNullException(nameof(repository));
        //}

        ProtonRetailView protonView = new ProtonRetailView(0);
        /// <summary>
        /// Consulta venda do dia de hoje tanto no Retail quanto no servidor de loja e exibe a diferença entre os valores. Recebe o centro como parâmetro.
        /// </summary>
        /// <param name="centro"></param>
        /// <returns></returns>
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
        [Route("porCentroData")]
        public VendaPorLojaModel VendaPorCentroData(int Centro, string DataInicial, string DataFinal)
        {


            protonView.VendaPorLoja(DataInicial, DataFinal);
            VendaPorLojaModel vendaPorLoja = protonView.listaVendaPorLoja.Where(n => n.Centro == Centro)
                                                .Select(n => n).FirstOrDefault();
                                                


            return vendaPorLoja;
        }


        [HttpGet]
        [Route("geralPorData")]
        public IActionResult GetGeralPorData(string DataInicial, string DataFinal)
        {
            protonView.VendaPorLoja(DataInicial, DataFinal);
            return Ok(protonView.listaVendaPorLoja);
        }

        [HttpGet]
        [Route("porUfData")]
        public IActionResult GetPorUfData(string Uf, string DataInicial, string DataFinal)
        {
            protonView.VendaPorDataUf(DataInicial, DataFinal, Uf);
            return Ok(protonView.listaVendaPorLoja);
        }

        [HttpGet]
        [Route("porRegionalData")]
        public IActionResult GetPorRegionalData(string Regional, string DataInicial, string DataFinal)
        {
            protonView.VendaPorDataRegional(DataInicial, DataFinal, Regional);
            return Ok(protonView.listaVendaPorLoja);
        }
    }
}
