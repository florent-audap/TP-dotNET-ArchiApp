namespace app;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Hello, World!");
        
        Person p = new Person { Nom = "Alice", Age = 30 };

        string message = p.Hello(true);
        Console.WriteLine(message);
    }
};


class Person
{
    public string Nom { get; set; }
    public int Age { get; set; }
    
    // Méthode Hello avec logique conditionnelle
    public string Hello(bool isLowercase)
    {
        string resultat = $"hello {Nom}, you are {Age}";

        if (isLowercase)
        {
            return resultat;
        }
        else
        {
            return resultat.ToUpper();
        }
    }
}
