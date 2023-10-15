using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Globalization;
using System.Data.SQLite;
using System.Xml;

namespace GerenciamentoProducao
{
    internal class Program
    {
        public static string GetDate()
        {
            DateTime currentDate = DateTime.Now;
            string formatedDate = currentDate.ToString("dd/MM/yyyy");
            return formatedDate;
        }

        public static void InterfaceHeader()
        {
            string currentDate = GetDate();

            Console.Clear();
            Console.WriteLine("+------------------------------------------------------------+------------+");
            Console.WriteLine($"! Gerenciamento de Ordens de Produção                        | {currentDate} !");
            Console.WriteLine("+------------------------------------------------------------+------------+");
        }

        static void Main()
        {
            do
            {
                Console.Title = "Menu Principal";
                InterfaceHeader();
                Console.WriteLine("| 1 - Registrar Ordem                                                     |");
                Console.WriteLine("| 2 - Listar Ordens                                                       |");
                Console.WriteLine("| 3 - Visualizar Relatório                                                |");
                Console.WriteLine("| 0 - Sair                                                                |");
                Console.WriteLine("+-------------------------------------------------------------------------+");
                Console.WriteLine("! Selecione uma opção:                                                    !");
                Console.WriteLine("+-------------------------------------------------------------------------+");
                Console.SetCursorPosition(23, 8);

            } while (true);
        }
    }
}