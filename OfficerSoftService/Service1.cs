using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace OfficerSoftService
{
    public partial class Service1 : ServiceBase
    {
        
        System.Threading.Timer timerService;
        System.Threading.Timer timerTamBanco;
        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            timerService = new System.Threading.Timer(new TimerCallback(execusao), null, 0, (10000*60));
            timerTamBanco = new System.Threading.Timer(new TimerCallback(insereTamBanco), null, 0, (3600000*24));
        }

        protected override void OnStop()
        {
            StreamWriter vWriter = new StreamWriter(@"c:\OfficerSoftServico.txt", true);
            vWriter.WriteLine("Servico Parado: " + DateTime.Now.ToString());
            vWriter.Flush();
            vWriter.Close();
        }

        private void execusao(object sender)
        {
            Connection conexao = new Connection();
            conexao.conectar();

        }

        private void insereTamBanco(object sender)
        {
            Connection conexao = new Connection();
            conexao.insereTamBanco();

        }

    }
}
