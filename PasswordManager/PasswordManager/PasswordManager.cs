/*
 * Program:         PasswordManager.exe
 * Module:          PasswordManager.cs
 * Date:            June 06, 2019
 * Author:          Sandeep Singh
 * Description:     Console Application that asks user to enter Account details and stores it 
 *                  in the form of JSON format. It also have applicability to review the account details later
 *                  and change password.
 */

 /*
  * READ ME!!!!
  * 
  * Schema and JSON Data file are in the debug folder
  * 
  */


//Used Libraries
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AccountInventory;
using System.IO;            // File class
using Newtonsoft.Json;  // JsonConvert class
using System.Text.RegularExpressions;
using Newtonsoft.Json.Schema; //Schema Library
using Newtonsoft.Json.Linq;

namespace PasswordManager
{
    class Program
    {

        //Main Method
        static void Main(string[] args)
        {
            //Start of the Program
            Console.WriteLine("\n              PASSWORD-MANAGER PREPARED BY SANDY - Copyright 2019\n");

            //Global Variables
            string temp;
            int value;
            bool restart = false; //bools used in while loops
            string Schema;



            /*
             * Note: It creates the JSON File in teh Debug Folder
             */
            var fileInfo = new FileInfo("CompanyInfo.json");



            //If it doesn't exist create new one or if it is empty proceed from here
            if (!fileInfo.Exists || fileInfo.Length == 0)
            {
                /*
                 * Introduction to the program
                 */
                Console.WriteLine("\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                Console.WriteLine("|                                Account Entries                                   |");
                Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n");
                Console.WriteLine("\nEMPTY! Please add Some Accounts");
                var myFile = File.Create("CompanyInfo.json"); //Create if empty
                myFile.Close();

                Console.WriteLine("\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                Console.WriteLine("|                                   Menu                                           |");
                Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                Console.Write("|Press A to add the Account                                                        |");
                Console.Write("\n|Press Q to Quit the Application                                                   |");
                Console.WriteLine("\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n");

                bool f = false; //To check for the valid input
                while (f == false)
                {
                    Console.Write("Enter Here: ");
                    temp = Console.ReadLine();
                    f = true;
                    if (temp.Equals("a") || temp.Equals("A")) //If the user wants to add the account information
                    {
                        CompanyInventory companyDb = new CompanyInventory(); //New company Database object, which will store the Accounts



                        bool complete; //To add as many accounts in the file

                        do
                        {

                            Company comp = new Company(); // New Company object
                            Console.WriteLine("\n\nPlease enter the following information...\n");
                            bool okay = false;

                            Console.Write("Decription: ");
                            while (okay == false)
                            {
                                okay = true;
                                comp.name = Console.ReadLine();
                                if (comp.name == "")
                                {
                                    //Validation
                                    Console.WriteLine("\nDescription is required!!\n");
                                    Console.Write("Description: ");
                                    okay = false;
                                }
                            }
                            Account acc = new Account();
                            Console.Write("UserID: ");
                            bool flag = false;
                            while (flag == false)
                            {
                                flag = true;
                                acc.userId = Console.ReadLine();
                                if (acc.userId == "")
                                {
                                    //Validation
                                    Console.WriteLine("\nUserID is required!!\n");
                                    Console.Write("UserID:");
                                    flag = false;
                                }

                            }
                            Password pass = new Password(); //New Password Object to store the password properties
                            bool good = false;
                            while (good == false)
                            {
                                good = true;


                                Console.Write("Password: ");
                                string pwText = Console.ReadLine();

                                pass.value = pwText;
                                //acc.password = pwText;
                                try
                                {
                                    // PasswordTester class demonstration
                                    PasswordTester pw = new PasswordTester(pwText);
                                    pass.StrengthText = pw.StrengthLabel;
                                    pass.StrengthNum = pw.StrengthPercent;
                                }
                                catch (ArgumentException)
                                {
                                    Console.WriteLine("ERROR: Invalid password format");

                                    good = false;
                                }
                                catch (InvalidOperationException)
                                {
                                    Console.WriteLine("Password is required!!!");

                                    good = false;
                                }
                            }
                            Console.Write("LoginUrl: ");
                            acc.loginUrl = Console.ReadLine();
                            //string pattern = "(\\http[s] ?:\\/\\/)?([^\\/\\s] +\\/)(.*)";

                            Console.Write("Account#: ");
                            acc.AccountNum = Console.ReadLine();

                            DateTime dateNow = DateTime.Now;
                            pass.LastReset = dateNow.ToShortDateString();


                            acc.addPass(pass);
                            comp.addAcc(acc);
                            companyDb.addComp(comp);

                            Console.Write("\nAdd another Account? (y/n): ");
                            complete = Console.ReadKey().KeyChar != 'y';
                            if (complete.Equals(true))
                            {
                                Console.ForegroundColor = ConsoleColor.Black;
                                Console.BackgroundColor = ConsoleColor.White;
                                Console.WriteLine("\n***Please restart the application***\n");
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.BackgroundColor = ConsoleColor.Black;
                            }


                        } while (!complete);



                        writeCompToJsonFile(companyDb);

                    }


                    else if (temp.Equals("q") || temp.Equals("Q"))
                    {
                        Console.WriteLine("\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                        Console.WriteLine("|                        Thank you for using my Application!                       |");
                        Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                        System.Environment.Exit(1);
                    }

                    else
                    {
                        Console.WriteLine("***Please enter Valid Input***");
                        f = false;
                    }

                }
            }



            else
            {

                while (restart == false) //used to repeat the process if user make mistake
                {
                    restart.Equals(true);
                    CompanyInventory readDb = new CompanyInventory(); //New Company DB to store the info
                    readDb = ReadJsonFile();
                    //Intro
                    Console.ForegroundColor = ConsoleColor.White;
                    Console.BackgroundColor = ConsoleColor.Black;
                    Console.WriteLine("\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                    Console.WriteLine("|                                Account Entries                                   |");
                    Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n");
                    if (readDb.db.Count == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.WriteLine("\nEMPTY! Please add Some Accounts\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Black;
                    }
                    int num = 1;
                    //If data exists, Show the Account Decription (Name)
                    foreach (Company company in readDb.db)
                    {
                        Console.Write((num) + $". {company.name}");
                        Console.WriteLine();
                        num = num + 1;
                    }
                    Console.WriteLine("\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                    Console.WriteLine("|                                   Menu                                           |");
                    Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                    Console.Write("|Press A to add the Account                                                        |");
                    Console.WriteLine("\n|Press # corresponding to the entry to read or update the Account Info             |");
                    Console.WriteLine("|Press Q to Quit the Application                                                   |");
                    Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n");
                    Console.Write("Enter Here: ");
                    temp = Console.ReadLine();
                    if (int.TryParse(temp, out value) || temp.Equals("Q") || temp.Equals("q") || temp.Equals("a") || temp.Equals("A"))
                    { }
                    else
                    {
                        //ERROR
                        Console.ForegroundColor = ConsoleColor.Black;
                        Console.BackgroundColor = ConsoleColor.White;
                        Console.WriteLine("\n***Please enter valid input!!***\n");
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.BackgroundColor = ConsoleColor.Black;
                    }

                    //If user wants to add
                    if (temp.Equals("a") || temp.Equals("A"))
                    {



                        bool complete2;
                        do
                        {

                            Company comp = new Company();

                            Console.WriteLine("\n\nPlease enter the following information...\n");
                            bool okay = false;
                            bool flag = false;
                            bool good = false;
                            Console.Write("Decription: ");

                            while (okay == false)
                            {
                                okay = true;
                                comp.name = Console.ReadLine();

                                if (comp.name == "")
                                {
                                    Console.ForegroundColor = ConsoleColor.Black;
                                    Console.BackgroundColor = ConsoleColor.White;
                                    Console.WriteLine("\n***Description is required!!***\n");
                                    Console.ForegroundColor = ConsoleColor.White;
                                    Console.BackgroundColor = ConsoleColor.Black;
                                    Console.Write("Description: ");
                                    okay = false;
                                }

                            }
                            Account acc = new Account();
                            Console.Write("UserID: ");

                            while (flag == false)
                            {
                                flag = true;
                                acc.userId = Console.ReadLine();
                                if (acc.userId == "")
                                {
                                    //ERROR Message
                                    Console.ForegroundColor = ConsoleColor.Black;
                                    Console.BackgroundColor = ConsoleColor.White;
                                    Console.WriteLine("\nUserID is required!!\n");
                                    Console.ForegroundColor = ConsoleColor.White;
                                    Console.BackgroundColor = ConsoleColor.Black;
                                    Console.Write("UserID:");
                                    flag = false;
                                }

                            }


                            Password pass = new Password();

                            while (good == false)
                            {
                                good = true;

                                Console.Write("Password: ");
                                string pwText = Console.ReadLine();

                                pass.value = pwText;
                                //acc.password = pwText;
                                try
                                {
                                    // PasswordTester class demonstration
                                    PasswordTester pw = new PasswordTester(pwText);
                                    pass.StrengthText = pw.StrengthLabel;
                                    pass.StrengthNum = pw.StrengthPercent;
                                }
                                catch (ArgumentException)
                                {
                                    //ERROR Message
                                    Console.ForegroundColor = ConsoleColor.Black;
                                    Console.BackgroundColor = ConsoleColor.White;
                                    Console.WriteLine("\nERROR: Invalid password format\n");
                                    Console.ForegroundColor = ConsoleColor.White;
                                    Console.BackgroundColor = ConsoleColor.Black;
                                    good = false;
                                }
                                catch (InvalidOperationException)
                                {
                                    //ERROR Message
                                    Console.ForegroundColor = ConsoleColor.White;
                                    Console.BackgroundColor = ConsoleColor.Black;
                                    Console.WriteLine("\nPassword is required!!!");
                                    Console.ForegroundColor = ConsoleColor.Black;
                                    Console.BackgroundColor = ConsoleColor.White;
                                    good = false;
                                }

                            }
                            Console.Write("LoginUrl: ");
                            acc.loginUrl = Console.ReadLine();
                           
                                Console.Write("Account#: ");
                            acc.AccountNum = Console.ReadLine();

                            DateTime dateNow = DateTime.Now;
                            pass.LastReset = dateNow.ToShortDateString();


                            acc.addPass(pass);
                            comp.addAcc(acc);
                            readDb.addComp(comp);

                            Console.Write("\nAdd another item? (y/n): ");
                            complete2 = Console.ReadKey().KeyChar != 'y';


                        } while (!complete2);


                        //Write the entered data to a JSON File
                        writeCompToJsonFile(readDb);
                        restart.Equals(false);

                    }

                    //if number(int) is entered, It will try to parse- If successful then will show the output and if not displays the error message
                    else if (int.TryParse(temp, out value))
                    {

                        int EntryNum = value - 1;



                        try
                        {
                            //Accessing the information based on the user input
                            Console.WriteLine("\n" + value + $". {readDb.db[EntryNum].name}");
                            Console.WriteLine();


                            foreach (Account account in readDb.db[EntryNum].Inventory)
                            {
                                string readpassword = "";
                                string readLabel = "";
                                ushort readNum = 0;
                                string readDate = "";
                                Console.WriteLine($"UserID: {account.userId}");
                                foreach (Password password in readDb.db[EntryNum].Inventory[0].passDb)
                                {
                                    readpassword = password.value;
                                    readLabel = password.StrengthText;
                                    readNum = password.StrengthNum;
                                    readDate = password.LastReset;
                                }
                                Console.WriteLine($"Password: {readpassword}");
                                Console.WriteLine($"Password Strength: {readLabel} ({readNum})%");
                                Console.WriteLine($"Password Reset Date: {readDate}");
                                Console.WriteLine($"LoginUrl: {account.loginUrl}");
                                Console.WriteLine($"Account #: {account.AccountNum}");
                            }




                            //Second Menu
                            Console.WriteLine("\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                            Console.WriteLine("| Press M to for Main Menu                                                         |");
                            Console.WriteLine("| Press P to update the password                                                   |");
                            Console.WriteLine("| Press D to delete the Account Info                                               |");
                            Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~\n");

                            Console.Write("Enter Here: ");
                            temp = Console.ReadLine();

                            //For Main Menu
                            if (temp.Equals("M") || temp.Equals("m"))
                            {
                                restart.Equals(false);
                            }
                            //For Password Change Request
                            else if (temp.Equals("P") || temp.Equals("p"))
                            {
                                Console.Write("\nNew Password: ");
                                string newPass = Console.ReadLine();
                                readDb.db[EntryNum].Inventory[0].passDb[0].value = newPass;
                                DateTime dateNow = DateTime.Now;
                                readDb.db[EntryNum].Inventory[0].passDb[0].LastReset = dateNow.ToShortDateString();
                                writeCompToJsonFile(readDb);
                                Console.ForegroundColor = ConsoleColor.White;
                                Console.BackgroundColor = ConsoleColor.Black;
                                Console.WriteLine("***Password Changed***");
                                Console.ForegroundColor = ConsoleColor.Black;
                                Console.BackgroundColor = ConsoleColor.White;
                                restart.Equals(false);
                            }
                            //To Delete the Account Information
                            else if (temp.Equals("D") || temp.Equals("d"))
                            {
                                try
                                {
                                    Console.Write("Delete?(Y/N): ");
                                    temp = Console.ReadLine();
                                    if (temp.Equals("Y") || temp.Equals("y"))
                                    {
                                        readDb.db.RemoveAt(EntryNum);
                                        writeCompToJsonFile(readDb);
                                        restart.Equals(false);
                                    }
                                    else if (temp.Equals("N") || temp.Equals("n"))
                                    {
                                        restart.Equals(false);
                                    }

                                    else
                                    {
                                        throw new IndexOutOfRangeException(); //Throws exception to handle the wrong input
                                    }
                                }
                                catch (Exception e) //Catches all the exceptions
                                {
                                    Console.ForegroundColor = ConsoleColor.White;
                                    Console.BackgroundColor = ConsoleColor.Black;
                                    Console.WriteLine("\n***Please enter a valid input!!***\n");
                                    Console.ForegroundColor = ConsoleColor.Black;
                                    Console.BackgroundColor = ConsoleColor.White;


                                }
                            }
                            else
                            {
                                throw new IndexOutOfRangeException();//Throws exception to handle the wrong input
                            }

                        }
                        catch (Exception e)  //Catches all the exceptions
                        {
                            Console.ForegroundColor = ConsoleColor.White;
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.WriteLine("\n***Please enter valid number!!***\n");
                            Console.ForegroundColor = ConsoleColor.Black;
                            Console.BackgroundColor = ConsoleColor.White;
                            restart.Equals(false);

                        }

                    }
                    //IF wants to quit
                    else if (temp.Equals("q") || temp.Equals("Q"))
                    {
                        Console.WriteLine("\n~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                        Console.WriteLine("|                        Thank you for using my Application!                       |");
                        Console.WriteLine("~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~");
                        System.Environment.Exit(1);
                    }
                }

            }
        }

        /*
         * Method Name: writeCompToJsonFile
         * Purpose: Converts the inputed data to a JSON file and also calls the validation method
         * Arguments: CompanyInventory Object
         * Output: Void
         */
        private static void writeCompToJsonFile(CompanyInventory compInv)
        {
            //Serlialization
            string json = JsonConvert.SerializeObject(compInv);
            string data = json;
            string Schema = "";
            ReadFile("Schema.json", out Schema);

            IList<string> messages;
            if (ValidateEnteredData(data, Schema, out messages)) // Note: messages parameter is optional
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine($"\nData file is valid.\n");
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.White;
                File.WriteAllText("CompanyInfo.json", json);

            }
            else
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.BackgroundColor = ConsoleColor.Black;
                Console.WriteLine($"\nERROR:\tData file is invalid.\n");
                Console.ForegroundColor = ConsoleColor.Black;
                Console.BackgroundColor = ConsoleColor.White;

                // Report validation error messages
                foreach (string msg in messages)
                    Console.WriteLine($"\t{msg}");
            }

        }

        /*
       * Method Name: ReadJsonFile
       * Purpose: Reads the specified JSON File and also Deserlize it
       * Arguments: Nothing
       * Output:CompanyInventory Object
       */
        private static CompanyInventory ReadJsonFile()
        {
            string json = File.ReadAllText("CompanyInfo.json");
            return JsonConvert.DeserializeObject<CompanyInventory>(json);

        }

        /*
       * Method Name: ReadFile
       * Purpose: Just reads the file, used to read the Schema
       * Arguments: path of the file
       * Output: True/False if seccessful/unsuccessful in reading the file and also outputs the read file (only if true)
       */
        private static bool ReadFile(string path, out string json)
        {
            try
            {
                // Read JSON file data 
                json = File.ReadAllText(path);
                return true;
            }
            catch
            {
                json = null;
                return false;
            }
        }

        /*
       * Method Name: ValidateEnteredData
       * Purpose: Validates the JSON file against the specified Schema
       * Arguments: Json File, Schema File
       * Output: True-If successful in validating and False- if not along with the error messages
       */
        private static bool ValidateEnteredData(string json, string Schema, out IList<string> messages)
        {
            JSchema schema = JSchema.Parse(Schema);
            JObject account = JObject.Parse(json);
            return account.IsValid(schema, out messages);
        }





    } // end class


}
