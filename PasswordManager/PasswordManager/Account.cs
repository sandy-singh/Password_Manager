/*
 * Program:         PasswordManager.exe
 * Module:          Account.cs
 * Date:            June 06, 2019
 * Author:          Sandeep Singh
 * Description:     C# classes which help in running the main program
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountInventory
{
    //Account Class(individual) which contains account info
    class Account
    {
        public string userId { get; set; }
        public List<Password> passDb = new List<Password>(); //List of Password Objects

        //Helper Method
        public void addPass(Password p)
        {
            passDb.Add(p);
        }
        public string loginUrl { get; set; }
        public string AccountNum { get; set; }
   
    }

    //Password Class
    class Password
    {
        public string value { get; set; }
        public ushort StrengthNum { get; set; }
        public string StrengthText { get; set; }
        public string LastReset { get; set; }

    }
    
    //Company class which contains Accounts dor that company 
    class Company
    {
        public string name { get; set; }
        public List<Account> Inventory = new List<Account>(); //Lsit of account Objects

        //Helper Method
        public void addAcc(Account acc)
        {
            Inventory.Add(acc);
        }
    }

    //Company Inventory Class which contains the list of companies
    class CompanyInventory
    {
        public List<Company> db = new List<Company>();
        
        //Helper Method
        public void addComp(Company c)
        {
            db.Add(c);
        }
    }
}
