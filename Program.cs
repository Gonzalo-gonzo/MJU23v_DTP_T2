using System.Diagnostics;

namespace MJU23v_DTP_T2
{
    internal class Program
    {
        static List<Link> links = new List<Link>();

        class Link
        {
            public string Category { get; set; }
            public string Group { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            public string Url { get; set; }

            public Link(string category, string group, string name, string description, string url)
            {
                Category = category;
                Group = group;
                Name = name;
                Description = description;
                Url = url;
            }

            public Link(string line)
            {
                string[] parts = line.Split('|');
                Category = parts[0];
                Group = parts[1];
                Name = parts[2];
                Description = parts[3];
                Url = parts[4];
            }

            public void Print(int index)
            {
                Console.WriteLine($"|{index,-2}|{Category,-10}|{Group,-10}|{Name,-20}|{Description,-40}|");
            }

            public void OpenLink()
            {
                Process application = new Process();
                application.StartInfo.UseShellExecute = true;
                application.StartInfo.FileName = Url;
                application.Start();
            }

            public string Serialize()
            {
                return $"{Category}|{Group}|{Name}|{Description}|{Url}";
            }
        }

        static void Main(string[] args)
        {
            string filePath = @"..\..\..\links\links.lis";

            // Använd den nya metoden för att läsa in länkar från fil
            links = LoadLinksFromFile(filePath);

            Console.WriteLine("Välkommen till länklistan! Skriv 'hjälp' för hjälp!");

            do
            {
                Console.Write("> ");
                string cmdInput = Console.ReadLine().Trim();
                string[] cmdParts = cmdInput.Split();
                string command = cmdParts[0];

                if (command == "sluta")
                {
                    Console.WriteLine("Hej då! Välkommen åter!");
                    break;
                }
                else if (command == "hjälp")
                {
                    // Använd den nya metoden för att skriva ut hjälptext
                    PrintHelp();
                }
                else if (command == "lista")
                {
                    int index = 0;
                    foreach (Link linkObj in links)
                        linkObj.Print(index++);
                }
                else if (command == "ny")
                {
                    Console.WriteLine("Skapa en ny länk:");
                    Console.Write("  ange kategori: ");
                    string category = Console.ReadLine();
                    Console.Write("  ange grupp: ");
                    string group = Console.ReadLine();
                    Console.Write("  ange namn: ");
                    string name = Console.ReadLine();
                    Console.Write("  ange beskrivning: ");
                    string description = Console.ReadLine();
                    Console.Write("  ange länk: ");
                    string url = Console.ReadLine();

                    Link newLink = new Link(category, group, name, description, url);
                    links.Add(newLink);
                }
                else if (command == "spara")
                {
                    if (cmdParts.Length == 2)
                    {
                        filePath = $@"..\..\..\links\{cmdParts[1]}";
                    }

                    using (StreamWriter writer = new StreamWriter(filePath))
                    {
                        foreach (Link linkObj in links)
                        {
                            writer.WriteLine(linkObj.Serialize());
                        }
                    }
                }
                else if (command == "ta")
                {
                    if (cmdParts[1] == "bort")
                    {
                        links.RemoveAt(int.Parse(cmdParts[2]));
                    }
                }
                else if (command == "öppna")
                {
                    if (cmdParts[1] == "grupp")
                    {
                        foreach (Link linkObj in links)
                        {
                            if (linkObj.Group == cmdParts[2])
                            {
                                linkObj.OpenLink();
                            }
                        }
                    }
                    else if (cmdParts[1] == "länk")
                    {
                        int index = int.Parse(cmdParts[2]);
                        links[index].OpenLink();
                    }
                }
                else
                {
                    Console.WriteLine($"Okänt kommando: '{command}'");
                }

            } while (true);
        }

        // Ny metod för att läsa in länkar från fil
        private static List<Link> LoadLinksFromFile(string filePath)
        {
            List<Link> linksList = new List<Link>();

            using (StreamReader reader = new StreamReader(filePath))
            {
                int index = 0;
                string currentLine;

                while ((currentLine = reader.ReadLine()) != null)
                {
                    Link linkObj = new Link(currentLine);
                    linksList.Add(linkObj);
                }
            }

            return linksList;
        }

        // Ny metod för att skriva ut hjälptext
        private static void PrintHelp()
        {
            Console.WriteLine("hjälp           - skriv ut den här hjälpen");
            Console.WriteLine("sluta           - avsluta programmet");
            Console.WriteLine("lista           - lista alla länkar");
            Console.WriteLine("ny              - skapa en ny länk");
            Console.WriteLine("spara           - spara länkar till fil");
            Console.WriteLine("ta bort <index> - ta bort en länk");
            Console.WriteLine("öppna länk <index> - öppna en specifik länk");
            Console.WriteLine("öppna grupp <gruppnamn> - öppna alla länkar i en grupp");
        }
    }
}
