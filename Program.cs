﻿using NLog;
using System.Linq;
using Northwind_Console_Net06.Model;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

// See https://aka.ms/new-console-template for more information
string path = Directory.GetCurrentDirectory() + "\\nlog.config";

// create instance of Logger
var logger = LogManager.LoadConfiguration(path).GetCurrentClassLogger();
logger.Info("Program started");

try
{
    var db = new NWConsole_23_JPSContext();
    string choice;
    do
    {
        Console.WriteLine("1. Display Categories");
        Console.WriteLine("2. Add Category");
        Console.WriteLine("3. Display Products");
        Console.WriteLine("6. Add Product");
        Console.WriteLine("7. Edit Product");
        //Console.WriteLine();
        Console.WriteLine("\"q\" to quit");
        choice = Console.ReadLine();
        Console.Clear();
        logger.Info($"Option {choice} selected");
        if (choice == "1")
        {
            Console.WriteLine("Would you like to:\n1. Display All Categories\n2. Display Category and related products\n3. Display all Categories and their related products\n4. Display Specific Category");
            string answer = Console.ReadLine();

            if (answer == "1")
            {
                var query = db.Categories.OrderBy(p => p.CategoryName);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{query.Count()} records returned");
                Console.ForegroundColor = ConsoleColor.Magenta;
                foreach (var item in query)
                {
                    Console.WriteLine($"{item.CategoryName} - {item.Description}");
                }
                Console.ForegroundColor = ConsoleColor.White;
            }

            else if (answer == "2")
            {

                var query = db.Categories.OrderBy(p => p.CategoryId);

                Console.WriteLine("Select the category whose products you want to display:");
                Console.ForegroundColor = ConsoleColor.DarkRed;
                foreach (var item in query)
                {
                    Console.WriteLine($"{item.CategoryId}. {item.CategoryName}");
                }
                Console.ForegroundColor = ConsoleColor.White;
                int id = int.Parse(Console.ReadLine());
                Console.Clear();
                logger.Info($"CategoryId {id} selected");
                Category category = db.Categories.Include("Products").FirstOrDefault(c => c.CategoryId == id);
                Console.WriteLine($"{category.CategoryName} - {category.Description}");
                foreach (Product p in category.Products)
                {
                    Console.WriteLine($"\t{p.ProductName}");
                }

            }

            else if (answer == "3")
            {

                var query = db.Categories.Include("Products").OrderBy(p => p.CategoryId);
                foreach (var item in query)
                {
                    Console.WriteLine($"{item.CategoryName}");
                    foreach (Product p in item.Products)
                    {
                        Console.WriteLine($"\t{p.ProductName}");
                    }
                }

            }

            else if (answer == "4")

            {

                Console.WriteLine("Enter Category Name:");
                string name = Console.ReadLine();

               var query = db.Categories.Where(c => c.CategoryName.Contains(name)).Include("Products").OrderBy(c => c.CategoryName);
                foreach (var item in query)
                {
                    Console.WriteLine($"{item.CategoryName}");
                    foreach (Product p in item.Products)
                    {
                        Console.WriteLine($"\t{p.ProductName}");
                    }
                }

            }

        }

        else if (choice == "2")
        {
             Category category = AddCategory(db, logger);
             if (category != null)
             {
                 db.Add(category);
                 logger.Info("Category added - {name}", category.CategoryName);
             }

        }

        else if (choice == "3")
        {
            Console.WriteLine($"Would you like to see:\n 1.All Products\n 2.Display Specific Product");
            string selection = Console.ReadLine();

            if (selection == "1")
            {
                Console.WriteLine("Would you like to:\n 1.Display All Products?\n 2.Display Active Products\n3.Display Discontinued Products?");
                string answer = Console.ReadLine();

                if (answer == "1")
                {

                    var query = db.Products.OrderBy(p => p.ProductId);

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{query.Count()} records returned");
                    Console.ForegroundColor = ConsoleColor.Blue;

                    foreach (var item in query)
                    {
                        Console.WriteLine($"{item.ProductId}. {item.ProductName}");
                    }

                    Console.ForegroundColor = ConsoleColor.White;

                }
                else if (answer == "2")
                {
                    var query = db.Products.Where(p => p.Discontinued == false);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($"{query.Count()} records returned");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    foreach (var item in query)
                    {
                        Console.WriteLine($"{item.ProductId}. {item.ProductName}");
                    }

                    Console.ForegroundColor = ConsoleColor.White;
                }
                else if (answer == "3")
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    var query = db.Products.Where(p => p.Discontinued);
                    Console.WriteLine($"{query.Count()} records returned");
                    Console.ForegroundColor = ConsoleColor.Blue;
                    foreach (var item in query)
                    {
                        Console.WriteLine($"{item.ProductId}. {item.ProductName}");
                    }

                    Console.ForegroundColor = ConsoleColor.White;
                }
            }

            else if (selection == "2")
            {

                Console.WriteLine("Name of Product?:");
                string name = Console.ReadLine();

                var query = db.Products.Where(p => p.ProductName.Contains(name));
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{query.Count()} records returned");
                Console.ForegroundColor = ConsoleColor.Blue;
                foreach (var item in query)
                {
                    Console.WriteLine($"Product ID:{item.ProductId}\nProduct Name:{item.ProductName}\nSupplier ID:{item.SupplierId}\nCategory ID:{item.CategoryId}\nQuantity:{item.QuantityPerUnit}\nUnit Price:{item.UnitPrice}\nUnits In Stock:{item.UnitsInStock}\nUnits On Order:{item.UnitsOnOrder}\nReOrder Level:{item.ReorderLevel}");
                }

                Console.ForegroundColor = ConsoleColor.White;

            }


        }
        else if (choice == "6")
        {

            Product product = AddProduct(db, logger);
            if (product != null)
            {
                db.AddProduct(product);
                logger.Info("Product added - {name}", product.ProductName);
            }

        }
        else if (choice == "7")
        {
            Console.WriteLine("Choose the Product to edit:");
            var product = GetProduct(db, logger);
            if (product != null)
            {
                
                Product UpdatedProduct = EditProduct(db, logger);
                if (UpdatedProduct != null)
                {
                    UpdatedProduct.ProductId = product.ProductId;
                    db.EditProduct(UpdatedProduct);
                    logger.Info($"Product (id: {product.ProductId}) updated");
                }
            }
        }


        Console.WriteLine();

    } while (choice.ToLower() != "q");
}
catch (Exception ex)
{
    logger.Error(ex.Message);
}

