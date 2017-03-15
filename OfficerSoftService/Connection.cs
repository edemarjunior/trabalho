using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.IO;

namespace OfficerSoftService
{
    class Connection
    {
        Email emails = new Email();
        static String conexao = "";
        MySqlConnection banco;
        String bancoOfficer = "server=localhost; port=3306; Uid=root; database=officersoft_bkp;Pwd=1234";



        public String conectar()
        {
            String teste = "";
            try
            {
                conexao = bancoOfficer;
                banco = new MySqlConnection(conexao);
                banco.Open();
                String sql = "select codigo_cli, login_cli as login, emails_not_cli as email, master_ip_cli as ip_master, " +
                    "master_porta_cli as porta_master, master_usuario_cli as usuario_master," +
                    " master_senha_cli as senha_master, group_concat(codigo_csl) as codigo_slave , group_concat(ip_csl) as ip_slave ,group_concat(porta_csl)" +
                    " as porta_slave, group_concat(usuario_csl) as usuario_slave, group_concat(senha_csl) as senha_slave from cliente " +
                    "left join cliente_slave on codigo_cli = codcli_csl where ativo_cli = 1 group by codigo_cli;";

                MySqlCommand slqComamand = new MySqlCommand(sql, banco);
                MySqlDataReader dtreader = slqComamand.ExecuteReader();

                while (dtreader.Read())//Enquanto existir dados no select
                {

                    String[] infoMaster = new String[2];
                    String[] infoSlave = new string[2];
                    String[] cod_slave = dtreader["codigo_slave"].ToString().Split(',');
                    String[] ip_master = dtreader["ip_slave"].ToString().Split(',');
                    String[] porta_master = dtreader["porta_slave"].ToString().Split(',');
                    String[] senha_master = dtreader["senha_slave"].ToString().Split(',');
                    String[] usuario_master = dtreader["usuario_slave"].ToString().Split(',');
                    String email = dtreader["email"].ToString();

                    infoMaster = retornaTransacao(dtreader["ip_master"].ToString(), dtreader["porta_master"].ToString()
                        , dtreader["usuario_master"].ToString(), dtreader["senha_master"].ToString());

                    for (int i = 0; i < ip_master.Length; i++)
                    {
                        infoSlave = retornaTransacao(ip_master[i], porta_master[i], usuario_master[i], senha_master[i]);
                        insertLog(int.Parse(dtreader["codigo_cli"].ToString()), int.Parse(cod_slave[i]), infoMaster, infoSlave, email);
                    }

                }

                banco.Close();

            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
            }
            return teste;
        }


        public String[] retornaTransacao(String ip, String porta, String usuario, String senha)
        {
            String[] info = new String[2];
            try
            {

                conexao = "server=" + ip + "; port=" + porta + "; Uid=" + usuario + "; database= rtdpj; Pwd=" + senha;
                banco = new MySqlConnection(conexao);
                banco.Open();
                String sql = "select dathor_tra as data, (select count(codigo_pfr) from pessoafr) as quantPfr from transacao order by 1 desc limit 1;";
                MySqlCommand slqComamand = new MySqlCommand(sql, banco);

                MySqlDataReader dtreader = slqComamand.ExecuteReader();

                while (dtreader.Read())//Enquanto existir dados no select
                {

                    DateTime data = DateTime.Parse(dtreader["data"].ToString());
                    info[0] = data.ToString("yyyy-MM-dd HH:mm:ss");
                    info[1] = dtreader["quantPfr"].ToString();

                }
                banco.Close();
                dtreader.Close();
                return info;

            }
            catch (Exception E)
            {
                Console.WriteLine(E.Message);
                info = null;
                return info;
            }

        }


