using System.Data.SQLite;
using System.Globalization;

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
        private static bool running = true;

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

        public static void CreateWindow(int quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                Console.WriteLine("|                                                                         |");
            }
        }
        public static void InterfaceHeader()
        {
            string currentDate = GetDate();
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("+------------------------------------------------------------+------------+");
            Console.WriteLine($"! Gerenciamento de Ordens de Produção                        | {currentDate} !");
            Console.WriteLine("+------------------------------------------------------------+------------+");
        }

        // Manage Product

        public static bool MP_UpdateProduct(int productId, int productQuantity, SQLiteConnector connector)
        {
            string query = @"UPDATE Product SET product_quantity = product_quantity + @ProductQuantity WHERE product_id = @ProductId";

            try
            {
                using (var command = new SQLiteCommand(query, connector.GetConnection()))
                {
                    command.Parameters.AddWithValue("ProductQuantity", productQuantity);
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

        public static bool MP_ConfirmChange()
        {
            string? input = "";

            while (string.IsNullOrWhiteSpace(input))
            {
                try
                {
                    Console.SetCursorPosition(0, 9);
                    Console.WriteLine("|                                                                         |");
                    Console.SetCursorPosition(2, 9);
                    Console.WriteLine("Deseja confirmar a entrada? (S/N):");
                    Console.WriteLine("+-------------------------------------------------------------------------+");
                    Console.SetCursorPosition(37, 9);
                    input = Console.ReadLine();

                    if (input == null && string.IsNullOrWhiteSpace(input))
                    {
                        Console.SetCursorPosition(0, 11);
                        Console.WriteLine("|                                                                         |");
                        Console.SetCursorPosition(2, 11);
                        Console.WriteLine("Confirmação inválida.");
                        Console.WriteLine("+-------------------------------------------------------------------------+");

                        System.Threading.Thread.Sleep(1000);
                        ClearCurrentConsoleLine(0, 11);
                        ClearCurrentConsoleLine(0, 12);
                        input = "";
                    }
                    else
                    {
                        input = input.Trim().ToUpper();

                        if (input != "S" && input != "N")
                        {
                            Console.SetCursorPosition(0, 11);
                            Console.WriteLine("|                                                                         |");
                            Console.SetCursorPosition(2, 11);
                            Console.WriteLine("Confirmação inválida.");
                            Console.WriteLine("+-------------------------------------------------------------------------+");

                            System.Threading.Thread.Sleep(1000);
                            ClearCurrentConsoleLine(0, 11);
                            ClearCurrentConsoleLine(0, 12);
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

        public static string MP_ValidateProductQuantity()
        {
            string? input = "";
            int productQuantity = 0;

            while (string.IsNullOrWhiteSpace(input))
            {
                try
                {
                    Console.SetCursorPosition(29, 7);
                    input = Console.ReadLine();

                    if (input == null & string.IsNullOrWhiteSpace(input))
                    {
                        Console.SetCursorPosition(0, 9);
                        Console.WriteLine("|                                                                         |");
                        Console.SetCursorPosition(2, 9);
                        Console.WriteLine("A entrada não pode estar vazia.");
                        Console.WriteLine("+-------------------------------------------------------------------------+");
                        System.Threading.Thread.Sleep(1000);
                        ClearCurrentConsoleLine(0, 9);
                        ClearCurrentConsoleLine(0, 10);
                    }
                    else if (!input.All(char.IsDigit))
                    {
                        Console.SetCursorPosition(0, 9);
                        Console.WriteLine("|                                                                         |");
                        Console.SetCursorPosition(2, 9);
                        Console.WriteLine("A entrada deve conter apenas números.");
                        Console.WriteLine("+-------------------------------------------------------------------------+");
                        input = "";
                        System.Threading.Thread.Sleep(1000);
                        Console.SetCursorPosition(0, 7);
                        Console.WriteLine("| Quantidade para adicionar:                                                             |");
                        ClearCurrentConsoleLine(0, 9);
                        ClearCurrentConsoleLine(0, 10);
                    }
                    else
                    {
                        if (int.TryParse(input, out productQuantity))
                        {
                            if (productQuantity == 0)
                            {
                                Console.SetCursorPosition(0, 9);
                                Console.WriteLine("|                                                                         |");
                                Console.SetCursorPosition(2, 9);
                                Console.WriteLine("A quantidade não pode ser 0.");
                                Console.WriteLine("+-------------------------------------------------------------------------+");
                                System.Threading.Thread.Sleep(1000);
                                Console.SetCursorPosition(0, 7);
                                Console.WriteLine("| Quantidade para adicionar:                                                             |");
                                ClearCurrentConsoleLine(0, 9);
                                ClearCurrentConsoleLine(0, 10);
                                input = "";
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.Clear();
                    Console.WriteLine("Operação válida: " + ex);
                }
            }

            return input;
        }

        public static string MP_ValidateProductInput(SQLiteConnector connector)
        {
            string? input = "";
            int productId = 0;

            while (string.IsNullOrWhiteSpace(input))
            {
                try
                {
                    Console.SetCursorPosition(11, 5);
                    input = Console.ReadLine();

                    if (input == null && string.IsNullOrWhiteSpace(input))
                    {
                        Console.SetCursorPosition(0, 9);
                        Console.WriteLine("|                                                                         |");
                        Console.SetCursorPosition(2, 9);
                        Console.WriteLine("A entrada não pode estar vazia.");
                        Console.WriteLine("+-------------------------------------------------------------------------+");
                        System.Threading.Thread.Sleep(1000);
                        ClearCurrentConsoleLine(0, 9);
                        ClearCurrentConsoleLine(0, 10);
                    }
                    else if (!input.All(char.IsDigit))
                    {
                        Console.SetCursorPosition(0, 9);
                        Console.WriteLine("|                                                                         |");
                        Console.SetCursorPosition(2, 9);
                        Console.WriteLine("A entrada deve conter apenas números.");
                        Console.WriteLine("+-------------------------------------------------------------------------+");
                        System.Threading.Thread.Sleep(1000);
                        Console.SetCursorPosition(0, 5);
                        Console.WriteLine("| Produto:                                                                |");
                        ClearCurrentConsoleLine(0, 9);
                        ClearCurrentConsoleLine(0, 10);
                        input = "";
                    }
                    else
                    {
                        if (int.TryParse(input, out productId))
                        {
                            string query = "SELECT * FROM Product WHERE product_id = @Id";

                            try
                            {
                                using (var command = new SQLiteCommand(query, connector.GetConnection()))
                                {
                                    command.Parameters.AddWithValue("@Id", productId);
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
                    Console.WriteLine("Operação inválida:" + ex);
                }
            }

            return input;
        }

        public static void ManageProduct(SQLiteConnector connector)
        {
            bool repeat = true;

            while (repeat)
            {
                Console.Title = "Gerenciar Produto";

                Console.WriteLine("! Gerenciar Produto                                                       !");
                Console.WriteLine("+-------------------------------------------------------------------------+");
                Console.WriteLine("| Produto:                                                                |");
                Console.WriteLine("|                                                                         |");
                Console.WriteLine("| Quantidade para adicionar:                                              |");
                Console.WriteLine("+-------------------------------------------------------------------------+");

                int productId = Convert.ToInt32(MP_ValidateProductInput(connector));
                int productQuantity = Convert.ToInt32(MP_ValidateProductQuantity());
                bool productIsConfirmed = MP_ConfirmChange();

                if (productIsConfirmed)
                {
                    bool productIsUpdated = MP_UpdateProduct(productId, productQuantity, connector);

                    if (productIsUpdated)
                    {
                        repeat = false;
                        InterfaceHeader();
                        Console.SetCursorPosition(0, 3);
                        Console.WriteLine("|                                                                         |");
                        Console.SetCursorPosition(2, 3);
                        Console.WriteLine("Produto alterado com sucesso. Pressione ENTER para voltar ao menu.");
                        Console.WriteLine("+-------------------------------------------------------------------------+");
                        Console.SetCursorPosition(68, 3);
                        Console.ReadKey();
                    } else
                    {
                        Console.Clear();
                        InterfaceHeader();
                    }
                }
            }
        }

        // Register Product

        public static bool RP_RegisterProduct(string productName, int productQuantity, SQLiteConnector connector)
        {
            string query = "INSERT INTO [Product] (product_name, product_quantity) VALUES (@ProductName, @ProductQuantity)";

            try
            {
                using (var command = new SQLiteCommand(query, connector.GetConnection()))
                {
                    command.Parameters.AddWithValue("@ProductName", productName);
                    command.Parameters.AddWithValue("@ProductQuantity", productQuantity);
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

        public static bool RP_ConfirmRegister()
        {
            string? input = "";

            while (string.IsNullOrWhiteSpace(input))
            {
                try
                {
                    Console.SetCursorPosition(0, 8);
                    Console.WriteLine("|                                                                         |");
                    Console.SetCursorPosition(2, 8);
                    Console.WriteLine("Deseja confirmar a entrada? (S/N):");
                    Console.WriteLine("+-------------------------------------------------------------------------+");
                    Console.SetCursorPosition(37, 8);
                    input = Console.ReadLine();

                    if (input == null && string.IsNullOrWhiteSpace(input))
                    {
                        Console.SetCursorPosition(0, 10);
                        Console.WriteLine("|                                                                         |");
                        Console.SetCursorPosition(2, 10);
                        Console.WriteLine("Confirmação inválida.");
                        Console.WriteLine("+-------------------------------------------------------------------------+");

                        System.Threading.Thread.Sleep(1000);
                        ClearCurrentConsoleLine(0, 10);
                        ClearCurrentConsoleLine(0, 11);
                        input = "";
                    }
                    else
                    {
                        input = input.Trim().ToUpper();

                        if (input != "S" && input != "N")
                        {
                            Console.SetCursorPosition(0, 10);
                            Console.WriteLine("|                                                                         |");
                            Console.SetCursorPosition(2, 10);
                            Console.WriteLine("Confirmação inválida.");
                            Console.WriteLine("+-------------------------------------------------------------------------+");

                            System.Threading.Thread.Sleep(1000);
                            ClearCurrentConsoleLine(0, 10);
                            ClearCurrentConsoleLine(0, 11);
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
                    Console.WriteLine("Operação inválida:" + ex);
                }
            }


            return false;
        }

        public static string RP_ValidateProductQuantity()
        {
            string? input = "";
            int productQuantity = 0;

            while (string.IsNullOrWhiteSpace(input))
            {
                try
                {
                    Console.SetCursorPosition(22, 6);
                    input = Console.ReadLine();

                    if (input == null && string.IsNullOrWhiteSpace(input))
                    {
                        Console.SetCursorPosition(0, 8);
                        Console.WriteLine("|                                                                         |");
                        Console.SetCursorPosition(2, 8);
                        Console.WriteLine("A entrada não pode estar vazia.");
                        Console.WriteLine("+-------------------------------------------------------------------------+");

                        System.Threading.Thread.Sleep(1000);
                        ClearCurrentConsoleLine(0, 8);
                        ClearCurrentConsoleLine(0, 9);
                        input = "";
                    }
                    else if (!input.All(char.IsDigit))
                    {
                        Console.SetCursorPosition(0, 8);
                        Console.WriteLine("|                                                                         |");
                        Console.SetCursorPosition(2, 8);
                        Console.WriteLine("A entrada deve conter apenas números.");
                        Console.WriteLine("+-------------------------------------------------------------------------+");
                        input = "";
                        System.Threading.Thread.Sleep(1000);
                        Console.SetCursorPosition(0, 6);
                        Console.WriteLine("| Quantidade Inicial:                                                     |");
                        ClearCurrentConsoleLine(0, 8);
                        ClearCurrentConsoleLine(0, 9);
                    }
                    else
                    {
                        if (int.TryParse(input, out productQuantity))
                        {
                            if (productQuantity == 0)
                            {
                                Console.SetCursorPosition(0, 8);
                                Console.WriteLine("|                                                                         |");
                                Console.SetCursorPosition(2, 8);
                                Console.WriteLine("A quantidade não pode ser 0.");
                                Console.WriteLine("+-------------------------------------------------------------------------+");
                                input = "";
                                System.Threading.Thread.Sleep(1000);
                                Console.SetCursorPosition(0, 6);
                                Console.WriteLine("| Quantidade Inicial:                                                     |");
                                ClearCurrentConsoleLine(0, 8);
                                ClearCurrentConsoleLine(0, 9);
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

        public static string RP_ValidateProductName()
        {
            string? input = "";

            while (string.IsNullOrWhiteSpace(input))
            {
                try
                {
                    Console.SetCursorPosition(19, 5);
                    input = Console.ReadLine();

                    if (input == null && string.IsNullOrWhiteSpace(input))
                    {
                        Console.SetCursorPosition(0, 8);
                        Console.WriteLine("|                                                                         |");
                        Console.SetCursorPosition(2, 8);
                        Console.WriteLine("A entrada não pode estar vazia.");
                        Console.WriteLine("+-------------------------------------------------------------------------+");

                        System.Threading.Thread.Sleep(1000);
                        ClearCurrentConsoleLine(0, 8);
                        ClearCurrentConsoleLine(0, 9);
                        input = "";
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

        public static void RegisterProduct(SQLiteConnector connector)
        {
            bool repeat = true;

            while (repeat)
            {
                Console.Title = "Cadastrar Produto";

                Console.WriteLine("! Cadastrar Produto                                                       !");
                Console.WriteLine("+-------------------------------------------------------------------------+");
                Console.WriteLine("| Nome do Produto:                                                        |");
                Console.WriteLine("| Quantidade Inicial:                                                     |");
                Console.WriteLine("+-------------------------------------------------------------------------+");

                string productName = RP_ValidateProductName();
                int productQuantity = Convert.ToInt32(RP_ValidateProductQuantity());
                bool productIsConfirmed = RP_ConfirmRegister();

                if (productIsConfirmed)
                {
                    bool productIsRegistered = RP_RegisterProduct(productName, productQuantity, connector);

                    if (productIsRegistered)
                    {
                        repeat = false;
                        InterfaceHeader();
                        Console.SetCursorPosition(0, 3);
                        Console.WriteLine("|                                                                         |");
                        Console.SetCursorPosition(2, 3);
                        Console.WriteLine("Produto registrado com sucesso. Pressione ENTER para voltar ao menu.");
                        Console.WriteLine("+-------------------------------------------------------------------------+");
                        Console.SetCursorPosition(70, 3);
                        Console.ReadKey();
                    }
                } else
                {
                    Console.Clear();
                    InterfaceHeader();
                }
            }
        }

        public static bool LO_ConfirmChange()
        {
            return false;
        }

        public static bool LO_DetailedData(int orderId, SQLiteConnector connector)
        {
            string query = "SELECT o.*, p.product_name FROM [Order] o INNER JOIN Product p ON o.product_id = p.product_id WHERE o.order_id = @OrderId";

            try
            {
                using (var command = new SQLiteCommand(query, connector.GetConnection()))
                {
                    command.Parameters.AddWithValue("@OrderId", orderId);
                    connector.OpenConnection();

                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Console.SetCursorPosition(21, 3);
                            Console.WriteLine(reader["order_id"]);
                            Console.SetCursorPosition(11, 5);
                            Console.WriteLine(reader["product_name"]);
                            Console.SetCursorPosition(14, 6);
                            Console.WriteLine(reader["order_quantity"]);
                            Console.SetCursorPosition(19, 7);
                            Console.WriteLine(reader["order_deliveryDate"]);
                            Console.SetCursorPosition(10, 8);

                            int orderStatus = Convert.ToInt32(reader["order_status"]);

                            if (orderStatus == 0)
                            {
                                Console.WriteLine("Em andamento");
                                return true;
                            } else if (orderStatus == 1)
                            {
                                Console.WriteLine("Concluído");
                                return false;
                            } else
                            {
                                Console.Clear();
                                Console.WriteLine("Operação inválida.");
                                return false;
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

            return false;
        }

        // List Orders
        public static void LO_OrderStatus(int orderId, SQLiteConnector connector)
        {
            bool repeat = true;

            while (repeat)
            {
                InterfaceHeader();
                Console.WriteLine("! Detalhes da Ordem:                                                      !");
                Console.WriteLine("+-------------------------------------------------------------------------+");
                Console.WriteLine("| Produto:                                                                |");
                Console.WriteLine("| Quantidade:                                                             |");
                Console.WriteLine("| Data de Entrega:                                                        |");
                Console.WriteLine("| Status:                                                                 |");
                Console.WriteLine("+-------------------------------------------------------------------------+");
                Console.WriteLine("| Pressione ESC para voltar.                                              |");
                Console.WriteLine("+-------------------------------------------------------------------------+");

                bool isDevelopmentStatus = LO_DetailedData(orderId, connector);
                if (isDevelopmentStatus)
                {
                    Console.SetCursorPosition(0, 10);
                    Console.WriteLine("| Pressione ENTER para alterar o status para concluído.                   |");
                    Console.WriteLine("| Pressione ESC para voltar.                                              |");
                    Console.WriteLine("+-------------------------------------------------------------------------+");

                    bool changeStatusIsConfirmed = LO_ConfirmChange();

                    if (changeStatusIsConfirmed)
                    {

                    }

                }

                ConsoleKeyInfo key = Console.ReadKey();
                if (key.Key == ConsoleKey.Escape)
                {
                    break; // Sai do loop while quando a tecla "ESC" é pressionada
                }
            }
        }

        public static bool LO_ShowOrders(int orderType, SQLiteConnector connector)
        {
            bool running = true;
            string query = "SELECT o.*, p.product_name FROM [Order] o INNER JOIN Product p ON o.product_id = p.product_id WHERE o.order_status = @OrderType";

            try
            {
                using (var command = new SQLiteCommand(query, connector.GetConnection()))
                {
                    command.Parameters.AddWithValue("@OrderType", orderType);
                    connector.OpenConnection();
                    var reader = command.ExecuteReader();

                    var orders = new List<Tuple<int, string, int, string>>();

                    while (reader.Read())
                    {
                        int orderId = Convert.ToInt32(reader["order_id"]);
                        string productName = Convert.ToString(reader["product_name"]);
                        int orderQuantity = Convert.ToInt32(reader["order_quantity"]);
                        string deliveryDate = Convert.ToString(reader["order_deliveryDate"]);

                        string formattedOrder = string.Format("{0,-10}\t{1,-20}\t{2,-15}\t{3,-15}", orderId, productName, orderQuantity, deliveryDate);
                        orders.Add(new Tuple<int, string, int, string>(orderId, productName, orderQuantity, formattedOrder));
                    }

                    int selectedItemIndex = 0;
                    int itemCount = orders.Count;

                    int itemsToShow = 20;

                    ConsoleKeyInfo key;

                    while (running)
                    {
                        InterfaceHeader();
                        CreateWindow(1);
                        Console.SetCursorPosition(2, 3);
                        Console.WriteLine("{0,-10}\t{1,-20}\t{2,-15}\t{3,-15}", "N° Ordem", "Nome", "Quantidade", "Data"); // Cabeçalho da tabela
                        Console.WriteLine("+-------------------------------------------------------------------------+");
                        CreateWindow(itemCount);
                        Console.SetCursorPosition(2, 5);

                        int startIndex = Math.Max(0, selectedItemIndex - itemsToShow + 1);
                        int endIndex = Math.Min(itemCount, startIndex + itemsToShow);

                        for (int i = startIndex; i < endIndex; i++)
                        {
                            Console.SetCursorPosition(2, 5 + i - startIndex);

                            if (i == selectedItemIndex)
                            {
                                Console.BackgroundColor = ConsoleColor.Gray;
                                Console.ForegroundColor = ConsoleColor.Black;
                            }

                            Console.WriteLine(orders[i].Item4);

                            if (i == selectedItemIndex)
                            {
                                Console.ResetColor();
                            }
                        }
                        Console.WriteLine("+-------------------------------------------------------------------------+");
                        Console.WriteLine("| ENTER para ver mais detalhes                                            |");
                        Console.WriteLine("| ESC para voltar ao menu                                                 |");
                        Console.WriteLine("+-------------------------------------------------------------------------+");

                        key = Console.ReadKey(true);

                        if (key.Key == ConsoleKey.DownArrow && selectedItemIndex < itemCount - 1)
                        {
                            selectedItemIndex++;
                        }
                        else if (key.Key == ConsoleKey.UpArrow && selectedItemIndex > 0)
                        {
                            selectedItemIndex--;
                        }
                        else if (key.Key == ConsoleKey.Enter)
                        {
                            // Ir pra função com o order_id
                            int selectedOrderId = orders[selectedItemIndex].Item1;
                            LO_OrderStatus(selectedOrderId, connector);
                        }
                        else if (key.Key == ConsoleKey.Escape)
                        {
                            running = false;
                        }
                    }

                    connector.CloseConnection();
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

        public static void ListOrders(SQLiteConnector connector)
        {
            bool listRunning = true;
            int currentState = 0;

            while (listRunning)
            {
                switch(currentState)
                {
                    case 0:
                        Console.Title = "Listar Ordens";
                        InterfaceHeader();
                        Console.WriteLine("| 1 - Ordens em andamento                                                 |");
                        Console.WriteLine("| 2 - Ordens concluídas                                                   |");
                        Console.WriteLine("| 0 - Voltar                                                              |");
                        Console.WriteLine("+-------------------------------------------------------------------------+");
                        Console.WriteLine("! Selecione uma opção:                                                    !");
                        Console.WriteLine("+-------------------------------------------------------------------------+");
                        Console.SetCursorPosition(23, 7);
                        char keyPressed = Console.ReadKey().KeyChar;

                        if (keyPressed == '0')
                        {
                            listRunning = false;
                        }
                        else if (keyPressed == '1')
                        {
                            currentState = 1;
                        }
                        else if (keyPressed == '2')
                        {
                            currentState = 2;
                        }
                        break;

                    case 1: // Em andamento
                        if (!LO_ShowOrders(0, connector))
                        {
                            currentState = 0; // Retorna ao menu anterior se a tecla ESC for pressionada
                        }
                        else
                        {
                            Console.ReadLine();
                            currentState = 0;
                        }
                        break;

                    case 2: // Concluídas
                        InterfaceHeader();
                        LO_ShowOrders(1, connector);
                        currentState = 0;
                        break;

                    default:
                        break;
                }
            }
        }


        // Register Order
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
                            if (orderQuantity == 0)
                            {
                                Console.SetCursorPosition(0, 10);
                                Console.WriteLine("|                                                                         |");
                                Console.SetCursorPosition(2, 10);
                                Console.WriteLine("A quantidade não pode ser 0.");
                                Console.WriteLine("+-------------------------------------------------------------------------+");
                                System.Threading.Thread.Sleep(1000);
                                Console.SetCursorPosition(0, 7);
                                Console.WriteLine("| Quantidade:                                                             |");
                                ClearCurrentConsoleLine(0, 10);
                                ClearCurrentConsoleLine(0, 11);
                                input = "";
                            }
                            else
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
                        // Colocar timer e remover aviso
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
                                            if (Convert.ToInt32(reader["product_quantity"]) <= 0)
                                            {
                                                Console.SetCursorPosition(2, 6);
                                                Console.WriteLine(reader["product_name"] + " (Materiais Indisponíveis)");
                                                System.Threading.Thread.Sleep(1000);

                                                Console.SetCursorPosition(0, 5);
                                                Console.WriteLine("| Produto:                                                                |");
                                                Console.SetCursorPosition(0, 6);
                                                Console.WriteLine("|                                                                         |");

                                                input = "";
                                            }
                                            else
                                            {
                                                Console.SetCursorPosition(2, 6);
                                                Console.WriteLine(reader["product_name"] + " (Qtd disponível: " + reader["product_quantity"] + ")");
                                            }
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
                Console.Title = "Registrar Ordem";

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

        static void Main()
        {
            SQLiteConnector connector = new();
            connector.CreateConnection();
            bool running = true;
            int currentState = 0;

            while (running)
            {
                switch (currentState)
                {
                    case 0: // Menu principal
                        Console.Title = "Menu Principal";
                        InterfaceHeader();
                        Console.WriteLine("| 1 - Registrar Ordem                                                     |");
                        Console.WriteLine("| 2 - Listar Ordens                                                       |");
                        Console.WriteLine("| 3 - Cadastrar Produto                                                   |");
                        Console.WriteLine("| 4 - Gerenciar Produto                                                   |");
                        Console.WriteLine("| 0 - Sair                                                                |");
                        Console.WriteLine("+-------------------------------------------------------------------------+");
                        Console.WriteLine("! Selecione uma opção:                                                    !");
                        Console.WriteLine("+-------------------------------------------------------------------------+");
                        Console.SetCursorPosition(23, 9);
                        char keyPressed = Console.ReadKey().KeyChar;

                        if (keyPressed == '0')
                        {
                            running = false;
                        }
                        else if (keyPressed == '1')
                        {
                            currentState = 1;
                        } else if (keyPressed == '2')
                        {
                            currentState = 2;
                        } else if (keyPressed == '3')
                        {
                            currentState = 3;
                        } else if (keyPressed == '4')
                        {
                            currentState = 4;
                        }
                        break;

                    case 1: // Registrar Ordem
                        InterfaceHeader();
                        RegisterOrder(connector);
                        currentState = 0;
                        break;

                    case 2: // Listar Ordens
                        InterfaceHeader();
                        ListOrders(connector);
                        currentState = 0;
                        break;

                    case 3: // Cadastrar Produto
                        InterfaceHeader();
                        RegisterProduct(connector);
                        currentState = 0;
                        break;

                    case 4: // Gerenciar Materiais
                        InterfaceHeader();
                        ManageProduct(connector);
                        currentState = 0;
                        break;

                    default:
                        break;
                }

            }
        }
    }
}