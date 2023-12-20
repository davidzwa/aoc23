namespace q6;

public class Question
{
    public static double PolyEquation(double tr, double th, double d)
    {
        // Each millisecond holding, speed increases one millimeter/s
        // tr = race time ms
        // th = holding time ms
        // tr1 = leftover time = tr - th 
        // v = 1 * th
        // d = v * Tr1
        // Equation d = Th * (Tr - Th)
        // -Th^2 + Th*Tr - d = 0
        // a = -1, b = 1, c = -d

        // D = b^2 - 4ac
        // Z0 = (-b + sqrt(b^2-4ac))/(2a)
        // Z1 = (-b - sqrt(b^2-4ac))/(2a)
        return -th * -th + th * tr - d;
    }

    public static double Equation(double tr, double th)
    {
        return th * (tr - th);
    }

    public static double Discriminant(long a, long b, long c)
    {
        return Math.Pow(b, 2) - 4 * a * c;
    }

    public static (double ThMin, double ThMax) Zeroes(long a, long b, long c)
    {
        var d = Discriminant(a, b, c);
        if (d < 0)
        {
            throw new Exception("No options");
        }

        var th1 = (-b - Math.Sqrt(d)) / (2 * a);
        var th2 = (-b + Math.Sqrt(d)) / (2 * a);
        return new(Math.Min(th1, th2), Math.Max(th1, th2));
    }

    public static (long Max, long Min) GetWinOptions(double thMin, double thMax)
    {
        var min = (int)Math.Ceiling(thMin);
        var max = (int)Math.Floor(thMax);

        Console.WriteLine($"min {min} max {max}");
        return new(max, min);
    }

    public static bool WillWin(long th, long distance, long time)
    {
        return Equation(time, th) >= distance;
    }

    public static long ProcessRace((long Time, long Distance, State State) race)
    {
        race.Distance += 1;
        // Question 1: given holding time, what is distance.
        // I expect a parabolic relation
        var (thMin, thMax) = Zeroes(-1, race.Time, -race.Distance);
        if (thMax < thMin)
        {
            Console.WriteLine("Exceptional");
        }

        // var distance = Equation(race.Time, thMax);
        // var distance2 = Equation(race.Time, thMin);
        // Console.WriteLine($"th0 {thMax} {distance} th1 {thMin} {distance2} race {race.Distance}");

        var result = GetWinOptions(thMin, thMax);
        if (!WillWin(result.Max, race.Distance, race.Time))
        {
            throw new Exception($"Wont win {result.Item1}");
        }

        if (!WillWin(result.Min, race.Distance, race.Time))
        {
            throw new Exception($"Wont win2 {result.Item1}");
        }

        if (result.Max < result.Min)
        {
            throw new Exception("Order");
        }

        race.State.WinOptions = result.Max - result.Min + 1;
        Console.WriteLine(race.State.WinOptions);


        // Does it win?
        // Question 2: get minimum winning speed
        // Question 3: get maximum winning speed
        
        if (race.State.WinOptions == 0)
        {
            throw new Exception("Race winOptions is 0");
        }

        return race.State.WinOptions;
    }
}