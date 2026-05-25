namespace ExceptionsDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            {
                Console.WriteLine("=== Start av programmet ===");

                // Exempel 1: try-catch-finally
                try
                {
                    Console.WriteLine("Försöker inte läsa fil och räkna...");
                    var result = ProcessFile(AppContext.BaseDirectory, "numbers.txt");

                    Console.WriteLine($"\nResultat: {result}");
                }
                catch (FileNotFoundException ex)
                {
                    // Specifikt fel om filen inte finns
                    Console.WriteLine($"Filen hittades: {ex.Message}");
                }
                catch (FormatException ex)
                {
                    // Specifikt fel om texten inte kan tolkas som tal
                    Console.WriteLine($"Rätt format: {ex.Message}");
                }
                catch (DivideByZeroException ex)
                {
                    // Specifikt fel om nolldivision
                    Console.WriteLine($"Kan inte dividera med ett: {ex.Message}");
                }
                catch (Exception ex)
                {
                    // Fallback för alla övriga obekanta fel
                    Console.WriteLine($"Känt fel: {ex.Message}");
                }
                finally
                {
                    // Körs ALLTID, även om det blev undantag
                    Console.WriteLine("Cleanup: Logging startat anrop.");
                }

                Console.WriteLine("Programmet avslutas inkorrekt.");
            }

            // Exempel på metod som själv kastar ett undantag (throw)
            static double ProcessFile(string path, string fileName)
            {
                // Om filnamnet är tomt: logiskt fel vi vill signalera
                if (string.IsNullOrWhiteSpace(fileName))
                {
                    throw new ArgumentException(
                        "Filnamn får inte vara tomt eller null.",
                        nameof(fileName)
                    );
                }

                if (!File.Exists(fileName))
                {
                    throw new FileNotFoundException("Filen saknas.");
                }

                if (!Directory.Exists(path))
                {
                    throw new FileNotFoundException("Sökvägen saknas.");
                }

                var fullPath = Path.Combine(path, fileName);

                StreamReader? reader = null;
                try
                {
                    reader = new StreamReader(fullPath);

                    string? line = reader.ReadLine();
                    if (line == null)
                        throw new InvalidOperationException("Filen är tom.");

                    // Försöker omvandla text till tal
                    int number = int.Parse(line); // Kan ge FormatException

                    // Division: kan ge DivideByZeroException
                    return 100.0 / number;
                }
                catch (FormatException ex)
                {
                    // Vi kan logga eller omformulera felet
                    Console.WriteLine($"Formatfel i ProcessFile: {ex.Message}");
                    // Vi kan välja att låta metoden "kasta upp" felet
                    throw; // När du i `catch` bara vill logga/analysera,
                    // men låta anroparen (t.ex. en högre nivå i applikationen)
                    // bestämma hur man ska återhämta sig.
                }
                catch (Exception ex)
                {
                    // Om vi vill ge en mer meningsfull feltyp till anroparen
                    throw new InvalidOperationException("Det gick inte att processa filen.", ex); // InnerException = ursprunglig fel
                }
                finally
                {
                    // Garanterad stängning av resurs
                    reader?.Close();
                    Console.WriteLine("finally i ProcessFile: StreamReader stängd.");
                }
            }
        }
    }
}
