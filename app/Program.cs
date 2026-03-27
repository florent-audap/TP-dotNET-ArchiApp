using System;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics; // Indispensable pour Stopwatch
using Newtonsoft.Json;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

namespace app;

class Program
{
    static void Main(string[] args)
    {
        // 1. JSON (inchangé)
        Person p = new Person { Nom = "Alice", Age = 30 };
        string message = p.Hello(true);
        Console.WriteLine(message);
        Console.WriteLine(JsonConvert.SerializeObject(p, Formatting.Indented));

        // 2. Configuration dossiers
        string source = "ImagesSource";
        string sortie = "ImagesResultat";
        Directory.CreateDirectory(source);
        Directory.CreateDirectory(sortie);

        string[] fichiers = Directory.GetFiles(source, "*.*");

        if (fichiers.Length == 0)
        {
            Console.WriteLine($"\nAjoutez des images dans {source} pour tester la performance.");
            return;
        }

        // --- DÉBUT DE LA MESURE ---
        Console.WriteLine($"\nTraitement de {fichiers.Length} images...");
        Stopwatch sw = Stopwatch.StartNew(); 

        Parallel.ForEach(fichiers, chemin => 
        {
            using (Image img = Image.Load(chemin))
            {
                img.Mutate(x => x.Resize(400, 0).Grayscale());
                img.Save(Path.Combine(sortie, Path.GetFileName(chemin)));
            }
        });

        sw.Stop(); 
        // --- FIN DE LA MESURE ---

        Console.WriteLine($"\nTerminé en : {sw.ElapsedMilliseconds} ms");
        Console.WriteLine($"Moyenne par image : {sw.ElapsedMilliseconds / fichiers.Length} ms");
    }
}

class Person
{
    public required string Nom { get; set; }
    public int Age { get; set; }

    public string Hello(bool isLowercase)
    {
        string msg = $"hello {Nom}, you are {Age}";
        return isLowercase ? msg : msg.ToUpper();
    }
}