Product AddProduct(NWConsole_23_JPSContext db, Logger logger)
{
    Product product = new Product();
    Console.WriteLine("Enter the new Product name:");
    product.ProductName = Console.ReadLine();
    Console.WriteLine("Enter the Supplier ID:");
    product.SupplierId = Convert.ToInt32(Console.ReadLine());
    Console.WriteLine("Enter the Category ID:");
    product.CategoryId = Convert.ToInt32(Console.ReadLine());
    Console.WriteLine("Enter the Quantity per Unit:");
    product.QuantityPerUnit = Console.ReadLine();
    Console.WriteLine("Enter the Unit Price:");
    product.UnitPrice = Convert.ToDecimal(Console.ReadLine());
    Console.WriteLine("Enter the number of Units in Stock:");
    product.UnitsInStock = Convert.ToInt16(Console.ReadLine());
    Console.WriteLine("Enter the number of Units on Order:");
    product.UnitsOnOrder = Convert.ToInt16(Console.ReadLine());
    Console.WriteLine("Enter the ReOrder level:");
    product.ReorderLevel = Convert.ToInt16(Console.ReadLine());
    Console.WriteLine("Is the product discontinued?(Y/N):");
    string discon = Console.ReadLine();
    if(discon == "y")
    {
        product.Discontinued = true;
    }

    ValidationContext context = new ValidationContext(product, null, null);
    List<ValidationResult> results = new List<ValidationResult>();

    var isValid = Validator.TryValidateObject(product, context, results, true);
    if (isValid)
    {
        
        if (db.Products.Any(p => p.ProductName == product.ProductName))
        {
            // generate error
            results.Add(new ValidationResult("Product exists", new string[] { "Name" }));
        }
        else
        {
            return product;
        }

        foreach (var result in results)
        {
            logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
        }

    }

    return null;
}

