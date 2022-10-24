using Microsoft.CodeAnalysis.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

using VerifyCS = Ncss.Analyzer.Test.CSharpAnalyzerVerifier<Ncss.Analyzer.LlocAnalyzerAnalyzer>;

namespace Ncss.Analyzer.Test;

[TestClass]
public class NcssAnalyzerUnitTest
{

    //Diagnostic and CodeFix both triggered and checked for
    [TestMethod]
    public async Task TestWithMethod()
    {
        

        var test = @"
using System.Collections.Generic;

public class Item
{
    public string Name { get; set; }
    public int SellIn { get; set; }
    public int Quality { get; set; }

    public override string ToString()
    {
        return this.Name + "", "" + this.SellIn + "", "" + this.Quality;
    }
}
public class GildedRose
{
    IList<Item> Items;
    public GildedRose(IList<Item> Items)
    {
        this.Items = Items;
    }

    public void UpdateQuality()
    {
        for (var i = 0; i < Items.Count; i++)
        {
            if (Items[i].Name != ""Aged Brie"" && Items[i].Name != ""Backstage passes to a TAFKAL80ETC concert"")
            {
                if (Items[i].Quality > 0)
                {
                    if (Items[i].Name != ""Sulfuras, Hand of Ragnaros"")
                    {
                        Items[i].Quality = Items[i].Quality - 1;
                    }
                }
            }
            else
            {
                if (Items[i].Quality < 50)
                {
                    Items[i].Quality = Items[i].Quality + 1;

                    if (Items[i].Name == ""Backstage passes to a TAFKAL80ETC concert"")
                    {
                        if (Items[i].SellIn < 11)
                        {
                            if (Items[i].Quality < 50)
                            {
                                Items[i].Quality = Items[i].Quality + 1;
                            }
                        }

                        if (Items[i].SellIn < 6)
                        {
                            if (Items[i].Quality < 50)
                            {
                                Items[i].Quality = Items[i].Quality + 1;
                            }
                        }
                    }
                }
            }

            if (Items[i].Name != ""Sulfuras, Hand of Ragnaros"")
            {
                Items[i].SellIn = Items[i].SellIn - 1;
            }

            if (Items[i].SellIn < 0)
            {
                if (Items[i].Name != ""Aged Brie"")
                {
                    if (Items[i].Name != ""Backstage passes to a TAFKAL80ETC concert"")
                    {
                        if (Items[i].Quality > 0)
                        {
                            if (Items[i].Name != ""Sulfuras, Hand of Ragnaros"")
                            {
                                Items[i].Quality = Items[i].Quality - 1;
                            }
                        }
                    }
                    else
                    {
                        Items[i].Quality = Items[i].Quality - Items[i].Quality;
                    }
                }
                else
                {
                    if (Items[i].Quality < 50)
                    {
                        Items[i].Quality = Items[i].Quality + 1;
                    }
                }
            }
        }
    }
}
";

        var expected = VerifyCS.Diagnostic("NCSSAnalyzer").WithSpan(23, 5, 23, 11).WithArguments("UpdateQuality", 28);
        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [TestMethod]
    public async Task TestWithProperty()
    {
        var test = @"
using System;

namespace Ncss.Analyzer.Test
{

internal class PasswordSuggester
{
    static string Suggestion
    {
        get
        {
            int l = 2;
            int u = 3;
            int d = 4;
            int s = 3;

            string str = string.Empty;

            Random randm = new Random();
            string num = ""0123456789"";

            string low_case = ""abcdefghijklmnopqrstuvwxyz"";
            string up_case = ""ABCDEFGHIJKLMNOPQRSTUVWXYZ"";
            string spl_char = ""@#$_()!"";

            int pos = 0;

            if (l == 0)
            {

                pos = randm.Next(1, 1000) % str.Length;

                str = str.Insert(pos, low_case[randm.Next(1,
                                     1000) % 26].ToString());
            }

            if (u == 0)
            {
                pos = randm.Next(1, 1000) % str.Length;
                str = str.Insert(pos, up_case[randm.Next(
                                  1, 1000) % 26].ToString());
            }

            if (d == 0)
            {
                pos = randm.Next(1, 1000) % str.Length;
                str = str.Insert(pos, num[randm.Next(1, 1000)
                                          % 10].ToString());
            }

            if (s == 0)
            {
                pos = randm.Next(1, 1000) % str.Length;
                str = str.Insert(pos, spl_char[randm.Next(
                                  1, 1000) % 7].ToString());
            }

            return str;
        }
    }

}
}
";
        var expected = VerifyCS.Diagnostic("NCSSAnalyzer").WithSpan(11, 9, 11, 12).WithArguments("Suggestion.get", 24);
        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }

    [TestMethod]
    public async Task TestWithIndexer()
    {
        var test = @"
using System;

namespace Ncss.Analyzer.Test
{

internal class PasswordSuggester
{
    public string this[string key]
    {
        get
        {
            int l = 2;
            int u = 3;
            int d = 4;
            int s = 3;

            string str = string.Empty;

            Random randm = new Random();
            string num = ""0123456789"";

            string low_case = ""abcdefghijklmnopqrstuvwxyz"";
            string up_case = ""ABCDEFGHIJKLMNOPQRSTUVWXYZ"";
            string spl_char = ""@#$_()!"";

            int pos = 0;

            if (l == 0)
            {

                pos = randm.Next(1, 1000) % str.Length;

                str = str.Insert(pos, low_case[randm.Next(1,
                                     1000) % 26].ToString());
            }

            if (u == 0)
            {
                pos = randm.Next(1, 1000) % str.Length;
                str = str.Insert(pos, up_case[randm.Next(
                                  1, 1000) % 26].ToString());
            }

            if (d == 0)
            {
                pos = randm.Next(1, 1000) % str.Length;
                str = str.Insert(pos, num[randm.Next(1, 1000)
                                          % 10].ToString());
            }

            if (s == 0)
            {
                pos = randm.Next(1, 1000) % str.Length;
                str = str.Insert(pos, spl_char[randm.Next(
                                  1, 1000) % 7].ToString());
            }

            return str;
        }
    }

}
}
";
        var expected = VerifyCS.Diagnostic("NCSSAnalyzer").WithSpan(11, 9, 11, 12).WithArguments("this[string key].get", 24);
        await VerifyCS.VerifyAnalyzerAsync(test, expected);
    }
}