        public void insertLog(int codCliente, int codSlave, String[] infoMaster, String[] infoSlave, String email)
        {
            conexao = bancoOfficer;
            String status_slave = "";
            banco = new MySqlConnection(conexao);
            banco.Open();
            String sql = "SHOW STATUS LIKE 'Slave_running';";
            MySqlCommand slqComamand = new MySqlCommand(sql, banco);
            MySqlDataReader dtreader = slqComamand.ExecuteReader();

            while (dtreader.Read())
            {
                status_slave = dtreader["Value"].ToString();
            }
            dtreader.Close();

            string hora = DateTime.Now.ToString("HH:mm:ss");
            string data = DateTime.Now.ToString("yyyy-MM-dd");
            int status = 0;
            TimeSpan ts = new TimeSpan();
            if (infoMaster != null || infoSlave != null)
            {
                DateTime dataMaster = DateTime.Parse(infoMaster[0]);
                DateTime dataSlave = DateTime.Parse(infoSlave[0]);
                ts = dataMaster - dataSlave;
            }


            if (infoMaster == null || infoSlave == null
                || String.Equals(status_slave, "off",
                StringComparison.OrdinalIgnoreCase)
                || (int.Parse(infoMaster[1]) - int.Parse(infoSlave[1])) > 15
                || ts.TotalMinutes < -5)
            {
                status = 1;
            }
            if (infoMaster == null)
            {
                status = 1;
            }

            sql = "insert into log (codcli_bkl, dathor_bkl, master_ulttra_bkl, master_totpfr_bkl, " +
                "slave_id_bkl, slave_running_bkl, slave_ulttra_bkl, slave_totpfr_bkl, status_bkl) " +
                "values (@codigo_cli, @dathor_bkl, @master_ulttra_bkl, @master_totpfr_bkl, " +
                "@slave_id_bkl, @slave_running_bkl, @slave_ulttra_bkl, @slave_totpfr_bkl, @status_bkl)";

            slqComamand = new MySqlCommand(sql, banco);
            slqComamand.Parameters.AddWithValue("@codigo_cli", codCliente);
            slqComamand.Parameters.AddWithValue("@dathor_bkl", data + " " + hora);
            if (infoMaster != null)
            {
                slqComamand.Parameters.AddWithValue("@master_ulttra_bkl", infoMaster[0]);
                slqComamand.Parameters.AddWithValue("@master_totpfr_bkl", infoMaster[1]);
            }
            else
            {
                slqComamand.Parameters.AddWithValue("@master_ulttra_bkl", "0000-00-00 00:00:00");
                slqComamand.Parameters.AddWithValue("@master_totpfr_bkl", 0);
            }

            slqComamand.Parameters.AddWithValue("@slave_id_bkl", codSlave);
            slqComamand.Parameters.AddWithValue("@slave_running_bkl", status_slave);
            if (infoSlave != null)
            {
                slqComamand.Parameters.AddWithValue("@slave_ulttra_bkl", infoSlave[0]);
                slqComamand.Parameters.AddWithValue("@slave_totpfr_bkl", infoSlave[1]);
            }
            else
            {
                slqComamand.Parameters.AddWithValue("@slave_ulttra_bkl", "0000-00-00 00:00:00");
                slqComamand.Parameters.AddWithValue("@slave_totpfr_bkl", 0);
            }

            slqComamand.Parameters.AddWithValue("@status_bkl", status);
            slqComamand.ExecuteNonQuery();
            slqComamand.Dispose();
            banco.Close();
            emails.enviarEmail(email, codCliente, status);
        }


        public long Size(System.IO.DirectoryInfo dirInfo)
        {
            long total = 0;

            //Obtem o tamanho total dos arquivos no diretório
            foreach (System.IO.FileInfo file in dirInfo.GetFiles())
                total += file.Length;

            //Obtem o tamanho total dos sub-diretórios da pasta 
            foreach (System.IO.DirectoryInfo dir in dirInfo.GetDirectories())
                total += Size(dir);

            return total;
        }

        public void insereTamBanco()
        {
            try
            {


                long tamanho;
                banco = new MySqlConnection(bancoOfficer);
                banco.Open();
                String sql = "select codigo_cli, local_data_cli as diretorio " +
                    "from cliente where ativo_cli= 1;";
                MySqlCommand slqComamand = new MySqlCommand(sql, banco);
                MySqlDataReader dtreader = slqComamand.ExecuteReader();

                while (dtreader.Read())
                {
                    tamanho = (Size(new DirectoryInfo(@dtreader["diretorio"].ToString())) / 1024) / 1024;
                    sql = "insert into cliente_bd_log (codcli_cbl, dathor_cbl, tamanho_bkl)" +
                            "  values(@codigo, @data, @tamanho); ";

                    MySqlConnection banco2 = new MySqlConnection(bancoOfficer);
                    banco2.Open();
                    MySqlCommand slqComamand2 = new MySqlCommand(sql, banco2);
                    slqComamand2.Parameters.AddWithValue("@codigo", dtreader["codigo_cli"].ToString());
                    slqComamand2.Parameters.AddWithValue("@data", DateTime.Now.ToString("yyyy-MM-dd") + " " + DateTime.Now.ToString("HH:mm:ss"));
                    slqComamand2.Parameters.AddWithValue("@tamanho", tamanho + " MB");
                    slqComamand2.ExecuteNonQuery();

                }
                slqComamand.Dispose();
                banco.Close();
            }
            catch (Exception E)
            {
                StreamWriter vWriter = new StreamWriter(@"c:\OfficerSoftServico.txt", true);
                vWriter.WriteLine(E.Message);
                vWriter.Flush();
                vWriter.Close();
            }
        }
    }
}


