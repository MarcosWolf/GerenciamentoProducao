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
    public class SQLiteConnector
    {
        private SQLiteConnection? _connection;
        public string? databasePath;

        public SQLiteConnector()
        {
            _connection = null;
            databasePath = null;
        }

        public void CreateConnection()
        {
            _connection = new SQLiteConnection(@"Data Source=C:\arquivo.db;Version=3;");
        }

        public void OpenConnection()
        {
            if (_connection != null && _connection.State != System.Data.ConnectionState.Open)
            {
                _connection.Open();
            }
        }

        public void CloseConnection()
        {
            if (_connection != null && _connection.State != System.Data.ConnectionState.Closed)
            {
                _connection.Close();
            }
        }

        public SQLiteConnection? GetConnection()
        {
            return _connection;
        }

        public void ExecuteQuery(string query)
        {
            try
            {
                OpenConnection();
                if (_connection != null)
                {
                    var command = _connection.CreateCommand();
                    command.CommandText = query;
                    command.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Console.Clear();
                Console.WriteLine("Erro ao execultar consulta: " + ex.Message);
            }
            finally
            {
                CloseConnection();
            }
        }
    }

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

        public static void InterfaceManager(char menuChoice, SQLiteConnector connector)
        {
            string[] listArray = { "Registrar Ordem", "Listar Ordens", "Visualizar Relatório" };
            int auxChoice = (int)Char.GetNumericValue(menuChoice);
            bool repeat = false;

            do
            {
                if (auxChoice >= 1 && auxChoice <= 3)
                {
                    InterfaceHeader();
                    Console.Title = listArray[auxChoice - 1];

                    try
                    {
                        if (auxChoice == 1)
                        {
                            //RegisterOrder(connector);
                        }
                    }
                    catch (Exception)
                    {
                        Console.WriteLine("Erro");
                        repeat = true;
                    }
                }
            } while (repeat);

            Console.ReadKey();
        }

        static void Main()
        {
            SQLiteConnector connector = new();
            connector.CreateConnection();

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