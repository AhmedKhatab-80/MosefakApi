namespace MosefakApp.Infrastructure.Seed
{
    public static class SeedData
    {
        public static void Seed(AppDbContext dbContext)
        {
            if (dbContext is not null)
            {
            }

        }
    }
}

/*
 
  -- Bad Practice

                    if(Products is not null)
                    {
                        foreach (var prod in Products)
                        {
                            await dbContext.Products.AddAsync(prod);
                            await dbContext.SaveChangesAsync();
                        }

                    }
 
  -- Good Practice as Performace

                    if(Products is not null)
                    {
                        foreach (var prod in Products)
                        {
                            await dbContext.Products.AddAsync(prod);
                        }
                            await dbContext.SaveChangesAsync();
                    }

 */