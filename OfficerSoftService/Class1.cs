using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using System.Net.Mail;

namespace OfficerSoftService
{
    class Connection
    {
        static String conexao = "";
        MySqlConnection banco; 
        

        public String conectar()
        {
            String teste = "";
            try
            {
                conexao = "server=localhost:3306; Uid=root; database=officersoft_bkp;Pwd=1234";
                banco = new MySqlConnection(conexao);
                banco.Open();
                String sql = "select codigo_cli, login_cli as login, master_ip_cli as ip_master, " +
                    "master_porta_cli as porta_master, master_usuario_cli as usuario_master," +
                    " master_senha_cli as senha_master, group_concat(ip_csl) as ip_slave ,group_concat(porta_csl)" +
                    " as porta_slave, group_concat(usuario_csl) as usuario_slave, group_concat(senha_csl) as senha_slave from cliente" +
                    "left join cliente_slave on codigo_cli = codcli_csl where ativo_cli = group by codigo_cli;";
                                                                                                                                                             
                MySqlCommand slqComamand = new MySqlCommand(sql, banco);
                MySqlDataReader dtreader = slqComamand.ExecuteReader();
                
                while (dtreader.Read())//Enquanto existir dados no select
                {
                    

                    String[] ip_master = dtreader["ip_slave"].ToString().Split(',');
                    String[] porta_master = dtreader["porta_slave"].ToString().Split(',');
                    String[] senha_master = dtreader["usuario_slave"].ToString().Split(',');
                    String[] usuario_master = dtreader["senha_slave"].ToString().Split(',');

                    for(int i = 0; i < ip_master.Length; i++)
                    {

                    }

                }

                    banco.Close();

            }
            catch
            {
                teste = "  gybcec";
            }
            return teste;
        }

        public String[] retornaTransacao(String ip, String porta, String usuario, String senha)
        {
            
                conexao = "server=" + ip + ":" + porta + "; Uid=" + usuario + "; database= rtdpj; Pwd=" + senha;
            String[] info = new String[2];
            banco = new MySqlConnection(conexao);
                String sql = "select dathor_tra as data, (select count(codigo_pfr) from pessoafr) as quantPfr from transacao order by 1 desc limit 1;";
                MySqlCommand slqComamand = new MySqlCommand(sql, banco);
                MySqlDataReader dtreader2 = slqComamand.ExecuteReader();

                while (dtreader2.Read())//Enquanto existir dados no select
                {
                    
                    DateTime data = DateTime.Parse(dtreader2["data"].ToString());
                    info[0] = data.ToString("yyyy-MM-dd HH:mm:ss");
                    info[1] = dtreader2["quantPfr"].ToString();
                    
                }
            return info;

        }

        }
    }

