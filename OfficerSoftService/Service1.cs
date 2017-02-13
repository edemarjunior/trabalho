using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace OfficerSoftService
{
    public partial class Service1 : ServiceBase
    {
        
        System.Threading.Timer timer;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            timer = new System.Threading.Timer(new TimerCallback(execusao), null, 15000, 60000);
        }

        protected override void OnStop()
        {
            StreamWriter vWriter = new StreamWriter(@"c:\testeServico.txt", true);

            vWriter.WriteLine("Servico Parado: " + DateTime.Now.ToString());
            vWriter.Flush();
            vWriter.Close();
        }

        private void execusao(object sender)
        {
            Connection conexao = new Connection();
           
            StreamWriter vWriter = new StreamWriter(@"c:\testeServico.txt", true);
            vWriter.WriteLine("Servico Rodando: " + DateTime.Now.ToString() + " Colaborador: "+conexao.conectar() + " Email enviado");
            enviarEmail();
            vWriter.Flush();
            vWriter.Close();
           
                
            
        }

        public void enviarEmail()
        {
            MailMessage mail = new MailMessage();
            mail.To.Add(new MailAddress("jredemar_sah@hotmail.com"));
            mail.From = new MailAddress("jredemar_sah@hotmail.com");
            mail.Subject = "teste";
            mail.IsBodyHtml = true;
            mail.Body = "oi <br>" +
                "<table>              < thead > < tr > < th > Código </ th > " +
                "< th > Serventia </ th >< th > Nome </ th ></ tr > </ thead >< tbody>" +
                "</ tbody ></ table > ";
            SmtpClient SmtpServer = new SmtpClient("smtp.live.com", 587);
            using (SmtpServer)
            {
                SmtpServer.Credentials = new System.Net.NetworkCredential("jredemar_sah@hotmail.com", "edemarjuniormaur");
                SmtpServer.EnableSsl = true;
                SmtpServer.Send(mail);
            }

            }
        }
}
