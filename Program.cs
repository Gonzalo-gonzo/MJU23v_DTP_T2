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
                // FIXME: Kontrollera om raden innehåller rätt antal delar innan split.
                string[] parts = line.Split('|');
                if (parts.Length != 5)
                {
                    throw new ArgumentException("Felaktigt format i länkraden.");
                }
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
                // FIXME: Lägg till felhantering om länken är ogiltig eller inte kan öppnas.
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

            // FIXME: Kontrollera om filen existerar innan inläsning.
            links = LoadLinksFromFile(filePath);

            Console.WriteLine("Välkommen till länklistan! Skriv 'hjälp' för hjälp!");

            do
            {
                Console.Write("> ");
                string cmdInput = Console.ReadLine().Trim();

                if (string.IsNullOrWhiteSpace(cmdInput))
                {
                    // FIXME: Lägg till en vänligare hantering av tomma kommandon.
                    HandleUnknownCommand("");
                    continue;
                }

                string[] cmdParts = cmdInput.Split();
                string command = cmdParts[0];

                if (command == "sluta")
                {
                    Console.WriteLine("Hej då! Välkommen åter!");
                    break;
                }
                else if (command == "hjälp")
                {
                    PrintHelp();
                }
                else if (command == "lista")
                {
                    if (links.Count == 0)
                    {
                        // FIXME: Informera användaren om att listan är tom.
                        Console.WriteLine("Inga länkar att visa.");
                    }
                    else
                    {
                        int index = 0;
                        foreach (Link linkObj in links)
                            linkObj.Print(index++);
                    }
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

                    // FIXME: Validera URL-formatet innan du lägger till länken.
                    Link newLink = new Link(category, group, name, description, url);
                    links.Add(newLink);
                }
                else if (command == "spara")
                {
                    if (cmdParts.Length == 2)
                    {
                        filePath = $@"..\..\..\links\{cmdParts[1]}";
                        // FIXME: Kontrollera om det går att skriva till filen innan sparning.
                        using (StreamWriter writer = new StreamWriter(filePath))
                        {
                            foreach (Link linkObj in links)
                            {
                                writer.WriteLine(linkObj.Serialize());
                            }
                        }
                        Console.WriteLine($"Länkar sparade till {filePath}.");
                    }
                    else
                    {
                        Console.WriteLine("Fel: Ange ett filnamn för att spara.");
                    }
                }
                else if (command == "ladda")
                {
                    if (cmdParts.Length == 2)
                    {
                        filePath = $@"..\..\..\links\{cmdParts[1]}";
                        // FIXME: Kontrollera om filen existerar innan inläsning.
                        links = LoadLinksFromFile(filePath);
                        Console.WriteLine($"Länkar laddade från {filePath}.");
                    }
                    else
                    {
                        Console.WriteLine("Fel: Ange en giltig filväg.");
                    }
                }
                else if (command == "ta")
                {
                    if (cmdParts.Length >= 3 && cmdParts[1] == "bort" && int.TryParse(cmdParts[2], out int index) && index >= 0 && index < links.Count)
                    {
                        links.RemoveAt(index);
                        Console.WriteLine($"Länk på index {index} har tagits bort.");
                    }
                    else
                    {
                        // FIXME: Ge användaren mer information om vad som gick fel.
                        Console.WriteLine("Fel: Ogiltigt index eller kommando.");
                    }
                }
                else if (command == "öppna")
                {
                    if (cmdParts.Length >= 3 && cmdParts[1] == "grupp")
                    {
                        string groupName = cmdParts[2];
                        var groupLinks = links.Where(link => link.Group == groupName).ToList();

                        if (groupLinks.Any())
                        {
                            foreach (Link linkObj in groupLinks)
                            {
                                linkObj.OpenLink();
                            }
                        }
                        else
                        {
                            // FIXME: Informera användaren om att gruppen inte hittades.
                            Console.WriteLine($"Fel: Gruppen '{groupName}' hittades inte.");
                        }
                    }
                    else if (cmdParts.Length >= 3 && cmdParts[1] == "länk" && int.TryParse(cmdParts[2], out int linkIndex) && linkIndex >= 0 && linkIndex < links.Count)
                    {
                        links[linkIndex].OpenLink();
                    }
                    else
                    {
                        // FIXME: Förbättra felmeddelandet för ogiltiga kommandon.
                        Console.WriteLine("Fel: Ogiltigt kommando eller index.");
                    }
                }
                else
                {
                    HandleUnknownCommand(command);
                }

            } while (true);
        }

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

        private static void HandleUnknownCommand(string command)
        {
            // FIXME: Gör felmeddelandet mer användarvänligt.
            Console.WriteLine($"Okänt kommando: '{command}'");
        }
    }
}
