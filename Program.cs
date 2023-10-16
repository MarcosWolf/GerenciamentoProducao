using System.Data.SQLite;
using System.Globalization;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using System.Diagnostics;

/*
 * Gerenciamento de Controle de Produção - Desafio Técnico
 * Desenvolvido por: Marcos Vinícios do Carmo Ramos
 * (013) 98131-4531
 * www.github.com/MarcosWolf/
 * www.marcoswolf.com.br/
 * www.linkedin.com/in/MarcosWolf
*/

namespace GerenciamentoProducao
{
    /// <summary>
    /// Esta classe é responsável pela conexão com o banco de dados SQLite
    /// </summary>
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

        /// <summary>
        /// Método para facilitar na hora de precisar limpar uma linha.
        /// </summary>
        /// <param name="left">Indica a posição horizontal do Console que deve ser selecionada.</param>
        /// <param name="top">Indica a posição vertical do Console que deve ser selecionada.</param>
        public static void ClearCurrentConsoleLine(int left, int top)
        {
            Console.SetCursorPosition(left, top);
            Console.Write(new string(' ', Console.WindowWidth));
            Console.SetCursorPosition(left, top);
        }

        /// <summary>
        /// Método para obter a data atual.
        /// </summary>
        /// <returns>Retorna a data formada no padrão dd/MM/yyyy.</returns>
        public static string GetDate()
        {
            DateTime currentDate = DateTime.Now;
            string formatedDate = currentDate.ToString("dd/MM/yyyy");
            return formatedDate;
        }

        /// <summary>
        /// Método para facilitar na hora de precisar fazer as bordas da janela.
        /// </summary>
        /// <param name="quantity">Variável para especificar a quantidade necessária de linhas durante o laço.</param>
        public static void CreateWindow(int quantity)
        {
            for (int i = 0; i < quantity; i++)
            {
                Console.WriteLine("|                                                                         |");
            }
        }

        /// <summary>
        /// Método para desenhar o cabeçalho da aplicação.
        /// </summary>
        public static void InterfaceHeader()
        {
            string currentDate = GetDate();
            Console.Clear();
            Console.SetCursorPosition(0, 0);
            Console.WriteLine("+------------------------------------------------------------+------------+");
            Console.WriteLine($"! Gerenciamento de Ordens de Produção                        | {currentDate} !");
            Console.WriteLine("+------------------------------------------------------------+------------+");
        }

        /*
         * Manage Product
         */

        /// <summary>
        /// Atualiza a quantidade de um produto no banco de dados.
        /// </summary>
        /// <param name="productId">O ID do produto a ser atualizado.</param>
        /// <param name="productQuantity">A quantidade a ser adicionada ao produto.</param>
        /// <param name="connector">O conector SQLite usado para acessar o banco de dados.</param>
        /// <returns>Verdadeiro se a atualização foi bem-sucedida, falso caso contrário.</returns>
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

        /// <summary>
        /// Confirma a intenção do usuário de continuar o processo de incremento de quantidade.
        /// </summary>
        /// <returns>Verdadeiro se o usuário optou por confirmar, falso caso contrário.</returns>
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

        /// <summary>
        /// Valida os dados de entrada referentes a quantidade.
        /// </summary>
        /// <returns>Se todas as validações ocorrerem corretamente, retorna o dado do input Quantidade.</returns>
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

        /// <summary>
        /// Valida os dados de entrada referentes ao código do Produto.
        /// </summary>
        /// <param name="connector">O conector SQLite usado para acessar o banco de dados.</param>
        /// <returns>Se todas as validações ocorrerem corretamente, retorna o dado do input Id</returns>
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

