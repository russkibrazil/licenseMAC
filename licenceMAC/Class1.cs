using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Management;

namespace licenceMAC
{
    public class Class1
    {
        int test;
        string mac = String.Empty;
        bool falha = false;

        void pegaMac()
        {
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection moc = mc.GetInstances();
            foreach (ManagementObject mo in moc)
            {
                if (mac == string.Empty) {
                    if ((bool)mo["IPEnabled"] == true)
                        mac = mo["IPAddress"].ToString();
                }
                mo.Dispose(); //realmente necessario? Garante somente o primeiro cartao habilitado
            }
        }
        void trocaMascaraMac (string mac, char busca = ':', char troca = '')
        {
            mac = mac.Replace(busca, troca);
        }
        bool testaConexao(string site)
        {
            System.Net.WebRequest requisita = System.Net.WebRequest.Create(site);
            try
            {
                System.Net.WebResponse respo = requisita.GetResponse();
                respo.Close();
                requisita = null;
                return true;
            }
            catch
            {
                requisita = null;
                //manda msg
                return false;
            }
        }
        bool verificaChaveporArquivo(string base_remota, string desch_local, string CrUsr = "", string CrPss = "")
        {
            System.Net.WebClient wc = new System.Net.WebClient();
            System.Net.NetworkCredential cred = new System.Net.NetworkCredential(CrUsr, CrPss);
            wc.Credentials = cred;
            try
            {
                wc.DownloadFile(base_remota, desch_local); //TODO: Pegar endereço da aplicação
            }
            catch
            {
                falha = true;
                //mostra msg
                return false;
            }
            wc.Dispose();
            StreamReader lic = new StreamReader(desch_local);
            string lin;
            bool found = false;
            do
            {
                lin = lic.ReadLine();
                if (lin == mac)
                    found = true;
            } while (lin != null && found == false);
            lic.Close();
            try
            {
                File.Delete(desch_local);
            }
            catch (FileNotFoundException)
            {
                //Faz Nada. ´Já foi removido mesmo...
            }

            if (!found)
            {
                return false;
            }
            return true;
        }
    }

}