Category AddCategory(NWConsole_23_JPSContext db, Logger logger)
{
    Category category = new Category();
    Console.WriteLine("What is the name of the new Category:");
    category.CategoryName = Console.ReadLine();
    Console.WriteLine("What is the description of the Category:");
    category.Description = Console.ReadLine();
    

    ValidationContext context = new ValidationContext(category, null, null);
    List<ValidationResult> results = new List<ValidationResult>();

    var isValid = Validator.TryValidateObject(category, context, results, true);
    if (isValid)
    {
      
        if (db.Categories.Any(c => c.CategoryName == category.CategoryName))
        {
           
            results.Add(new ValidationResult("Category exists", new string[] { "Name" }));
        }
        else
        {
            return category;
        }

        foreach (var result in results)
        {
            logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
        }

    }

    return null;
}

 

    

     
 logger.Info("Program ended");

 static Product GetProduct(NWConsole_23_JPSContext db, Logger logger)
{
    
    var products = db.Products.OrderBy(p => p.ProductId);
    foreach (Product p in products)
    {
        Console.WriteLine($"{p.ProductId}: {p.ProductName}");
    }
    if (int.TryParse(Console.ReadLine(), out int ProductId))
    {
        Product product = db.Products.FirstOrDefault(p => p.ProductId == ProductId);
        if (product != null)
        {
            return product;
        }
    }
    logger.Error("Invalid Product Id");
    return null;
}

static Product EditProduct(NWConsole_23_JPSContext db, Logger logger)
{
    Product product = new Product();
    Console.WriteLine("What would you like to edit?");
    Console.WriteLine("1. Product Name");
    Console.WriteLine("2. Supplier ID");
    Console.WriteLine("3. Category ID");
    Console.WriteLine("4. Quantity per Unit");
    Console.WriteLine("5. Price per Unit");
    Console.WriteLine("6. Units in Stock");
    Console.WriteLine("7. Units on Order");
    Console.WriteLine("8. ReOrder Level");
    Console.WriteLine("9. Discontinued Status");
    string answer = Console.ReadLine();

    if(answer == "1")
    {
        Console.WriteLine("What is the new Product Name?");
        product.ProductName = Console.ReadLine();
    }

    else if (answer == "2")
    {
        Console.WriteLine("What is the new Supplier ID?");
        product.SupplierId = Convert.ToInt32(Console.ReadLine());
    }
    else if (answer == "3")
    {
        Console.WriteLine("What is the new Category ID?");
        product.CategoryId = Convert.ToInt32(Console.ReadLine());
    }
    else if (answer == "4")
    {
        Console.WriteLine("What is the new Quantity per Unit?");
        product.QuantityPerUnit = Console.ReadLine();
    }
    else if (answer == "5")
    {
        Console.WriteLine("What is the new Unit Price");
        product.UnitPrice = Convert.ToDecimal(Console.ReadLine());
    }
    else if (answer == "6")
    {
        Console.WriteLine("How many Units in Stock?");
        product.UnitsInStock = Convert.ToInt16(Console.ReadLine());
    }
    else if (answer == "7")
    {
        Console.WriteLine("How many Units on Order?");
        product.UnitsOnOrder = Convert.ToInt16(Console.ReadLine());
    }
    else if (answer == "8")
    {
        Console.WriteLine("What is the new ReOrder Level?");
        product.ReorderLevel = Convert.ToInt16(Console.ReadLine());
    }
    else if (answer == "9")
    {
        Console.WriteLine("Is the Product discontinued? (Y/N)");
        string discon = Console.ReadLine();
    if(discon == "y")
    {
        product.Discontinued = true;
    }
    }
    

    ValidationContext context = new ValidationContext(product, null, null);
    List<ValidationResult> results = new List<ValidationResult>();

    var isValid = Validator.TryValidateObject(product, context, results, true);
    if (isValid)
    {
            
        if (db.Products.Any(p => p.ProductName == product.ProductName)) {
          
             results.Add(new ValidationResult("Product name exists", new string[] { "Name" }));
        } else {
            return product;
        }

         foreach (var result in results)
         {
            logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
         }
       
    }
    
    return null;
}