        /// <summary>
        /// Gerencia a atualização de um produto no banco de dados. Solicita a entrada do ID do produto e a quantidade a ser adicionada,
        /// confirma a operação e atualiza a quantidade do produto. Exibe mensagens de sucesso ou falha de atualização.
        /// </summary>
        /// <param name="connector">O conector SQLite usado para acessar o banco de dados.</param>
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
                    }
                }
                else
                {
                    Console.Clear();
                    InterfaceHeader();
                }
            }
        }

        /*
         * Register Product
        */

        /// <summary>
        /// Cadastra um produto no banco de dados.
        /// </summary>
        /// <param name="productName">O nome do produto a ser cadastrado.</param>
        /// <param name="productQuantity">A quantidade do produto a ser inserido no banco.</param>
        /// <param name="connector">O conector SQLite usado para acessar o banco de dados.</param>
        /// <returns>Verdadeiro se a inserção foi bem-sucedida, falso caso contrário.</returns>
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

        /// <summary>
        /// Confirma a intenção do usuário de continuar o processo de cadastro do produto.
        /// </summary>
        /// <returns>Verdadeiro se o usuário optou por confirmar, falso caso contrário.</returns>
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

        /// <summary>
        /// Valida os dados de entrada referentes a quantidade.
        /// </summary>
        /// <returns>Se todas as validações ocorrerem corretamente, retorna o dado do input Quantidade.</returns>
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

        /// <summary>
        /// Valida os dados de entrada referentes ao nome do Produto.
        /// </summary>
        /// <returns>Se todas as validações ocorrerem corretamente, retorna o dado do input Nome</returns>
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

        /// <summary>
        /// Registra um novo produto no sistema.
        /// </summary>
        /// <param name="connector">O conector SQLite usado para acessar o banco de dados.</param>
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

        /// <summary>
        /// Atualiza o estado de "Em andamento" para "Concluído" de uma ordem.
        /// </summary>
        /// <param name="orderId">O ID da ordem a ser alterada.</param>
        /// <param name="connector">O conector SQLite usado para acessar o banco de dados.</param>
        /// <returns>Verdadeiro se a alteração foi bem-sucedida, falso caso contrário.</returns>
        public static bool LO_RegisterChange(int orderId, SQLiteConnector connector)
        {
            string query = @"UPDATE [Order] SET order_status = 1 WHERE order_id = @OrderId";

            try
            {
                using (var command = new SQLiteCommand(query, connector.GetConnection()))
                {
                    command.Parameters.AddWithValue("OrderId", orderId);

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

        /*
         * List Orders
        */

        /// <summary>
        /// Confirma a intenção do usuário de continuar a alteração de estado da ordem.
        /// </summary>
        /// <returns>Verdadeiro se o usuário optou por confirmar, falso caso contrário.</returns>
        public static bool LO_ConfirmChange()
        {
            string? input = "";

            while (string.IsNullOrWhiteSpace(input))
            {
                try
                {
                    Console.SetCursorPosition(0, 10);
                    Console.WriteLine("|                                                                         |");
                    Console.SetCursorPosition(2, 10);
                    Console.WriteLine("Deseja confirmar a alteração? (S/N):");
                    Console.WriteLine("+-------------------------------------------------------------------------+");
                    ClearCurrentConsoleLine(0, 12);
                    Console.SetCursorPosition(39, 10);
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
                    Console.WriteLine("Operação inválida: " + ex);
                }
            }

            return false;
        }

        /// <summary>
        /// Método responsável para separar as informações da ordem selecionada no banco de dados e desenhá-las na tela.
        /// </summary>
        /// <param name="orderId">O ID da ordem que está sendo apresentada.</param>
        /// <param name="connector">O conector SQLite usado para acessar o banco de dados.</param>
        /// <returns>Verdadeiro se a ordem está com o estado "Em andamento", falso caso esteja com o estado em "Concluído" ou apresente algum erro de operação.</returns>
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

        /// <summary>
        /// Método responsável para apresentar os dados detalhados da ordem selecionada.
        /// </summary>
        /// <param name="orderId">O ID da ordem que está sendo apresentada.</param>
        /// <param name="connector">O conector SQLite usado para acessar o banco de dados.</param>
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

                    ConsoleKeyInfo keyUnfinished = Console.ReadKey();
                    if (keyUnfinished.Key == ConsoleKey.Enter)
                    {
                        bool changeStatusIsConfirmed = LO_ConfirmChange();

                        if (changeStatusIsConfirmed)
                        {
                            bool isStatusRegistered = LO_RegisterChange(orderId, connector);

                            if (isStatusRegistered)
                            {
                                repeat = false;
                                InterfaceHeader();
                                Console.SetCursorPosition(0, 3);
                                Console.WriteLine("|                                                                         |");
                                Console.SetCursorPosition(2, 3);
                                Console.WriteLine("Ordem alterada com sucesso. Pressione ENTER para voltar ao menu.");
                                Console.WriteLine("+-------------------------------------------------------------------------+");
                                Console.SetCursorPosition(67, 3);
                                Console.ReadKey();
                            }
                            else
                            {
                                Console.Clear();
                                InterfaceHeader();
                            }
                        }
                    }
                    else if (keyUnfinished.Key == ConsoleKey.Escape)
                    {
                        break;
                    }

                }
                else
                {
                    ConsoleKeyInfo keyFinished = Console.ReadKey();
                    if (keyFinished.Key == ConsoleKey.Escape)
                    {
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Carrega as ordens do banco de dados de acordo com o tipo de ordem especificada.
        /// </summary>
        /// <param name="orderType">O tipo de ordem a ser carregado. 0 para ordens em andamento, 1 para ordens concluídas.</param>
        /// <param name="connector">O conector SQLite usado para acessar o banco de dados.</param>
        /// <returns>Uma lista de tuplas que contém as informações das ordens carregadas. Cada tupla contém o ID da ordem, o nome do produto, a quantidade de ordens e a data de entrega.</returns>
        public static List<Tuple<int, string, int, string>> LO_LoadOrders(int orderType, SQLiteConnector connector)
        {
            List<Tuple<int, string, int, string>> orders = new List<Tuple<int, string, int, string>>();
            string query = "SELECT o.*, p.product_name FROM [Order] o INNER JOIN Product p ON o.product_id = p.product_id WHERE o.order_status = @OrderType";

            try
            {
                using (var command = new SQLiteCommand(query, connector.GetConnection()))
                {
                    command.Parameters.AddWithValue("@OrderType", orderType);
                    connector.OpenConnection();
                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        int orderId = Convert.ToInt32(reader["order_id"]);
                        string productName = Convert.ToString(reader["product_name"]);
                        int orderQuantity = Convert.ToInt32(reader["order_quantity"]);
                        string deliveryDate = Convert.ToString(reader["order_deliveryDate"]);

                        string formattedOrder = string.Format("{0,-10}\t{1,-20}\t{2,-15}\t{3,-15}", orderId, productName, orderQuantity, deliveryDate);
                        orders.Add(new Tuple<int, string, int, string>(orderId, productName, orderQuantity, formattedOrder));
                    }

                    return orders;
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

                InterfaceHeader();
                CreateWindow(1);
                Console.SetCursorPosition(2, 3);
                Console.WriteLine("{0,-10}\t{1,-20}\t{2,-15}\t{3,-15}", "N° Ordem", "Nome", "Quantidade", "Data"); // Cabeçalho da tabela
                Console.WriteLine("+-------------------------------------------------------------------------+");
                CreateWindow(orders.Count);
                Console.SetCursorPosition(2, 5);
            }

            return orders;
        }

        /// <summary>
        /// Exibe as ordens correspondentes ao tipo de estado selecionado em uma lista interativa. 
        /// </summary>
        /// <param name="orderType">O tipo de ordem a ser carregado. 0 para ordens em andamento, 1 para ordens concluídas.</param>
        /// <param name="connector">O conector SQLite usado para acessar o banco de dados.</param>
        /// <returns>Uma lista de tuplas que contém as informações das ordens carregadas. Cada tupla contém o ID da ordem, o nome do produto, a quantidade de ordens e a data de entrega.</returns>
        public static bool LO_ShowOrders(int orderType, SQLiteConnector connector)
        {
            bool running = true;
            ConsoleKeyInfo key;

            while (running)
            {
                List<Tuple<int, string, int, string>> orders = LO_LoadOrders(orderType, connector);

                int selectedItemIndex = 0;
                int itemsToShow = 20;
                int startIndex, endIndex;

                do
                {
                    startIndex = Math.Max(0, selectedItemIndex - itemsToShow + 1);
                    endIndex = Math.Min(orders.Count, startIndex + itemsToShow);

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

                    if (key.Key == ConsoleKey.DownArrow && selectedItemIndex < orders.Count - 1)
                    {
                        selectedItemIndex++;
                    }
                    else if (key.Key == ConsoleKey.UpArrow && selectedItemIndex > 0)
                    {
                        selectedItemIndex--;
                    }
                    else if (key.Key == ConsoleKey.Enter)
                    {
                        int selectedOrderId = orders[selectedItemIndex].Item1;
                        LO_OrderStatus(selectedOrderId, connector);
                        orders = LO_LoadOrders(orderType, connector);
                    }
                    else if (key.Key == ConsoleKey.Escape)
                    {
                        running = false;
                    }
                } while (running);
            }
            
            return false;
        }

        /// <summary>
        /// Obtém os detalhes das ordens pelo banco de dados, para utilizar na geração de relatório PDF.
        /// </summary>
        /// <param name="connector">O conector SQLite usado para acessar o banco de dados.</param>
        /// <param name="orderType">O tipo de ordem a ser recuperado. 0 para ordens em andamento, 1 para ordens concluídas.</param>
        /// <returns>Uma lista de strings contendo os detalhes das ordens no formato "ID da Ordem: Nome do Produto - Quantidade und (Data de Entrega)".</returns>
        public static List<string> LO_FetchOrders(SQLiteConnector connector, int orderType)
        {
            List<string> orders = new List<string>();

            string query = "SELECT o.*, p.product_name FROM [Order] o INNER JOIN Product p ON o.product_id = p.product_id WHERE o.order_status = @OrderType";

            using (var command = new SQLiteCommand(query, connector.GetConnection()))
            {
                command.Parameters.AddWithValue("@OrderType", orderType);
                connector.OpenConnection();

                using (SQLiteDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        string orderDetails = $"{reader["order_id"]}: {reader["product_name"]} - {reader["order_quantity"]} und ({reader["order_deliveryDate"]})";
                        orders.Add(orderDetails);
                    }
                }

                connector.CloseConnection();
            }

            return orders;
        }

        /// <summary>
        /// Método responsável para gerar os relatórios em PDF.
        /// </summary>
        /// <param name="connector">O conector SQLite usado para acessar o banco de dados.</param>
        /// <returns>Retorna verdadeiro caso tudo ocorra corretamente, caso contrário retorna falso.</returns>
        public static bool LO_GeneratePDF(SQLiteConnector connector)
        {
            List<string> ordersInProgress = LO_FetchOrders(connector, 0);
            List<string> completedOrders = LO_FetchOrders(connector, 1);

            try
            {
                using (PdfDocument document = new PdfDocument())
                {
                    PdfPage page = document.AddPage();
                    XGraphics gfx = XGraphics.FromPdfPage(page);
                    XFont font = new XFont("Verdana", 20, XFontStyle.BoldItalic);
                    XFont titleFont = new XFont("Arial", 16, XFontStyle.Bold);
                    XFont subtitleFont = new XFont("Arial", 12, XFontStyle.Bold);
                    XFont paragraphFont = new XFont("Arial", 10, XFontStyle.Regular);

                    // Título
                    gfx.DrawString("Relatório de Ordens - " + GetDate(), titleFont, XBrushes.Black,
                        new XRect(0, 0, page.Width, 100),
                        XStringFormats.Center);

                    // Seção de Ordens em Andamento
                    gfx.DrawString("Ordens em Andamento (" + ordersInProgress.Count + ")", subtitleFont, XBrushes.Black,
                        new XRect(0, 30, page.Width, 100),
                        XStringFormats.Center);

                    int currentY = 100;

                    // Adicionando detalhes das Ordens em Andamento
                    foreach (string order in ordersInProgress)
                    {
                        gfx.DrawString(order, paragraphFont, XBrushes.Black,
                            new XRect(50, currentY, page.Width - 100, page.Height - 100),
                            XStringFormats.TopLeft);
                        currentY += 20;
                    }

                    PdfPage page2 = document.AddPage();
                    XGraphics gfx2 = XGraphics.FromPdfPage(page2);

                    gfx2.DrawString("Relatório de Ordens - " + GetDate(), titleFont, XBrushes.Black,
                new XRect(0, 0, page2.Width, 100),
                XStringFormats.Center);

                    gfx2.DrawString("Ordens Concluídas (" + completedOrders.Count + ")", subtitleFont, XBrushes.Black,
                        new XRect(0, 30, page2.Width, 100),
                        XStringFormats.Center);

                    currentY = 100;

                    // Adicionando detalhes das Ordens em Andamento
                    foreach (string order in completedOrders)
                    {
                        gfx2.DrawString(order, paragraphFont, XBrushes.Black,
                            new XRect(50, currentY, page.Width - 100, page.Height - 100),
                            XStringFormats.TopLeft);
                        currentY += 20;
                    }

                    const string filename = "Report.pdf";
                    document.Save(filename);

                    if (File.Exists(filename))
                    {
                        System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
                        {
                            FileName = filename,
                            UseShellExecute = true
                        });
                    } else
                    {
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.Clear();
                Console.WriteLine("Erro ao gerar o PDF: " + ex);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Apresenta um menu interativo para listar e alterar estado de ordens e para gerar relatório PDF.
        /// </summary>
        /// <param name="connector">O conector SQLite utilizado para acessar o banco de dados.</param>
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
                        Console.WriteLine("| 3 - Gerar Relatório PDF                                                 |");
                        Console.WriteLine("| 0 - Voltar                                                              |");
                        Console.WriteLine("+-------------------------------------------------------------------------+");
                        Console.WriteLine("! Selecione uma opção:                                                    !");
                        Console.WriteLine("+-------------------------------------------------------------------------+");
                        Console.SetCursorPosition(23, 8);
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
                        else if (keyPressed == '3')
                        {
                            currentState = 3;
                        }
                        break;

                    case 1: // Em andamento
                        if (!LO_ShowOrders(0, connector))
                        {
                            currentState = 0;
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

                    case 3: // Relatório PDF
                        InterfaceHeader();
                        LO_GeneratePDF(connector);
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

                        System.Threading.Thread.Sleep(1000);
                        ClearCurrentConsoleLine(0, 10);
                        ClearCurrentConsoleLine(0, 11);
                        Console.SetCursorPosition(0, 5);
                        Console.WriteLine("| Produto:                                                                |");
                        input = "";
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

                    case 4: // Gerenciar Produtos
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