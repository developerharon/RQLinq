using RQLinq;

var orders = new List<Order>
        {
            new Order {
                Id = 1,
                Products = new List<Product>
                {
                    new Product { Id = 1, Name = "Apple", Price = 0.50m },
                    new Product { Id = 2, Name = "Banana", Price = 0.25m },
                    new Product { Id = 3, Name = "Orange", Price = 0.75m }
                },
                Tags = new Dictionary<string, string[]> { { "location", new string[] { "Nakuru" } } },
                DescriptionTags = new List<string> { "fruits", "fresh" } },
            new Order {
                Id = 2,
                Products = new List<Product>
                {
                    new Product { Id = 1, Name = "Books", Price = 0.50m },
                    new Product { Id = 2, Name = "Pencils", Price = 0.25m },
                    new Product { Id = 3, Name = "Erasers", Price = 0.75m }
                },
                Tags = new Dictionary<string, string[]> { { "location", new string[] { "Nairobi" } } },
                DescriptionTags = new List<string> { "school", "things" } },
            new Order {
                Id = 3,
                Products = new List<Product>
                {
                    new Product { Id = 1, Name = "Beef", Price = 0.50m },
                    new Product { Id = 2, Name = "Chicken", Price = 0.25m },
                    new Product { Id = 3, Name = "Pork", Price = 0.75m }
                },
                Tags = new Dictionary<string, string[]> { { "location", new string[] { "Mombasa" } } },
                DescriptionTags = new List<string> { "meat", "animal" } },
            new Order {
                Id = 4,
                Products = new List<Product>
                {
                    new Product { Id = 1, Name = "Detol", Price = 0.50m },
                    new Product { Id = 2, Name = "Geisha", Price = 0.25m },
                    new Product { Id = 3, Name = "Menengai", Price = 0.75m }
                },
                Tags = new Dictionary<string, string[]> { { "location", new string[] { "Kisumu" } } },
                DescriptionTags = new List<string> { "bathing", "soap", "school" } },
        };

bool showTree = true;

while (true)
{
    Console.Write("> ");
    var line = Console.ReadLine();

    if (string.IsNullOrEmpty(line))
        return;

    if (line == "showtree")
    {
        showTree = !showTree;
        Console.WriteLine(showTree ? "Showing parse trees" : "Not showing parse trees");
        continue;
    }

    if (line == "cls")
    {
        Console.Clear();
        continue;
    }

    if (line == "exit")
        Environment.Exit(0);

    var syntaxTree = RqlSyntaxTree.Parse(line);
    var color = Console.ForegroundColor;

    if (showTree)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        PrettyPrint(syntaxTree.Root);
        Console.ForegroundColor = color;
    }

    if (!syntaxTree.Diagnostics.Any())
    {
        var evaluator = new RqlEvaluator(syntaxTree.Root);
        var filterExpression = evaluator.Evaluate<Order>();

        try
        {
            var filteredOrders = orders
                .AsQueryable()
                .Where(filterExpression)
                .ToList();

            IDictionary<string, string[]> dict = new Dictionary<string, string[]>();


            foreach (var order in filteredOrders)
            {
                Console.WriteLine($"Order Id: {order.Id}, Name: {order.Products.FirstOrDefault()?.Name}, Price: {order.Products.FirstOrDefault()?.Price}");
            }
        }
        catch (NotImplementedException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }
    else
    {
        Console.ForegroundColor = ConsoleColor.DarkRed;

        foreach (var diagnostic in syntaxTree.Diagnostics)
            Console.WriteLine(diagnostic);

        Console.ForegroundColor = color;
    }
}

static void PrettyPrint(RqlNode node, string indent = "", bool isLast = true)
{
    // └──
    // ├──
    // │  

    var marker = isLast ? "└──" : "├──";
    Console.Write(indent);
    Console.Write(marker);
    Console.Write(node.Kind);

    if (node is RqlToken t && t.Value != null)
    {
        Console.Write(" ");
        Console.Write(t.Value);
    }
    Console.WriteLine();

    indent += isLast ? "    " : "|   ";

    var lastChild = node.GetChilden().LastOrDefault();

    foreach (var child in node.GetChilden())
        PrettyPrint(child, indent, child == lastChild);
}
