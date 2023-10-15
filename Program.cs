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
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string databaseFilePath = Path.Combine(currentDirectory, "banco.db");
            _connection = new SQLiteConnection(@"Data Source=" + databaseFilePath + ";Version=3;");
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

        public static bool RO_RemoveMaterial(int productId, int orderQuantity, SQLiteConnector connector)
        {
            string query = @"UPDATE Product SET product_quantity = product_quantity - @OrderQuantity WHERE product_id = @ProductId";

            try
            {
                using (var command = new SQLiteCommand(query, connector.GetConnection()))
                {
                    command.Parameters.AddWithValue("OrderQuantity", orderQuantity);
                    command.Parameters.AddWithValue("ProductId", productId);
                    
                    connector.OpenConnection();
                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        return true;
                    } else
                    {
                        return false;
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

            return false;
        }

        public static bool RO_RegisterOrder(int orderId, int orderQuantity, string orderDate, SQLiteConnector connector)
        {
            string query = "INSERT INTO [Order] (product_id, order_quantity, order_deliveryDate, order_status) VALUES (@ProductId, @OrderQuantity, @OrderDeliveryDate, 0)";

            try
            {
                using (var command = new SQLiteCommand(query, connector.GetConnection()))
                {
                    command.Parameters.AddWithValue("@ProductId", orderId);
                    command.Parameters.AddWithValue("@OrderQuantity", orderQuantity);
                    command.Parameters.AddWithValue("OrderDeliveryDate", orderDate);
                    connector.OpenConnection();

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        bool isMaterialRemoved = RO_RemoveMaterial(orderId, orderQuantity, connector);

                        if (isMaterialRemoved) {
                            return true;
                        } else
                        {
                            return false;
                        }
                    } else
                    {
                        return false;
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

            return false;
        }

        public static bool RO_ConfirmOrder()
        {
            string? input = "";

            while (string.IsNullOrWhiteSpace(input))
            {
                try
                {
                    Console.SetCursorPosition(0, 10);
                    Console.WriteLine("|                                                                         |");
                    Console.SetCursorPosition(2, 10);
                    Console.WriteLine("Deseja confirmar a entrada? (S/N):");
                    Console.WriteLine("+-------------------------------------------------------------------------+");
                    Console.SetCursorPosition(37, 10);
                    input = Console.ReadLine();

                    if (input == null && string.IsNullOrWhiteSpace(input))
                    {
                        Console.SetCursorPosition(0, 12);
                        Console.WriteLine("|                                                                         |");
                        Console.SetCursorPosition(2, 12);
                        Console.WriteLine("Confirmação inválida.");
                        Console.WriteLine("+-------------------------------------------------------------------------+");

                        System.Threading.Thread.Sleep(1000);
                        ClearCurrentConsoleLine(0, 12);
                        ClearCurrentConsoleLine(0, 13);
                        input = "";
                    }
                    else
                    {
                        input = input.Trim().ToUpper();

                        if (input != "S" && input != "N")
                        {
                            Console.SetCursorPosition(0, 12);
                            Console.WriteLine("|                                                                         |");
                            Console.SetCursorPosition(2, 12);
                            Console.WriteLine("Confirmação inválida.");
                            Console.WriteLine("+-------------------------------------------------------------------------+");

                            System.Threading.Thread.Sleep(1000);
                            ClearCurrentConsoleLine(0, 12);
                            ClearCurrentConsoleLine(0, 13);
                            input = "";
                        }
                        else if (input == "S")
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Clear();
                    Console.WriteLine("Operação inválida: " + ex);
                }
            }

            return false;
        }

        public static string RO_ValidateProductDate()
        {
            string? input = "";

            while (string.IsNullOrWhiteSpace(input))
            {
                try
                {
                    Console.SetCursorPosition(19, 8);
                    input = Console.ReadLine();

                    if (input == null && string.IsNullOrWhiteSpace(input))
                    {
                        Console.SetCursorPosition(0, 10);
                        Console.WriteLine("|                                                                         |");
                        Console.SetCursorPosition(2, 10);
                        Console.WriteLine("A entrada não pode estar vazia.");
                        Console.WriteLine("+-------------------------------------------------------------------------+");

                        System.Threading.Thread.Sleep(1000);
                        ClearCurrentConsoleLine(0, 10);
                        ClearCurrentConsoleLine(0, 11);
                    }
                    else
                    {
                        // Verificar se a data está válida
                        if (DateTime.TryParseExact(input, "dd/MM/yyyy", null, DateTimeStyles.None, out DateTime orderDate))
                        {
                            if (orderDate.Date < DateTime.Today)
                            {
                                Console.SetCursorPosition(0, 10);
                                Console.WriteLine("|                                                                         |");
                                Console.SetCursorPosition(2, 10);
                                Console.WriteLine("A Data de entrega está inválida.");
                                Console.WriteLine("+-------------------------------------------------------------------------+");

                                System.Threading.Thread.Sleep(1000);
                                Console.SetCursorPosition(0, 8);
                                Console.WriteLine("| Data de Entrega:                                                        |");
                                ClearCurrentConsoleLine(0, 10);
                                ClearCurrentConsoleLine(0, 11);
                                input = "";
                            }
                        }
                        else
                        {
                            Console.SetCursorPosition(0, 10);
                            Console.WriteLine("|                                                                         |");
                            Console.SetCursorPosition(2, 10);
                            Console.WriteLine("A Data de entrega está inválida.");
                            Console.WriteLine("+-------------------------------------------------------------------------+");

                            System.Threading.Thread.Sleep(1000);
                            Console.SetCursorPosition(0, 8);
                            Console.WriteLine("| Data de Entrega:                                                        |");
                            ClearCurrentConsoleLine(0, 10);
                            ClearCurrentConsoleLine(0, 11);
                            input = "";
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
                        
                        System.Threading.Thread.Sleep(1000);
                        ClearCurrentConsoleLine(0, 10);
                        ClearCurrentConsoleLine(0, 11);
                    }
                    else if (!input.All(char.IsDigit))
                    {
                        Console.SetCursorPosition(0, 10);
                        Console.WriteLine("|                                                                         |");
                        Console.SetCursorPosition(2, 10);
                        Console.WriteLine("A entrada deve conter apenas números.");
                        Console.WriteLine("+-------------------------------------------------------------------------+");
                        input = "";
                        System.Threading.Thread.Sleep(1000);
                        Console.SetCursorPosition(0, 7);
                        Console.WriteLine("| Quantidade:                                                             |");
                        ClearCurrentConsoleLine(0, 10);
                        ClearCurrentConsoleLine(0, 11);
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
            bool repeat = true;

            while (repeat)
            {
                
                Console.WriteLine("! Registrar Ordem                                                         !");
                Console.WriteLine("+-------------------------------------------------------------------------+");
                Console.WriteLine("| Produto:                                                                |");
                Console.WriteLine("|                                                                         |");
                Console.WriteLine("| Quantidade:                                                             |");
                Console.WriteLine("| Data de Entrega:                                                        |");
                Console.WriteLine("+-------------------------------------------------------------------------+");

                int orderId = Convert.ToInt32(RO_ValidateProductInput(connector));
                int orderQuantity = Convert.ToInt32(RO_ValidateProductQuantity(connector, orderId));
                string orderDate = RO_ValidateProductDate();
                bool orderIsConfirmed = RO_ConfirmOrder();

                if (orderIsConfirmed)
                {
                    bool orderIsRegistered = RO_RegisterOrder(orderId, orderQuantity, orderDate, connector);

                    if (orderIsRegistered)
                    {
                        repeat = false;
                        InterfaceHeader();
                        Console.SetCursorPosition(0, 3);
                        Console.WriteLine("|                                                                         |");
                        Console.SetCursorPosition(2, 3);
                        Console.WriteLine("Ordem registrada com sucesso. Pressione ENTER para voltar ao menu.");
                        Console.WriteLine("+-------------------------------------------------------------------------+");
                        Console.SetCursorPosition(68, 3);
                        Console.ReadKey();
                    }
                } else
                {
                    Console.Clear();
                    InterfaceHeader();
                }
            }

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