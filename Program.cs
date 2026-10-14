using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Day_01_G03;

class AnagramComparer : IEqualityComparer<string>
{
    private static string Key(string s) => new string(s.Trim().ToCharArray().OrderBy(c => c).ToArray());
    public bool Equals(string? x, string? y) => Key(x ?? "") == Key(y ?? "");
    public int GetHashCode(string obj) => Key(obj).GetHashCode();
}

class Program
{
    static string FindFile(string name)
    {
        foreach (var start in new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
        {
            var dir = new DirectoryInfo(start);
            while (dir != null)
            {
                var direct = Path.Combine(dir.FullName, name);
                if (File.Exists(direct)) return direct;
                var sub = Path.Combine(dir.FullName, "Data", name);
                if (File.Exists(sub)) return sub;
                dir = dir.Parent;
            }
        }
        throw new FileNotFoundException(name);
    }

    static void Title(string t) => Console.WriteLine($"\n===== {t} =====");

    static void Main()
    {
        var ProductList = ListGenerator.ProductsList;
        var CustomerList = ListGenerator.CustomersList;
        int[] Arr = { 5, 4, 1, 3, 9, 8, 6, 7, 2, 0 };
        string[] words = File.ReadAllLines(FindFile("dictionary_english.txt"));

        // ================= Element Operators =================
        Title("Element 1: first product out of stock");
        var outOfStock = ProductList.FirstOrDefault(p => p.UnitsInStock == 0);
        Console.WriteLine(outOfStock?.ToString() ?? "null");

        Title("Element 2: first product with Price > 1000 (or null)");
        var expensive = ProductList.FirstOrDefault(p => p.UnitPrice > 1000);
        Console.WriteLine(expensive?.ToString() ?? "null");

        Title("Element 3: second number greater than 5");
        Console.WriteLine(Arr.Where(n => n > 5).ElementAt(1));

        // ================= Aggregate Operators =================
        Title("Aggregate 1: count of odd numbers");
        Console.WriteLine(Arr.Count(n => n % 2 != 0));

        Title("Aggregate 2: customers and number of orders");
        foreach (var c in CustomerList.Select(c => new { c.CustomerID, c.CustomerName, OrdersCount = c.Orders.Count() }))
            Console.WriteLine($"{c.CustomerID} - {c.CustomerName} : {c.OrdersCount}");

        Title("Aggregate 3: categories and number of products");
        foreach (var g in ProductList.GroupBy(p => p.Category).Select(g => new { Category = g.Key, Count = g.Count() }))
            Console.WriteLine($"{g.Category} : {g.Count}");

        Title("Aggregate 4: sum of the array");
        Console.WriteLine(Arr.Sum());

        Title("Aggregate 5: total characters of all words");
        Console.WriteLine(words.Sum(w => w.Length));

        Title("Aggregate 6: shortest word length");
        Console.WriteLine(words.Min(w => w.Length));

        Title("Aggregate 7: longest word length");
        Console.WriteLine(words.Max(w => w.Length));

        Title("Aggregate 8: average word length");
        Console.WriteLine(words.Average(w => w.Length));

        Title("Aggregate 9: total units in stock per category");
        foreach (var g in ProductList.GroupBy(p => p.Category).Select(g => new { Category = g.Key, Total = g.Sum(p => p.UnitsInStock) }))
            Console.WriteLine($"{g.Category} : {g.Total}");

        Title("Aggregate 10: cheapest price per category");
        foreach (var g in ProductList.GroupBy(p => p.Category).Select(g => new { Category = g.Key, Min = g.Min(p => p.UnitPrice) }))
            Console.WriteLine($"{g.Category} : {g.Min}");

        Title("Aggregate 11: cheapest products per category (let)");
        var cheapest = from p in ProductList
                       group p by p.Category into g
                       let minPrice = g.Min(p => p.UnitPrice)
                       select new { Category = g.Key, Products = g.Where(p => p.UnitPrice == minPrice) };
        foreach (var g in cheapest)
        {
            Console.WriteLine(g.Category);
            foreach (var p in g.Products) Console.WriteLine($"   {p.ProductName} - {p.UnitPrice}");
        }

        Title("Aggregate 12: most expensive price per category");
        foreach (var g in ProductList.GroupBy(p => p.Category).Select(g => new { Category = g.Key, Max = g.Max(p => p.UnitPrice) }))
            Console.WriteLine($"{g.Category} : {g.Max}");

        Title("Aggregate 13: most expensive products per category");
        var priciest = from p in ProductList
                       group p by p.Category into g
                       let maxPrice = g.Max(p => p.UnitPrice)
                       select new { Category = g.Key, Products = g.Where(p => p.UnitPrice == maxPrice) };
        foreach (var g in priciest)
        {
            Console.WriteLine(g.Category);
            foreach (var p in g.Products) Console.WriteLine($"   {p.ProductName} - {p.UnitPrice}");
        }

        Title("Aggregate 14: average price per category");
        foreach (var g in ProductList.GroupBy(p => p.Category).Select(g => new { Category = g.Key, Avg = g.Average(p => p.UnitPrice) }))
            Console.WriteLine($"{g.Category} : {g.Avg}");

        // ================= Set Operators =================
        Title("Set 1: unique category names");
        foreach (var c in ProductList.Select(p => p.Category).Distinct()) Console.WriteLine(c);

        var productLetters = ProductList.Select(p => p.ProductName[0]);
        var customerLetters = CustomerList.Select(c => c.CustomerName[0]);

        Title("Set 2: unique first letters (products + customers)");
        Console.WriteLine(string.Join(", ", productLetters.Union(customerLetters)));

        Title("Set 3: common first letters");
        Console.WriteLine(string.Join(", ", productLetters.Intersect(customerLetters)));

        Title("Set 4: product first letters not in customers");
        Console.WriteLine(string.Join(", ", productLetters.Except(customerLetters)));

        Title("Set 5: last three characters of all names (with duplicates)");
        var lastThree = CustomerList.Select(c => c.CustomerName)
                        .Concat(ProductList.Select(p => p.ProductName))
                        .Select(n => n.Length >= 3 ? n.Substring(n.Length - 3) : n);
        Console.WriteLine(string.Join(", ", lastThree));

        // ================= Partitioning Operators =================
        var waOrders = CustomerList.Where(c => c.Region == "WA").SelectMany(c => c.Orders);

        Title("Partitioning 1: first 3 orders from Washington customers");
        foreach (var o in waOrders.Take(3)) Console.WriteLine(o);

        Title("Partitioning 2: all but the first 2 orders from Washington customers");
        foreach (var o in waOrders.Skip(2)) Console.WriteLine(o);

        Title("Partitioning 3: TakeWhile number >= its index");
        Console.WriteLine(string.Join(", ", Arr.TakeWhile((n, i) => n >= i)));

        Title("Partitioning 4: SkipWhile not divisible by 3");
        Console.WriteLine(string.Join(", ", Arr.SkipWhile(n => n % 3 != 0)));

        Title("Partitioning 5: SkipWhile number >= its index");
        Console.WriteLine(string.Join(", ", Arr.SkipWhile((n, i) => n >= i)));

        // ================= Quantifiers =================
        Title("Quantifiers 1: any word contains 'ei'");
        Console.WriteLine(words.Any(w => w.Contains("ei")));

        Title("Quantifiers 2: categories with at least one out-of-stock product");
        foreach (var g in ProductList.GroupBy(p => p.Category).Where(g => g.Any(p => p.UnitsInStock == 0)))
        {
            Console.WriteLine(g.Key);
            foreach (var p in g) Console.WriteLine($"   {p.ProductName} (stock {p.UnitsInStock})");
        }

        Title("Quantifiers 3: categories with all products in stock");
        foreach (var g in ProductList.GroupBy(p => p.Category).Where(g => g.All(p => p.UnitsInStock > 0)))
        {
            Console.WriteLine(g.Key);
            foreach (var p in g) Console.WriteLine($"   {p.ProductName} (stock {p.UnitsInStock})");
        }

        // ================= Grouping Operators =================
        Title("Grouping 1: numbers by remainder when divided by 5");
        List<int> numbers = new List<int> { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 };
        foreach (var g in numbers.GroupBy(n => n % 5).OrderBy(g => g.Key))
        {
            Console.WriteLine($"Numbers with a remainder of {g.Key} when divided by 5:");
            foreach (var n in g) Console.WriteLine(n);
        }

        Title("Grouping 2: words by first letter (count + first 5 words per letter)");
        foreach (var g in words.Where(w => w.Length > 0).GroupBy(w => char.ToLower(w[0])).OrderBy(g => g.Key))
            Console.WriteLine($"{g.Key} ({g.Count()}): {string.Join(", ", g.Take(5))}");

        Title("Grouping 3: anagram groups (custom comparer)");
        string[] anagrams = { "from", "salt", "earn", " last", "near", "form" };
        foreach (var g in anagrams.GroupBy(w => w.Trim(), new AnagramComparer()))
        {
            foreach (var w in g) Console.WriteLine(w.Trim());
            Console.WriteLine("....");
        }
    }
}
