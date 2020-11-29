using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;

namespace TiperiBezTohoTiperi
{
    class Program
    {
        // If modifying these scopes, delete your previously saved credentials
        // at ~/.credentials/sheets.googleapis.com-dotnet-quickstart.json
        static string[] Scopes = { SheetsService.Scope.Spreadsheets };
        static string ApplicationName = "TiperiBezTohoTiperi";
        static string Sheet = "140DiojBmiHuKPs3CVFIEyIWmFNvtc6luWauaTXDNReE";

        static string PremierLeague = "Premier League 2020/21";
        static string UEFA = "UEFA 2020/21";

        static void Main(string[] args)
        {
            string competition;
            Console.WriteLine("Evaluate Premier League(0) or UEFA(1)?");
            var input = Console.ReadLine();

            var columnRange = "G";
            var onlyTables = false;
            var lastIndex = 0;
            var firstIndex = 0;

            if (input == "1")
            {
                competition = UEFA;
                Console.WriteLine("What is the index of last match?");
                lastIndex = Int32.Parse(Console.ReadLine());

            } else if (input == "0")
            {
                competition = PremierLeague;
                Console.WriteLine("Do you only want to generate tables? (y)");
                if (Console.ReadLine() == "y") {
                    columnRange = "F";
                    onlyTables = true;
                }
                Console.WriteLine("What is the index of last match?");
                lastIndex = Int32.Parse(Console.ReadLine());

                Console.WriteLine("What is the index of the first team in the first table?");
                firstIndex = Int32.Parse(Console.ReadLine());

            } else
            {
                return;
            }

            UserCredential credential;

            using (var stream =
                new FileStream("credentials.json", FileMode.Open, FileAccess.Read))
            {
                // The file token.json stores the user's access and refresh tokens, and is created
                // automatically when the authorization flow completes for the first time.
                string credPath = "token.json";
                credential = GoogleWebAuthorizationBroker.AuthorizeAsync(
                    GoogleClientSecrets.Load(stream).Secrets,
                    Scopes,
                    "user",
                    CancellationToken.None,
                    new FileDataStore(credPath, true)).Result;
                Console.WriteLine("Credential file saved to: " + credPath);
            }

            // Create Google Sheets API service.
            var service = new SheetsService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Define request parameters.
            //String range = "Premier League 2018/19!B2:G10";
            String range = $"{competition}!B2:{columnRange}{lastIndex}";
            SpreadsheetsResource.ValuesResource.GetRequest request =
                    service.Spreadsheets.Values.Get(Sheet, range);

            ValueRange response = request.Execute();
            IList<IList<Object>> values = response.Values;
            if (values != null && values.Count > 0)
            {
                Table loksi = new Table("Loksi");
                Table pato = new Table("Pato");
                Table holo = new Table("Holo");

                var points = new int[3];
                foreach (var row in values)
                {
                    if (onlyTables)
                    {
                        Match match = new Match(row[0].ToString(), row[1].ToString(), row[4].ToString(), row[2].ToString(), row[3].ToString());
                        loksi.AddMatch(match.GetTeams()[0], match.GetTeams()[1], match.GetPickById(0)[0], match.GetPickById(0)[1]);
                        pato.AddMatch(match.GetTeams()[0], match.GetTeams()[1], match.GetPickById(2)[0], match.GetPickById(2)[1]);
                        holo.AddMatch(match.GetTeams()[0], match.GetTeams()[1], match.GetPickById(1)[0], match.GetPickById(1)[1]);
                    } else
                    {
                        if (!CheckMatchValidity(row))
                        {
                            Console.WriteLine(row[0] + " | " + row[1]);
                            continue;
                        }
                        Match match = new Match(row[0].ToString(), row[1].ToString(), row[4].ToString(), row[2].ToString(), row[3].ToString(), row[5].ToString());
                        var eval = match.Evaluate();
                        points[0] += eval[0];
                        points[1] += eval[1];
                        points[2] += eval[2];


                        Console.WriteLine(match.ToString() + " || " + eval[0] + " | " + eval[1] + " | " + eval[2]);
                    }
                }
                if (onlyTables)
                {
                    loksi.Sort();
                    loksi.ShowTable();
                    WriteTable(service, loksi, firstIndex);
                    Console.WriteLine();
                    holo.Sort();
                    holo.ShowTable();
                    WriteTable(service, holo, firstIndex+22);
                    Console.WriteLine();
                    pato.Sort();
                    pato.ShowTable();
                    WriteTable(service, pato, firstIndex+44);
                } else
                {
                    Console.WriteLine("Loksi: {0}, Holo: {1}, Pato: {2}", points[0], points[1], points[2]);
                }
            }
            else
            {
                Console.WriteLine("No data found.");
            }
            Console.Read();
        }

        private static void WriteTable(SheetsService service, Table table, int indexToStartAt)
        {
            IList<IList<object>> matrix = new List<IList<object>>();
            foreach (Team t in table.GetTeams())
            {
                var score = t.score[0].ToString() + ":" + t.score[1];
                List<object> entry = new List<object>
                {
                    t.name,
                    t.wins,
                    t.draws,
                    t.losses,
                    t.points,
                    score
                };
                    matrix.Add(entry);
            }

            var valueRange = new ValueRange
            {
                Values = matrix
            };

            SpreadsheetsResource.ValuesResource.UpdateRequest request =
                    service.Spreadsheets.Values.Update(valueRange, Sheet, $"Premier League 2020/21!O{indexToStartAt}:T{indexToStartAt+19}");
            request.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.RAW;
            UpdateValuesResponse result2 = request.Execute();
        }

        private static bool CheckMatchValidity(IList<Object> matchinfo)
        {
            if (matchinfo.Count != 6)
                return false;
            return true;
        }
    }
}
