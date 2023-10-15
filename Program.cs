﻿using System;
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
        public static void ClearCurrentConsoleLine(int left, int top)
        {
            Console.SetCursorPosition(left, top);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(left, top);
        }

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

        public static string RO_ValidateProductQuantity(SQLiteConnector connector, int orderId)
        {
            string? input = "";
            int orderQuantity = 0;

            while (string.IsNullOrWhiteSpace(input))
            {
                try
                {
                    Console.SetCursorPosition(14, 7);
                    input = Console.ReadLine();

                    if (input == null && string.IsNullOrWhiteSpace(input))
                    {
                        Console.SetCursorPosition(0, 10);
                        Console.WriteLine("|                                                                         |");
                        Console.SetCursorPosition(2, 10);
                        Console.WriteLine("A entrada não pode estar vazia.");
                        Console.WriteLine("+-------------------------------------------------------------------------+");
                    }
                    else if (!input.All(char.IsDigit))
                    {
                        Console.SetCursorPosition(0, 10);
                        Console.WriteLine("|                                                                         |");
                        Console.SetCursorPosition(2, 10);
                        Console.WriteLine("A entrada deve conter apenas números.");
                        Console.WriteLine("+-------------------------------------------------------------------------+");
                    }
                    else
                    {
                        if (int.TryParse(input, out orderQuantity))
                        {
                            string query = "SELECT * FROM product WHERE product_id = @Id";

                            try
                            {
                                using (var command = new SQLiteCommand(query, connector.GetConnection()))
                                {
                                    command.Parameters.AddWithValue("@Id", orderId);
                                    connector.OpenConnection();

                                    using (SQLiteDataReader reader = command.ExecuteReader())
                                    {
                                        if (reader.Read())
                                        {
                                            // Verificar se quantidade de estoque é insuficiente
                                            int materialQuantity = Convert.ToInt32(reader["product_quantity"]);

                                            if (materialQuantity < orderQuantity)
                                            {
                                                Console.SetCursorPosition(0, 10);
                                                Console.WriteLine("|                                                                         |");
                                                Console.SetCursorPosition(2, 10);
                                                Console.WriteLine("Quantidade de materiais insuficientes.");
                                                Console.WriteLine("+-------------------------------------------------------------------------+");
                                                System.Threading.Thread.Sleep(1000);

                                                Console.SetCursorPosition(0, 7);
                                                Console.WriteLine("| Quantidade:                                                             |");
                                                ClearCurrentConsoleLine(0, 10);
                                                ClearCurrentConsoleLine(0, 11);
                                                input = "";
                                            }
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.Clear();
                                Console.WriteLine("Erro ao realizar consulta: " + ex);
                            }
                            finally
                            {
                                connector.CloseConnection();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Clear();
                    Console.WriteLine("Operação inválida: " + ex);
                }
            }

            return input;
        }

        public static string RO_ValidateProductInput(SQLiteConnector connector)
        {
            string? input = "";
            int orderId = 0;

            while (string.IsNullOrWhiteSpace(input))
            {
                try
                {
                    Console.SetCursorPosition(11, 5);
                    input = Console.ReadLine();

                    if (input == null && string.IsNullOrWhiteSpace(input))
                    {
                        Console.SetCursorPosition(0, 10);
                        Console.WriteLine("|                                                                         |");
                        Console.SetCursorPosition(2, 10);
                        Console.WriteLine("A entrada não pode estar vazia.");
                        Console.WriteLine("+-------------------------------------------------------------------------+");
                    }
                    else if (!input.All(char.IsDigit))
                    {
                        Console.SetCursorPosition(0, 10);
                        Console.WriteLine("|                                                                         |");
                        Console.SetCursorPosition(2, 10);
                        Console.WriteLine("A entrada deve conter apenas números.");
                        Console.WriteLine("+-------------------------------------------------------------------------+");
                        input = "";
                        Console.SetCursorPosition(0, 5);
                        Console.WriteLine("| Produto:                                                                |");
                    }
                    else
                    {
                        if (int.TryParse(input, out orderId))
                        {
                            string query = "SELECT * FROM Product WHERE product_id = @Id";

                            try
                            {
                                using (var command = new SQLiteCommand(query, connector.GetConnection()))
                                {
                                    command.Parameters.AddWithValue("@Id", orderId);
                                    connector.OpenConnection();

                                    using (SQLiteDataReader reader = command.ExecuteReader())
                                    {
                                        if (reader.Read())
                                        {
                                            Console.SetCursorPosition(2, 6);
                                            Console.WriteLine(reader["product_name"] + " (Qtd disponível: " + reader["product_quantity"] + ")");
                                        }
                                        else
                                        {
                                            Console.SetCursorPosition(2, 6);
                                            Console.WriteLine("Nenhum Produto Encontrado.");
                                            System.Threading.Thread.Sleep(1000);

                                            Console.SetCursorPosition(0, 5);
                                            Console.WriteLine("| Produto:                                                                |");
                                            Console.SetCursorPosition(0, 6);
                                            Console.WriteLine("|                                                                         |");

                                            input = "";
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.Clear();
                                Console.WriteLine("Erro ao realizar consulta: " + ex);
                            }
                            finally
                            {
                                connector.CloseConnection();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Clear();
                    Console.WriteLine("Operação inválida: " + ex);
                }
            }

            return input;
        }

        public static void RegisterOrder(SQLiteConnector connector)
        {
            bool repeat = false;

            Console.WriteLine("! Registrar Ordem                                                         !");
            Console.WriteLine("+-------------------------------------------------------------------------+");
            Console.WriteLine("| Produto:                                                                |");
            Console.WriteLine("|                                                                         |");
            Console.WriteLine("| Quantidade:                                                             |");
            Console.WriteLine("| Data de Entrega:                                                        |");
            Console.WriteLine("+-------------------------------------------------------------------------+");

            int orderId = Convert.ToInt32(RO_ValidateProductInput(connector));
            int orderQuantity = Convert.ToInt32(RO_ValidateProductQuantity(connector, orderId));

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
                            RegisterOrder(connector);
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

                InterfaceManager(Console.ReadKey().KeyChar, connector);
            } while (true);
        }
    }
}