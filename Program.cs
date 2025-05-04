

using System.Net.Http.Json;
using System.Text;
using carnetDb.Models;
using Microsoft.Data.Sqlite;

namespace VCSeeder
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            LoginModel loginModel = new LoginModel();
            HttpClient httpClient = new HttpClient();

            Console.WriteLine("c3p1-dev/VCSeeder");
            Console.Write("Url : ");

            string? b = Console.ReadLine();
            string baseurl;
            if(b != null)
            {
                if (!b.EndsWith('/'))
                {
                    // adds / at the end of path
                    b = b + "/";
                }

                baseurl = b;
            }
            else
            {
                throw new Exception("Base Url can't be null");
            }
            
            Console.Write("Login : ");
            loginModel.username = Console.ReadLine();
            Console.Write("Password : ");
            loginModel.password = GetHiddenConsoleInput();

            // try auth
            var response = await httpClient.PostAsJsonAsync<LoginModel>(baseurl + "api/auth/login", loginModel);
            var content = await response.Content.ReadFromJsonAsync<Token>();

            // display token
            Console.WriteLine();
            Console.WriteLine();
            if (content != null && content.token != null)
            {
                Console.WriteLine("Token Bearer : " + content.token.ToString());
                Console.WriteLine("Expiration   : " + content.expiration.ToString());
                Console.WriteLine();
            }
            else
            {
                Console.WriteLine("Auth failed!");
                return;
            }

            // add token to http headers
            httpClient.DefaultRequestHeaders.Remove("Authorization");
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {content.token.ToString()}");

            // clear the target database
            Console.Write("Erasing FicCpts ...");
            var result = await httpClient.GetStringAsync(baseurl + "api/apps/visualcarnet/remove/accounts");
            Console.Write("Ok [" + result.ToString() + " lignes effacées]");
            Console.WriteLine();

            Console.Write("Erasing FicFams ...");
            result = await httpClient.GetStringAsync(baseurl + "api/apps/visualcarnet/remove/families");
            Console.Write("Ok [" + result.ToString() + " lignes effacées]");
            Console.WriteLine();

            Console.Write("Erasing FicSfas ...");
            result = await httpClient.GetStringAsync(baseurl + "api/apps/visualcarnet/remove/subfamilies");
            Console.Write("Ok [" + result.ToString() + " lignes effacées]");
            Console.WriteLine();

            Console.Write("Erasing WrkEcrLigs ...");
            result = await httpClient.GetStringAsync(baseurl + "api/apps/visualcarnet/remove/records");
            Console.Write("Ok [" + result.ToString() + " lignes effacées]");
            Console.WriteLine();

            // read source database
            string connectionString = "Data source=vcdump.db";
            SqliteConnection connection = new SqliteConnection(connectionString);

            // load it in memory
            List<FicCpt> AccountList = new List<FicCpt>();
            List<FicFam> FamilyList = new List<FicFam>();
            List<FicSfa> SubFamilyList = new List<FicSfa>();
            List<WrkEcrLig> RecordList = new List<WrkEcrLig>();

            //
            Console.Write("Loading local data ...");
            try
            {
                connection.Open();

                string sql = "SELECT * FROM FicCpt";
                SqliteCommand command = new SqliteCommand(sql, connection);
                SqliteDataReader reader = command.ExecuteReader();

                // get accounts
                while (reader.Read())
                {
                    AccountList.Add(new FicCpt()
                    {
                        CodCpt = reader.GetString(0),
                        Nom = reader.GetString(1),
                        Visible = reader.GetString(2)
                    });
                }

                sql = "SELECT * FROM FicFam";
                command = new SqliteCommand(sql, connection);
                reader = command.ExecuteReader();

                // get family table
                while (reader.Read())
                {
                    FamilyList.Add(new FicFam()
                    {
                        CodFam = reader.GetString(0),
                        Nom = reader.GetString(1)
                    });
                }

                sql = "SELECT * FROM FicSfa";
                command = new SqliteCommand(sql, connection);
                reader = command.ExecuteReader();

                // get subfamily table
                while (reader.Read())
                {
                    SubFamilyList.Add(new FicSfa()
                    {
                        CodSfa = reader.GetString(0),
                        Nom = reader.GetString(1),
                        CodFam = reader.GetString(2)
                    });
                }

                sql = "SELECT * FROM WrkEcrLig";
                command = new SqliteCommand(sql, connection);
                reader = command.ExecuteReader();

                // get subfamily list
                while (reader.Read())
                {
                    RecordList.Add(new WrkEcrLig()
                    {
                        CodCpt = reader.GetString(0),
                        Nol = reader.GetInt32(1),
                        Jma = DateOnly.FromDateTime(reader.GetDateTime(2)),
                        JmaVal = DateOnly.FromDateTime(reader.GetDateTime(3)),
                        NoChq =  reader.GetString(4),
                        Lib1 = reader.GetString(5),
                        Lib2 = reader.GetString(6),
                        Deb = reader.GetDouble(7),
                        Cre = reader.GetDouble(8),
                        CodSfa = reader.GetString(9),
                        SldProgressif = reader.GetDouble(10)
                    });
                }

            }
            catch (Exception e)
            {
                Console.WriteLine("Exception : " + e.Message);
            }

            Console.Write(" Ok [" + (AccountList.Count + FamilyList.Count + SubFamilyList.Count + RecordList.Count).ToString() + " lignes chargées]");
            Console.WriteLine();

            // write Accounts on online db
            Console.Write("Writing FicCpt table ...");
            foreach (var account in AccountList)
            {
                FicCptTransfertModel tm = new FicCptTransfertModel()
                {
                    Id = Guid.Empty,
                    UserId = Guid.Empty,
                    CodCpt = account.CodCpt,
                    Nom = account.Nom,
                    Visible = account.Visible
                };
                await httpClient.PostAsJsonAsync<FicCptTransfertModel>(baseurl + "api/apps/visualcarnet/add/account", tm);
            }
            Console.Write("Ok [" + AccountList.Count + "lignes écrites]");
            Console.WriteLine();

            // write Families on online db
            Console.Write("Writing FicFam table ...");
            foreach (var family in FamilyList)
            {
                FicFamTransfertModel tm = new FicFamTransfertModel()
                {
                    Id = Guid.Empty,
                    UserId = Guid.Empty,
                    CodFam = family.CodFam,
                    Nom = family.Nom
                };
                await httpClient.PostAsJsonAsync<FicFamTransfertModel>(baseurl + "api/apps/visualcarnet/add/family", tm);
            }
            Console.Write("Ok [" + FamilyList.Count + "lignes écrites]");
            Console.WriteLine();

            // write SubFamilies on online db
            Console.Write("Writing FicSfa table ...");
            foreach (var subfamily in SubFamilyList)
            {
                FicSfaTransfertModel tm = new FicSfaTransfertModel()
                {
                    Id = Guid.Empty,
                    UserId = Guid.Empty,
                    CodFam = subfamily.CodFam,
                    CodSfa = subfamily.CodSfa,
                    Nom = subfamily.Nom
                };
                await httpClient.PostAsJsonAsync<FicSfaTransfertModel>(baseurl + "api/apps/visualcarnet/add/subfamily", tm);
            }
            Console.Write("Ok [" + SubFamilyList.Count + "lignes écrites]");
            Console.WriteLine();

            // write Records on online db
            Console.Write("Writing WrkEcrLig table ...");
            foreach (var record in RecordList)
            {
                WrkEcrLigTransfertModel tm = new WrkEcrLigTransfertModel()
                {
                    Id = Guid.Empty,
                    UserId = Guid.Empty,
                    CodCpt = record.CodCpt,
                    CodSfa = record.CodSfa,
                    Cre = record.Cre,
                    Deb = record.Deb,
                    Jma = record.Jma,
                    JmaVal = record.JmaVal,
                    Lib1 = record.Lib1,
                    Lib2 = record.Lib2,
                    NoChq = record.NoChq,
                    Nol = record.Nol,
                    SldProgressif = record.SldProgressif
                };
                await httpClient.PostAsJsonAsync<WrkEcrLigTransfertModel>(baseurl + "api/apps/visualcarnet/add/record", tm);
            }
            Console.Write("Ok [" + RecordList.Count + "lignes écrites]");
            Console.WriteLine();
        }

        private static string GetHiddenConsoleInput()
        {
            StringBuilder input = new StringBuilder();
            while (true)
            {
                var key = Console.ReadKey(true);
                if (key.Key == ConsoleKey.Enter) break;
                if (key.Key == ConsoleKey.Backspace && input.Length > 0) input.Remove(input.Length - 1, 1);
                else if (key.Key != ConsoleKey.Backspace) input.Append(key.KeyChar);
            }
            return input.ToString();
        }
    }

    class Token
    {
        public string? token { get; set; }
        public DateTime expiration { get; set; }
    }

    class LoginModel
    {
        public string? username { get; set; }
        public string? password { get; set; }
    }
}
