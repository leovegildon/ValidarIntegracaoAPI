﻿using FirebirdSql.Data.FirebirdClient;
using IntegracaoApiNetCore6.Controllers;
using IntegracaoApiNetCore6.Models;
using Oracle.ManagedDataAccess.Client;
using System.Data;
using System.Drawing;
using System.Text.RegularExpressions;
using ValidarIntegracaoAPI.Models;

namespace ValidarIntegracaoAPI
{
    public class ProtonRetailView
    {
        public ProtonRetailView() { }
        public ProtonRetailView(int CentroConsulta)
        {
            this.CentroConsulta = CentroConsulta;
        }
        public List<VendaModel> listaVenda = new List<VendaModel>();
        public List<VendaPorLojaModel> listaVendaPorLoja = new List<VendaPorLojaModel>();

        public int CentroConsulta { get; }

        #region //VendaPorCentroFirebirdRetail
        public void VendaPorCentro(int centroConsulta)
        {
            listaVenda.Clear();
            string vendaOracle = "NULO";
            String vendaFirebird = "NULO";
            string data = "";
            string localBanco = "10.11.0.30";
            string ip = "";
            string nomeCentro = "";
            string centroOracle = "";
            string diferenca = "";
            decimal VendaFireBird;

            //PEGANDO OS DADOS DAS LOJAS A SEREM CONSULTADAS
            OracleConnection conn1 = new OracleConnection("Data Source=(DESCRIPTION="
                                            + "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + localBanco + ")(PORT=1521)))"
                                            + "(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=INTEGRACAOSP)));"
                                            + "User Id=pdvuser;Password=pdv1234");
            OracleCommand oCmd1 = new OracleCommand();
            string query1 = "SELECT s.loja_sap_pk CENTRO, " +
                           "u.tund_fantasia LOJA, " +
                           "s.host_servidor " +
                           "FROM tloja_sap s " +
                           "join TUND_UNIDADE u on s.loja_proton_uk = u.tund_unidade_pk " +
                           "where s.loja_sap_pk = " + centroConsulta;
            oCmd1.CommandText = query1;
            oCmd1.CommandType = CommandType.Text;
            oCmd1.Connection = conn1;
            conn1.Open();
            OracleDataReader ler1 = oCmd1.ExecuteReader();
            while (ler1.Read())
            {
                centroOracle = ler1.GetValue(0).ToString();
                nomeCentro = ler1.GetValue(1).ToString();
                ip = ler1.GetValue(2).ToString();
            }
            conn1.Close();

            //CONSULTA NO ORACLE
            OracleConnection conn = new OracleConnection("Data Source=(DESCRIPTION="
                                                        + "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + localBanco + ")(PORT=1521)))"
                                                        + "(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=INTEGRACAOSP)));"
                                                        + "User Id=pdvuser;Password=pdv1234");
            conn.Open();
            OracleCommand oCmd = new OracleCommand();
            oCmd.Connection = conn;
            string query = "SELECT sum(t.valor_liquido) venda " +
                                "FROM TTRANSACAO t " +
                                "join tloja_sap s " +
                                "on t.numero_loja_uk = s.loja_proton_uk " +
                                "WHERE TRUNC(t.data_hora_transacao_uk) = trunc(sysdate) " +
                                "and t.tipo in ('P', 'N', 'S', 'M') " +
                                "and t.cancelada = 'N' " +
                                "and not exists " +
                                "(select * " +
                                "from ttransacao tt " +
                                "where tt.numero_loja_uk = t.numero_loja_uk " +
                                "and tt.numero_transacao_uk = t.numero_transacao_uk " +
                                "and tt.numero_pdv_uk = t.numero_pdv_uk " +
                                "and tt.numero_serie_impressora_uk = t.numero_serie_impressora_uk " +
                                "and tt.data_hora_transacao_uk = t.data_hora_transacao_uk " +
                                "and tt.tipo = t.tipo " +
                                "and tt.cancelada = 'S') " +
                                "and s.loja_sap_pk = " + centroConsulta;
            //oCmd.Parameters.AddWithValue(":CENTROORA", 1006);
            oCmd.CommandText = query;
            oCmd.CommandType = CommandType.Text;
            OracleDataReader ler = oCmd.ExecuteReader();
            while (ler.Read())
            {
                vendaOracle = ler.GetValue(0).ToString();
            }
            //::::::::::::::::::::::::::::::::::::::::::::::::::::
            // TRATAMENTO DE EXCEÇÃO DO ORACLE
            //::::::::::::::::::::::::::::::::::::::::::::::::::::
            //Caso não tenha havido venda no dia consultado
            //na loja consultada, o Oracle retorna 'null' e
            //dessa forma na conversão para decimal abaixo irá,
            //ocorrer uma exceção. Para isso, o if abaixo irá
            //converter o valor para 0 caso retorne 'null' na
            //consulta do firebird.
            //::::::::::::::::::::::::::::::::::::::::::::::::::::
            if (vendaOracle == "")
            {
                vendaOracle = "0";
            }
            conn.Close();

            //CONSULTA NO FIREBIRD
            FbConnection fbConn = new FbConnection("DataSource=" + ip + "; Database=" + "C:\\proton\\pdv-server\\dat\\DBSRV.gdb" + "; User=SYSDBA; Password=masterkey; Connection lifetime=15");
            FbCommand cmdF = new FbCommand();
            fbConn.Open();
            cmdF.Connection = fbConn;
            cmdF.CommandText = "select sum(a.tped_valor_liquido_pedido) " +
                              "from tped_pedido_venda a " +
                              "where a.tped_data_pedido = 'today' " +
                              "and a.tped_status_pedido <> 'CA' " +
                              "and a.tped_tipo_venda in ('N','P','S','M')";
            FbDataReader lerF = cmdF.ExecuteReader();
            while (lerF.Read())
            {
                vendaFirebird = lerF.GetValue(0).ToString();
            }
            //::::::::::::::::::::::::::::::::::::::::::::::::::::
            // TRATAMENTO DE EXCEÇÃO DO FIREBIRD
            //::::::::::::::::::::::::::::::::::::::::::::::::::::
            //Caso não tenha havido venda no dia consultado
            //na loja consultada, o firebird retorna 'null' e
            //dessa forma na conversão para decimal abaixo irá,
            //ocorrer uma exceção. Para isso, o if abaixo irá
            //converter o valor para 0 caso retorne 'null' na
            //consulta do firebird.
            //::::::::::::::::::::::::::::::::::::::::::::::::::::
            if (vendaFirebird == "")
            {
                vendaFirebird = "0";
            }
            fbConn.Close();
            //VendaFireBird = Convert.ToDecimal(vendaFirebird);
            //vendaFirebird = VendaFireBird.ToString("#.##");
            diferenca = Convert.ToString(Convert.ToDecimal(vendaFirebird) - Convert.ToDecimal(vendaOracle));
            listaVenda.Add(new VendaModel(0, "", "", "", "", "", "") { Centro = centroConsulta, NomeCentro = nomeCentro, Data = DateTime.Now.ToString(), VendaRetail = vendaOracle, VendaFirebird = vendaFirebird, Diferenca = diferenca });
        }
        #endregion

        #region //VendaPorCentroGeral
        public void VendaPorCentroGeral(string dataIni, string dataFim)
        {
            listaVenda.Clear();
            string vendaOracle = "NULO";
            string vendaFirebird = "NULO";
            string data = "";
            string localBanco = "10.11.0.30";
            string ip = "";
            string valor;
            string nomeCentro = "";
            string centroOracle = "";
            decimal diferenca = 0;
            decimal VendaFireBird;
            string excecao = "OK";
            string centro = "0", loja = "0", host_servidor = "0";
            List<string> Lojas = new List<string>();
            List<string> Centros = new List<string>();
            string[] lojasArray;
            string[] centrosArray = new string[2];

            //Populando a lista de lojas com os centros que estão ativos no Retail
            OracleConnection conn1 = new OracleConnection("Data Source=(DESCRIPTION="
                                            + "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + localBanco + ")(PORT=1521)))"
                                            + "(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=INTEGRACAOSP)));"
                                            + "User Id=pdvuser;Password=pdv1234");
            OracleCommand oCmd1 = new OracleCommand();
            string query1 = "SELECT S.LOJA_SAP_PK, " +
                            "s.loja_sap_pk || ' - ' || u.tund_fantasia LOJA," +
                           "s.host_servidor " +
                           "FROM tloja_sap s " +
                           "join TUND_UNIDADE u on s.loja_proton_uk = u.tund_unidade_pk " +
                           "where s.ativa_difusao = 'S' " +
                           "order by 1";
            oCmd1.CommandText = query1;
            oCmd1.CommandType = CommandType.Text;
            oCmd1.Connection = conn1;

            conn1.Open();
            OracleDataReader ler1 = oCmd1.ExecuteReader();

            while (ler1.Read())
            {

                Lojas.Add(ler1.GetValue(1).ToString());
                Centros.Add(ler1.GetValue(0).ToString());
            }
            conn1.Close();

            lojasArray = Lojas.ToArray();
            centrosArray = Centros.ToArray();

            //centrosArray[0] = "1006";
            //centrosArray[1] = "1010";

            //============================
            // INICIO DO LAÇO FOR
            //============================
            for (int i = 0; i < centrosArray.Length; i++)
            {

                valor = "0.00";
                vendaOracle = "0.00";
                diferenca = 0;
                nomeCentro = "";
                excecao = "OK";
                //PEGANDO OS DADOS DAS LOJAS A SEREM CONSULTADAS
                OracleConnection conn2 = new OracleConnection("Data Source=(DESCRIPTION="
                                            + "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + localBanco + ")(PORT=1521)))"
                                            + "(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=INTEGRACAOSP)));"
                                            + "User Id=pdvuser;Password=pdv1234");
                OracleCommand oCmd2 = new OracleCommand();
                string query2 = "SELECT s.loja_sap_pk CENTRO, " +
                               "u.tund_fantasia LOJA, " +
                               "s.host_servidor " +
                               "FROM tloja_sap s " +
                               "join TUND_UNIDADE u on s.loja_proton_uk = u.tund_unidade_pk " +
                               "where s.loja_sap_pk = '"+ centrosArray[i]  + "'";
                oCmd2.CommandText = query2;
                oCmd2.CommandType = CommandType.Text;
                oCmd2.Connection = conn2;

                    conn2.Open();
                    OracleDataReader ler2 = oCmd2.ExecuteReader();
                    while (ler2.Read())
                    {
                        centro = ler2.GetValue(0).ToString();
                        loja = ler2.GetValue(1).ToString();
                        host_servidor = ler2.GetValue(2).ToString();
                    }
                    conn2.Close();

                nomeCentro = centro + " - " + loja;

                //CONSULTA NO ORACLE
                OracleConnection conn = new OracleConnection("Data Source=(DESCRIPTION="
                                            + "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + localBanco + ")(PORT=1521)))"
                                            + "(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=INTEGRACAOSP)));"
                                            + "User Id=pdvuser;Password=pdv1234"); 
                OracleCommand oCmd = new OracleCommand();
                string query = "SELECT sum(t.valor_liquido) venda " +
                                "FROM TTRANSACAO t " +
                                "join tloja_sap s " +
                                "on t.numero_loja_uk = s.loja_proton_uk " +
                                "WHERE TRUNC(t.data_hora_transacao_uk) between '"+ dataIni +"' and '"+ dataFim +"' " +
                                "and t.tipo in ('P', 'N', 'S', 'M') " +
                                "and t.cancelada = 'N' " +
                                "and not exists " +
                                "(select * " +
                                "from ttransacao tt " +
                                "where tt.numero_loja_uk = t.numero_loja_uk " +
                                "and tt.numero_transacao_uk = t.numero_transacao_uk " +
                                "and tt.numero_pdv_uk = t.numero_pdv_uk " +
                                "and tt.numero_serie_impressora_uk = t.numero_serie_impressora_uk " +
                                "and tt.data_hora_transacao_uk = t.data_hora_transacao_uk " +
                                "and tt.tipo = t.tipo " +
                                "and tt.cancelada = 'S') " +
                                "and s.loja_sap_pk = '" + centrosArray[i] + "'";

                oCmd.CommandText = query;
                oCmd.CommandType = CommandType.Text;
                oCmd.Connection = conn;

                    conn.Open();
                    OracleDataReader ler = oCmd.ExecuteReader();
                    while (ler.Read())
                    {
                        vendaOracle = ler.GetValue(0).ToString();
                    }
                    //::::::::::::::::::::::::::::::::::::::::::::::::::::
                    // TRATAMENTO DE EXCEÇÃO DO ORACLE
                    //::::::::::::::::::::::::::::::::::::::::::::::::::::
                    //Caso não tenha havido venda no dia consultado
                    //na loja consultada, o Oracle retorna 'null' e
                    //dessa forma na conversão para decimal abaixo irá,
                    //ocorrer uma exceção. Para isso, o if abaixo irá
                    //converter o valor para 0 caso retorne 'null' na
                    //consulta do firebird.
                    //::::::::::::::::::::::::::::::::::::::::::::::::::::
                    if (vendaOracle == "")
                    {
                        vendaOracle = "0";
                    }
                    conn.Close();


                //CONSULTA NO FIREBIRD

                string dataIniFb;
                string dataFimFb;

                dataIniFb = dataIni.Substring(0, 2) + "." + dataIni.Substring(2,2) + "." + dataIni.Substring(4,4);
                dataFimFb = dataFim.Substring(0, 2) + "." + dataFim.Substring(2, 2) + "." + dataFim.Substring(4, 4);
                try
                {
                    FbConnection fbConn = new FbConnection("DataSource=" + host_servidor + "; Database=" + "C:\\proton\\pdv-server\\dat\\DBSRV.gdb" + "; User=SYSDBA; Password=masterkey; Connection lifetime=15");
                    FbCommand cmd = new FbCommand();
                    fbConn.Open();
                    cmd.Connection = fbConn;
                    cmd.CommandText = "select sum(a.tped_valor_liquido_pedido) " +
                                      "from tped_pedido_venda a " +
                                      "where a.tped_data_pedido between '" + dataIniFb + "' and '" + dataFimFb + "' " +
                                      "and a.tped_status_pedido <> 'CA' " +
                                      "and a.tped_tipo_venda in ('N','P','S','M')";
                    FbDataReader lerFb = cmd.ExecuteReader();
                    while (lerFb.Read())
                    {
                        vendaFirebird = lerFb.GetValue(0).ToString();
                    }
                    //::::::::::::::::::::::::::::::::::::::::::::::::::::
                    // TRATAMENTO DE EXCEÇÃO DO FIREBIRD
                    //::::::::::::::::::::::::::::::::::::::::::::::::::::
                    //Caso não tenha havido venda no dia consultado
                    //na loja consultada, o firebird retorna 'null' e
                    //dessa forma na conversão para decimal abaixo irá,
                    //ocorrer uma exceção. Para isso, o if abaixo irá
                    //converter o valor para 0 caso retorne 'null' na
                    //consulta do firebird.
                    //::::::::::::::::::::::::::::::::::::::::::::::::::::
                    if (valor == "")
                    {
                        valor = "0";
                    }
                    fbConn.Close();
                    diferenca = Convert.ToDecimal(vendaFirebird) - Convert.ToDecimal(vendaOracle);

                    fbConn.Close();
                }
                catch(Exception exFb)
                {
                    excecao = exFb.Message;
                }
                listaVenda.Add(new VendaModel(0, "", "", "", "", "", "") 
                { 
                    Centro = Convert.ToInt32(centro), 
                    NomeCentro = nomeCentro, 
                    Data = DateTime.Now.ToString(), 
                    VendaRetail = vendaOracle, 
                    VendaFirebird = vendaFirebird, 
                    Diferenca = diferenca.ToString(),
                    Excecao = excecao
                });

            }//Fim For

        }
        #endregion

        #region //VendaPorLoja
        public void VendaPorLoja(string dataInicial, string dataFinal)
        {
            listaVendaPorLoja.Clear();
            string localBanco = "10.11.0.30";

        OracleConnection conn1 = new OracleConnection("Data Source=(DESCRIPTION="
                                            + "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + localBanco + ")(PORT=1521)))"
                                            + "(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=INTEGRACAOSP)));"
                                            + "User Id=pdvuser;Password=pdv1234");
            OracleCommand oCmd1 = new OracleCommand();
            string query1 = "SELECT s.loja_sap_pk   CENTRO, " +
               "u.tund_fantasia LOJA, " +
               "f.tloc_regiao as REGIONAL, " +
               "c.tloc_uf_fk as UF, " +
               "to_char(t.data_hora_transacao_uk, 'DD/MM/YYYY') AS DATA, " +
               "count(t.valor_liquido) as QTD_TICKETS, " +
               "sum(t.valor_liquido) as venda " +
          "FROM tloja_sap s " +
         "right join ttransacao t " +
            "on s.loja_proton_uk = t.numero_loja_uk " +
         "right join TUND_UNIDADE u " +
            "on s.loja_proton_uk = u.tund_unidade_pk " +
          "join tloc_cidade_cep c " +
            "on u.tund_cidade_cep_fk = c.tloc_cidade_cep_pk " +
          "join tloc_uf f " +
            "on c.tloc_uf_fk = f.tloc_uf_pk " +
         "WHERE TRUNC(t.data_hora_transacao_uk) between '" + dataInicial + "' and '" + dataFinal + "' " +
           " and t.tipo in ('N', 'S', 'M') " +
           "and t.cancelada = 'N' " +
           "and s.loja_sap_pk not like '9%' " +
           "and not exists " +
         "(select * " +
                  "from ttransacao tt " +
                 "where tt.numero_loja_uk = t.numero_loja_uk " +
                   "and tt.numero_transacao_uk = t.numero_transacao_uk " +
                   "and tt.numero_pdv_uk = t.numero_pdv_uk " +
                   "and tt.numero_serie_impressora_uk = t.numero_serie_impressora_uk " +
                   "and tt.data_hora_transacao_uk = t.data_hora_transacao_uk " +
                   "and tt.tipo = t.tipo " +
                   "and tt.cancelada = 'S') " +
         "GROUP BY LOJA_SAP_PK, u.tund_fantasia, f.tloc_regiao, c.tloc_uf_fk, to_char(t.data_hora_transacao_uk, 'DD/MM/YYYY') " +
         "ORDER BY 1, tund_fantasia";
            oCmd1.CommandText = query1;
            oCmd1.CommandType = CommandType.Text;
            oCmd1.Connection = conn1;
            conn1.Open();
            OracleDataReader ler1 = oCmd1.ExecuteReader();
            while (ler1.Read())
            {
                listaVendaPorLoja.Add(new VendaPorLojaModel(0, "", "", "", "", 0, 0) 
                { Centro = Convert.ToInt32(ler1.GetValue(0)),
                    Loja = ler1.GetValue(1).ToString(),
                    Regional = ler1.GetValue(2).ToString(),
                    Uf = ler1.GetValue(3).ToString(),
                    Data = ler1.GetValue(4).ToString(),
                    QtdTickets = Convert.ToInt32(ler1.GetValue(5)), 
                    Venda = Convert.ToInt32(ler1.GetValue(6))
                });

            }
            conn1.Close();
        }
        #endregion

        #region //VendaPorDataUf
        public void VendaPorDataUf(string dataInicial, string dataFinal, string Uf)
        {
            listaVendaPorLoja.Clear();
            string localBanco = "10.11.0.30";

            OracleConnection conn1 = new OracleConnection("Data Source=(DESCRIPTION="
                                                + "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + localBanco + ")(PORT=1521)))"
                                                + "(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=INTEGRACAOSP)));"
                                                + "User Id=pdvuser;Password=pdv1234");
            OracleCommand oCmd1 = new OracleCommand();
            string query1 = "SELECT s.loja_sap_pk   CENTRO, " +
               "u.tund_fantasia LOJA, " +
               "f.tloc_regiao as REGIONAL, " +
               "c.tloc_uf_fk as UF, " +
               "to_char(t.data_hora_transacao_uk, 'DD/MM/YYYY') AS DATA, " +
               "count(t.valor_liquido) as QTD_TICKETS, " +
               "sum(t.valor_liquido) as venda " +
          "FROM tloja_sap s " +
         "right join ttransacao t " +
            "on s.loja_proton_uk = t.numero_loja_uk " +
         "right join TUND_UNIDADE u " +
            "on s.loja_proton_uk = u.tund_unidade_pk " +
          "join tloc_cidade_cep c " +
            "on u.tund_cidade_cep_fk = c.tloc_cidade_cep_pk " +
          "join tloc_uf f " +
            "on c.tloc_uf_fk = f.tloc_uf_pk " +
         "WHERE TRUNC(t.data_hora_transacao_uk) between '" + dataInicial + "' and '" + dataFinal + "' " +
           " and t.tipo in ('N', 'S', 'M') " +
           "and f.tloc_uf_pk = '" + Uf + "'" +
           "and t.cancelada = 'N' " +
           "and s.loja_sap_pk not like '9%' " +
           "and not exists " +
         "(select * " +
                  "from ttransacao tt " +
                 "where tt.numero_loja_uk = t.numero_loja_uk " +
                   "and tt.numero_transacao_uk = t.numero_transacao_uk " +
                   "and tt.numero_pdv_uk = t.numero_pdv_uk " +
                   "and tt.numero_serie_impressora_uk = t.numero_serie_impressora_uk " +
                   "and tt.data_hora_transacao_uk = t.data_hora_transacao_uk " +
                   "and tt.tipo = t.tipo " +
                   "and tt.cancelada = 'S') " +
         "GROUP BY LOJA_SAP_PK, u.tund_fantasia, f.tloc_regiao, c.tloc_uf_fk, to_char(t.data_hora_transacao_uk, 'DD/MM/YYYY') " +
         "ORDER BY 1, tund_fantasia";
            oCmd1.CommandText = query1;
            oCmd1.CommandType = CommandType.Text;
            oCmd1.Connection = conn1;
            conn1.Open();
            OracleDataReader ler1 = oCmd1.ExecuteReader();
            while (ler1.Read())
            {
                listaVendaPorLoja.Add(new VendaPorLojaModel(0, "", "", "", "", 0, 0)
                {
                    Centro = Convert.ToInt32(ler1.GetValue(0)),
                    Loja = ler1.GetValue(1).ToString(),
                    Regional = ler1.GetValue(2).ToString(),
                    Uf = ler1.GetValue(3).ToString(),
                    Data = ler1.GetValue(4).ToString(),
                    QtdTickets = Convert.ToInt32(ler1.GetValue(5)),
                    Venda = Convert.ToInt32(ler1.GetValue(6))
                });

            }
            conn1.Close();
        }
        #endregion

        #region //VendaPorDataRegional
        public void VendaPorDataRegional(string dataInicial, string dataFinal, string regional)
        {
            listaVendaPorLoja.Clear();
            string localBanco = "10.11.0.30";

            OracleConnection conn1 = new OracleConnection("Data Source=(DESCRIPTION="
                                                + "(ADDRESS_LIST=(ADDRESS=(PROTOCOL=TCP)(HOST=" + localBanco + ")(PORT=1521)))"
                                                + "(CONNECT_DATA=(SERVER=DEDICATED)(SERVICE_NAME=INTEGRACAOSP)));"
                                                + "User Id=pdvuser;Password=pdv1234");
            OracleCommand oCmd1 = new OracleCommand();
            string query1 = "SELECT s.loja_sap_pk   CENTRO, " +
               "u.tund_fantasia LOJA, " +
               "f.tloc_regiao as REGIONAL, " +
               "c.tloc_uf_fk as UF, " +
               "to_char(t.data_hora_transacao_uk, 'DD/MM/YYYY') AS DATA, " +
               "count(t.valor_liquido) as QTD_TICKETS, " +
               "sum(t.valor_liquido) as venda " +
          "FROM tloja_sap s " +
         "right join ttransacao t " +
            "on s.loja_proton_uk = t.numero_loja_uk " +
         "right join TUND_UNIDADE u " +
            "on s.loja_proton_uk = u.tund_unidade_pk " +
          "join tloc_cidade_cep c " +
            "on u.tund_cidade_cep_fk = c.tloc_cidade_cep_pk " +
          "join tloc_uf f " +
            "on c.tloc_uf_fk = f.tloc_uf_pk " +
         "WHERE TRUNC(t.data_hora_transacao_uk) between '" + dataInicial + "' and '" + dataFinal + "' " +
           " and t.tipo in ('N', 'S', 'M') " +
           "and f.tloc_regiao like '" + regional + "%'" +
           "and t.cancelada = 'N' " +
           "and s.loja_sap_pk not like '9%' " +
           "and not exists " +
         "(select * " +
                  "from ttransacao tt " +
                 "where tt.numero_loja_uk = t.numero_loja_uk " +
                   "and tt.numero_transacao_uk = t.numero_transacao_uk " +
                   "and tt.numero_pdv_uk = t.numero_pdv_uk " +
                   "and tt.numero_serie_impressora_uk = t.numero_serie_impressora_uk " +
                   "and tt.data_hora_transacao_uk = t.data_hora_transacao_uk " +
                   "and tt.tipo = t.tipo " +
                   "and tt.cancelada = 'S') " +
         "GROUP BY LOJA_SAP_PK, u.tund_fantasia, f.tloc_regiao, c.tloc_uf_fk, to_char(t.data_hora_transacao_uk, 'DD/MM/YYYY') " +
         "ORDER BY 1, tund_fantasia";
            oCmd1.CommandText = query1;
            oCmd1.CommandType = CommandType.Text;
            oCmd1.Connection = conn1;
            conn1.Open();
            OracleDataReader ler1 = oCmd1.ExecuteReader();
            while (ler1.Read())
            {
                listaVendaPorLoja.Add(new VendaPorLojaModel(0, "", "", "", "", 0, 0)
                {
                    Centro = Convert.ToInt32(ler1.GetValue(0)),
                    Loja = ler1.GetValue(1).ToString(),
                    Regional = ler1.GetValue(2).ToString(),
                    Uf = ler1.GetValue(3).ToString(),
                    Data = ler1.GetValue(4).ToString(),
                    QtdTickets = Convert.ToInt32(ler1.GetValue(5)),
                    Venda = Convert.ToInt32(ler1.GetValue(6))
                });

            }
            conn1.Close();
        }
        #endregion
    }
}
