using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Web.Administration;

namespace TestingIIsAdmin
{
    class Program
    {
        static void Main(string[] args)
        {

            Console.WriteLine("Indique la IP con la que desea trabajar: ");
            string IP = Console.ReadLine();

            if (string.IsNullOrEmpty(IP))
            {
                Console.WriteLine("No se ingreso una IP, por tanto termino el programa. Gracias por hacerme perder mi tiempo ¬¬");
                return;
            }

            //Probar si la IP es una IP valida
            if (IsValidateIP(IP) == false)
            {
                Console.WriteLine("Tss, ingresaste una IP valida. Gracias por hacerme perder mi tiempo ¬¬");
                return;
            }

            //Preguntamos si se quiere agregar o eliminar
            Console.WriteLine("Deseas eliminar(1) o agregar (2) la IP?");
            string opcionEliminar = Console.ReadLine();

            bool agregar = opcionEliminar == "2";

            using (ServerManager serverManager = new ServerManager())
            {
                Configuration configuration = serverManager.GetApplicationHostConfiguration();

                List<string> sitiosIIs = new List<string>() { "adminEcommerce", "clientesEcomm", "disponibilidadecom", "exploracionecom", "menuecom", "notificacionesecom", "pedidosecom" };



                foreach (string sitioIis in sitiosIIs)
                {
                    ConfigurationSection ipSecuritySection = configuration.GetSection("system.webServer/security/ipSecurity", sitioIis);
                    ConfigurationElementCollection ipSecurityCollection = ipSecuritySection.GetCollection();

                    ConfigurationElement existingElement = ipSecurityCollection.FirstOrDefault(x => x.Attributes["ipAddress"].Value.ToString() == IP);

                    //Si quiere agregar y no existe el elemento, lo agrego
                    if (agregar && existingElement == null)
                    {
                        ConfigurationElement addElement = ipSecurityCollection.CreateElement("add");
                        addElement["ipAddress"] = IP;
                        addElement["allowed"] = true;
                        Console.WriteLine($"[ADD] Se agrega el elemento {IP} en el sitio: {sitioIis}");
                        ipSecurityCollection.Add(addElement);
                    }
                    else
                    {
                        ConfigurationElement removeElement = ipSecurityCollection.FirstOrDefault(x => x.Attributes["ipAddress"].Value.ToString() == IP);

                        //Si ya esta, lo elimino
                        if (removeElement != null)
                        {
                            Console.WriteLine($"[DELETE] Se encuentra el elemento {IP} en el sitio: {sitioIis}, se remueve");
                            ipSecurityCollection.Remove(removeElement);
                        }
                    }
                }

                serverManager.CommitChanges();
            }

            
        }

        public static bool IsValidateIP(string Address)
        {
            //Match pattern for IP address    
            string Pattern = @"^([1-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])(\.([0-9]|[1-9][0-9]|1[0-9][0-9]|2[0-4][0-9]|25[0-5])){3}$";
            //Regular Expression object    
            Regex check = new Regex(Pattern);

            //check to make sure an ip address was provided    
            if (string.IsNullOrEmpty(Address))
                //returns false if IP is not provided    
                return false;
            else
                //Matching the pattern    
                return check.IsMatch(Address, 0);
        }
    }
}
