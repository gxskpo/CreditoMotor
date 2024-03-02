using Newtonsoft.Json;

namespace Aguacate;

public struct Data(
    Dictionary<string, Dictionary<string, int[]>?> masc,
    Dictionary<string, Dictionary<string, int[]>?> fem)
{
    public Dictionary<string, Dictionary<string, int[]>?> Masc { get; } = masc;
    public Dictionary<string, Dictionary<string, int[]>?> Fem { get; } = fem;
}

public class Motorcito
{
    private static Data? DictData { get; set; }

    private const int MaximoMasc = 30;
    private const int MaximoFem = 28;


    private static double GetOptimal(int min, int max)
    {
        double p1 = min - Math.Sqrt(max - min);
        double p2 = min + 0.0175 * (max - min);
        return Math.Max(p1, p2);
    }

    private static string GetMonthDifference(DateTime date1, DateTime date2, int limit)
    {
        int res = (date1.Year - date2.Year) * 12 + date1.Month - date2.Month;
        return Math.Min(res, limit).ToString();
    }

    static void PrintResult(int min, int max, double optimo)
    {
        Console.WriteLine($"Maximo: {max} Minimo: {min} Óptimo: {optimo}");
    }

    public void CalculoMotor(char tipoNomina, DateTime fechaPrimerEmpleo, char genero)
    {
        DateTime today = DateTime.Now;
        Dictionary<string, Dictionary<string, int[]>?> dict =
            genero == 'F' ? DictData!.Value.Fem : DictData!.Value.Masc;
        int limit = genero == 'F' ? MaximoFem : MaximoMasc;
        string experience = GetMonthDifference(today, fechaPrimerEmpleo, limit);
        bool exists = dict.TryGetValue(experience, out var data);
        if (!exists)
        {
            data = dict["0"];
        }

        int min = data![tipoNomina.ToString()][0];
        int max = data[tipoNomina.ToString()][1];
        PrintResult(min, max, GetOptimal(min, max));
    }

    public Motorcito(string fileName)
    {
        string text = File.ReadAllText(fileName);
        Data? data = JsonConvert.DeserializeObject<Data>(text);
        if (data == null)
        {
            Console.WriteLine("No valid data found.");
            return;
        }

        DictData = data;
    }
}

public abstract class Program
{
    public static void Main(string[] args)
    {
        Motorcito motor = new("./z.json");
        motor.CalculoMotor('A', new DateTime(2022, 6, 12), 'F');
        motor.CalculoMotor('B', new DateTime(1993, 12, 30), 'F');
        motor.CalculoMotor('C', new DateTime(2020, 9, 19), 'M');
        motor.CalculoMotor('D', new DateTime(2019, 1, 15), 'M');
    }
}