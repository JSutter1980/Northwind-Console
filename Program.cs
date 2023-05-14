using NLog;
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
        Console.WriteLine("3. Edit Category");
        Console.WriteLine("4. Display Products");
        Console.WriteLine("5. Add Product");
        Console.WriteLine("6. Edit Product");
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
                 db.AddCategory(category);
                 logger.Info("Category added - {name}", category.CategoryName);
             }

        }

        else if (choice == "3")

        {

            Console.WriteLine("Choose the Category to edit:");
            var category = GetCategory(db, logger);
            if (category != null)
            {
                
                Category UpdatedCategroy = EditCategory(db, logger);
                if (UpdatedCategroy != null)
                {
                    UpdatedCategroy.CategoryId = category.CategoryId;
                    db.EditCategory(UpdatedCategroy);
                    logger.Info($"Category (id: {category.CategoryId}) updated");
                }
            }

        }

        else if (choice == "4")
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
        else if (choice == "5")
        {

            Product product = AddProduct(db, logger);
            if (product != null)
            {
                db.AddProduct(product);
                logger.Info("Product added - {name}", product.ProductName);
            }

        }
        else if (choice == "6")
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

static Category GetCategory(NWConsole_23_JPSContext db, Logger logger)
{
    
    var categroies = db.Categories.OrderBy(c => c.CategoryId);
    foreach (Category c in categroies)
    {
        Console.WriteLine($"{c.CategoryId}: {c.CategoryName}");
    }
    if (int.TryParse(Console.ReadLine(), out int ProductId))
    {
        Category category = db.Categories.FirstOrDefault(c => c.CategoryId == c.CategoryId);
        if (category != null)
        {
            return category;
        }
    }
    logger.Error("Invalid Category Id");
    return null;
}

static Category EditCategory(NWConsole_23_JPSContext db, Logger logger)
{

    Category category = new Category();

    Console.WriteLine("What is the new Category Name?");
    category.CategoryName = Console.ReadLine();
    Console.WriteLine("What is the new Description?");
    category.Description = Console.ReadLine();
    ValidationContext context = new ValidationContext(category, null, null);
    List<ValidationResult> results = new List<ValidationResult>();

    var isValid = Validator.TryValidateObject(category, context, results, true);
    if (isValid)
    {
        if (db.Categories.Any(c => c.CategoryName == category.CategoryName)) {
            results.Add(new ValidationResult("Category name exists", new string[] { "Name" }));
        } else {
            return category;
        }

        foreach (var result in results)
        {
            logger.Error($"{result.MemberNames.First()} : {result.ErrorMessage}");
        }
    
    }
    return null;
}

static Product EditProduct(NWConsole_23_JPSContext db, Logger logger)
{
    Product product = new Product();

    Console.WriteLine("What is the new Product Name?");
    product.ProductName = Console.ReadLine();
    Console.WriteLine("What is the new Unit Price?");
    product.UnitPrice = Convert.ToDecimal(Console.ReadLine());
    Console.WriteLine("How many Units are In Stock?");
    product.UnitsInStock = Convert.ToInt16(Console.ReadLine());
    
    ValidationContext context = new ValidationContext(product, null, null);
    List<ValidationResult> results = new List<ValidationResult>();

    var isValid = Validator.TryValidateObject(product, context, results, true);
    if (isValid)
    {
        if (db.Products.Any(p => p.ProductName == product.ProductName)) {
            results.Add(new ValidationResult("Category name exists", new string[] { "Name" }));
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


