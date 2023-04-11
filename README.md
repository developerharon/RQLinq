<div align="center">

[![Build](https://github.com/developerharon/RQLinq/actions/workflows/build.yml/badge.svg)](https://github.com/developerharon/RQLinq/actions/workflows/build.yml)
[![Deploy](https://github.com/developerharon/RQLinq/actions/workflows/deploy.yml/badge.svg)](https://github.com/developerharon/RQLinq/actions/workflows/deploy.yml) 

</div>

# RQLinq

C# library that converts RQL (Resource Query Language) to a Func expression that can run on a LINQ query.

## Installation

```

dotnet add package RQLinq --version 0.0.6

```

## Usage

Assuming you have a list of products, you can filter by ID using an RQL statement:

```csharp

string rql = "eq(id,4)";

var filterExpression = RqlEvaluator.Evaluate<Product>(rql);

var filteredProducts = products
                .AsQueryable()
                .Where(filterExpression)
                .ToList();

foreach (var product in filteredProducts)
{
    Console.WriteLine($"Product Id: {product.Id}, Name: {product.Name}, Price: {product.Price}");
}

```