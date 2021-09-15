using System;
using System.Collections.Generic;
using CsvHelper;
using System.IO;
using Newtonsoft.Json;
using System.Globalization;
using System.Text.RegularExpressions;


namespace DataProj
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("**************DataProject**************");
            Console.WriteLine();
            Console.WriteLine();

            /*1st --capital range of companies*/
            Dictionary<string, double> authCap = new Dictionary<string, double>()
            {
                ["<=1L"] = 0,
                ["1L to 10L"] = 0,
                ["10L to 1Cr"] = 0,
                ["1Cr to 10Cr"] = 0,
                [">10Cr"] = 0,

            };

            /*2nd  --no of Company regs per year */
            Dictionary<int, int> regYears = new Dictionary<int, int>();
            for (int i = 2000; i <= 2019; i++) regYears[i] = 0;

            /*3rd --no. of times a company reg by pba in 2015 */
            Dictionary<string, int> principalAct = new Dictionary<string, int>();

            /*4th --no. of times a company grouped on both year and pba from the year 2000-19 */
            var companyGrouped = new SortedDictionary<int, Dictionary<string, int>>();

            /*1st sol*/
            using (var streamReader = new StreamReader(@"C:\Users\Ammu\Downloads\WestBengal.csv"))
            {
                using (var csvRead = new CsvReader(streamReader, CultureInfo.InvariantCulture))
                {
                    var recs = csvRead.GetRecords<dynamic>();
                    foreach (var rec in recs)
                    {
                        double capital = Double.Parse(rec.AUTHORIZED_CAP);
                        string years = rec.DATE_OF_REGISTRATION;

                        /*YearStr var used in 2nd,3rd and 4th solutions*/
                        string yearStr = "20" + years.Substring(years.Length - 2);

                        /* principal used in 3rd and 4th solutions */
                        string principal = rec.PRINCIPAL_BUSINESS_ACTIVITY_AS_PER_CIN;

                        /*year var is used in 2nd,3rd and 4th solutions*/
                        int year = 0;

                        /*if loop to make sure registration field is not NA */
                        if (!yearStr.Contains("NA")) year = Int32.Parse(yearStr);

                        /*1st sol comp's capital range */
                        if (capital <= 100000) authCap["<=1L"]++;
                        else if (capital >= 100000 && capital <= 1000000) authCap["1L to 10L"]++;
                        else if (capital >= 1000000 && capital <= 10000000) authCap["10L to 1Cr"]++;
                        else if (capital >= 10000000 && capital <= 100000000) authCap["1Cr to 10Cr"]++;
                        else if (capital >= 10000000) authCap[">10Cr"]++;

                        /*2nd sol no of regs per year*/
                        if (Regex.IsMatch(yearStr, "^[0-9]*$"))
                        {
                            if (year >= 2000 && year <= 2019) regYears[year]++;
                        } //end of 2nd sol

                        /*3rd sol condition is reg year must be 2015 and pba not equal to NA */
                        if (year == 2015 && !(principal.Equals("NA")))
                        {
                            if (!principalAct.ContainsKey(principal)) principalAct.Add(principal, 1);
                            else principalAct[principal]++;
                        } //end of 3rd sol

                        /*4th sol reg n pba field must not be NA */
                        if (!(principal.Equals("NA")))
                        {
                            if ((year >= 2000 && year <= 2019))
                            {
                                //adding element to main dictionary
                                if (!companyGrouped.ContainsKey(year)) companyGrouped.Add(year, new Dictionary<string, int>());
                                else
                                {
                                    //sub dictionary
                                    if (!companyGrouped[year].ContainsKey(principal)) companyGrouped[year].Add(principal, 1);
                                    else companyGrouped[year][principal]++;
                                }
                            }
                        } //end of 4th sol

                    } //2nd using loop's
                } //1st using loop's
            }

            //string[] l = {$"{authCap}",$"{regYears}",$"{principalAct}",$"{companyGrouped}"};

            //Serializing the object to json file.
            String json1 = JsonConvert.SerializeObject(authCap, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(@"D:/MountBlue/JsProj/jOne.json", json1);
            String json2 = JsonConvert.SerializeObject(regYears, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(@"D:/MountBlue/JsProj/jTwo.json", json2);
            String json3 = JsonConvert.SerializeObject(principalAct, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(@"D:/MountBlue/JsProj/jThree.json", json3);
            String json4 = JsonConvert.SerializeObject(companyGrouped, Newtonsoft.Json.Formatting.Indented);
            File.WriteAllText(@"D:/MountBlue/JsProj/jFour.json", json4);
            Console.WriteLine("success");
            Console.ReadLine();
        }
    }
}
