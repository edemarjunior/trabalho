using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace OfficerSoftService
{
    class Email
    {
        private Dictionary<string, string> infoEmail = new Dictionary<string, string>();
    private MySqlConnection banco;
    private String bancoOfficer = "server=localhost; port=3306; Uid=root; database=officersoft_bkp;Pwd=1234";

    public void enviarEmail(String email, int codCliente, int status)
    {

        if (verificaEmailErro(status, codCliente))
        {
            buscaDadosEmail();

            MailMessage mail = new MailMessage();
            mail.To.Add(new MailAddress(infoEmail["notificacao_email_remetente"].ToString()));
            mail.From = new MailAddress(email);
            mail.Subject = infoEmail["notificacao_negativa_assunto"].ToString();

            mail.Body = infoEmail["notificacao_negativa_email"].ToString();
            mail.IsBodyHtml = true;
            SmtpClient SmtpServer = new SmtpClient(infoEmail["notificacao_servidor_smtp"].ToString(), 587);
            using (SmtpServer)
            {
                SmtpServer.Credentials = new System.Net.NetworkCredential(
                   infoEmail["notificacao_servidor_login"].ToString(), infoEmail["notificacao_servidor_senha"].ToString());
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);
            }

            String[] emailsCol = infoEmail["notificacao_email_colaboradores"].ToString().Split();
            for (int i = 0; i < emailsCol.Length; i++)
            {
                mail = new MailMessage();
                mail.To.Add(new MailAddress(infoEmail["notificacao_email_remetente"].ToString()));
                mail.From = new MailAddress(emailsCol[i]);
                mail.Subject = infoEmail["notificacao_negativa_assunto"].ToString();

                mail.Body = infoEmail["notificacao_negativa_email"].ToString();
                mail.IsBodyHtml = true;
                SmtpServer = new SmtpClient(infoEmail["notificacao_servidor_smtp"].ToString(), 587);
                using (SmtpServer)
                {
                    SmtpServer.Credentials = new System.Net.NetworkCredential(
                       infoEmail["notificacao_servidor_login"].ToString(), infoEmail["notificacao_servidor_senha"].ToString());
                    SmtpServer.EnableSsl = true;
                    SmtpServer.Send(mail);
                }
            }
        }
        if (verificaEmailRetorno(status, codCliente))
        {
            buscaDadosEmail();

            MailMessage mail = new MailMessage();
            mail.To.Add(new MailAddress(infoEmail["notificacao_email_remetente"].ToString()));
            mail.From = new MailAddress(email);
            mail.Subject = infoEmail["notificacao_positiva_assunto"].ToString();

            mail.Body = infoEmail["notificacao_positiva_email"].ToString();
            mail.IsBodyHtml = true;
            SmtpClient SmtpServer = new SmtpClient(infoEmail["notificacao_servidor_smtp"].ToString(), 587);
            using (SmtpServer)
            {
                SmtpServer.Credentials = new System.Net.NetworkCredential(
                   infoEmail["notificacao_servidor_login"].ToString(), infoEmail["notificacao_servidor_senha"].ToString());
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);
            }

            String[] emailsCol = infoEmail["notificacao_email_colaboradores"].ToString().Split(',');
            for (int i = 0; i < emailsCol.Length; i++)
            {
                mail = new MailMessage();
                mail.To.Add(new MailAddress(infoEmail["notificacao_email_remetente"].ToString()));
                mail.From = new MailAddress(emailsCol[i]);
                mail.Subject = infoEmail["notificacao_positiva_assunto"].ToString();

                mail.Body = infoEmail["notificacao_positiva_email"].ToString();
                mail.IsBodyHtml = true;
                SmtpServer = new SmtpClient(infoEmail["notificacao_servidor_smtp"].ToString(), 587);
                using (SmtpServer)
                {
                    SmtpServer.Credentials = new System.Net.NetworkCredential(
                       infoEmail["notificacao_servidor_login"].ToString(), infoEmail["notificacao_servidor_senha"].ToString());
                    SmtpServer.EnableSsl = true;
                    SmtpServer.Send(mail);
                }
            }
        }
    }

    public void buscaDadosEmail()
    {
        try
        {
            banco = new MySqlConnection(bancoOfficer);
            banco.Open();
            String sql = "select variavel_con as descri, valor_con as valor from config";

            MySqlCommand slqComamand = new MySqlCommand(sql, banco);
            MySqlDataReader dtreader = slqComamand.ExecuteReader();

            while (dtreader.Read())//Enquanto existir dados no select
            {

                infoEmail[dtreader["descri"].ToString()] = dtreader["valor"].ToString();

            }

            banco.Close();
        }
        catch (Exception E)
        {
            Console.WriteLine(E.Message);
        }
    }

    public bool verificaEmailRetorno(int status, int codCliente)
    {
        try
        {
            banco = new MySqlConnection(bancoOfficer);
            banco.Open();
            String sql = "select status_bkl,codigo_cel as quant from log left" +
                " join cliente_email_log on codigo_bkl = codlog_cel where codcli_bkl = " +
                " @codCliente order by codigo_bkl desc limit 1";
            MySqlCommand slqComamand = new MySqlCommand(sql, banco);
            slqComamand.Parameters.AddWithValue("@codCliente", codCliente);
            MySqlDataReader dtreader = slqComamand.ExecuteReader();
            int retorno = 0;
            int quant = 0;
            while (dtreader.Read())//Enquanto existir dados no select
            {

                    retorno = dtreader.GetInt32("status_bkl");
                    if (!dtreader.IsDBNull(1))
                        quant = 1;

                }
                banco.Close();
            if (status == 0 && retorno == 1 && quant == 0)
            {
                return true;

            }
            else
            {
                return false;
            }



        }
        catch (Exception E)
        {
            Console.WriteLine(E.Message);
        }
        return false;
    }

    public bool verificaEmailErro(int status, int codCliente)
    {
        try
        {
            banco = new MySqlConnection(bancoOfficer);
            banco.Open();
            String sql = "select status_bkl,codigo_cel as quant from log left" +
                " join cliente_email_log on codigo_bkl = codlog_cel where codcli_bkl = " +
                " @codCliente order by codigo_bkl desc limit 1";
            MySqlCommand slqComamand = new MySqlCommand(sql, banco);
            slqComamand.Parameters.AddWithValue("@codCliente", codCliente);
            MySqlDataReader dtreader = slqComamand.ExecuteReader();
            int retorno = 0;
            int quant = 0;
            while (dtreader.Read())//Enquanto existir dados no select
            {

                retorno = dtreader.GetInt32("status_bkl");
                if (!dtreader.IsDBNull(1))
                    quant = 1;

            }
            banco.Close();
            if (status == 1 && retorno == 0 && quant == 0)
            {
                return true;

            }
            else
            {
                return false;
            }

        }
        catch (Exception E)
        {
            Console.WriteLine(E.Message);
        }
        return false;
    }

}
}
