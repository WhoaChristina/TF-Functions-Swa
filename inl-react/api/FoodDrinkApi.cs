using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Cosmos.Table;
using System.Linq;

namespace Functions
{
    public static class FoodDrinkApi
    {
        [FunctionName("FoodDrinkApi")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = null)] HttpRequest req,
                        [Table("DrinkFood", "1", Connection = "FoodDrinkDatabaseConnection")] CloudTable table,
            ILogger log)
        {
            log.LogInformation("C# HTTP trigger function processed a request.");

            var query = table.CreateQuery<TableData>();
            query.TakeCount = 1;

            var foodDrinks = (await table.ExecuteQuerySegmentedAsync(query, null)).ToList();
            log.LogInformation(foodDrinks.FirstOrDefault().ToString());

            if (foodDrinks.Any())
            {
                
                var result = new ApiResponse(
                    foodDrinks.First().DrinkName,
                    foodDrinks.First().DrinkImg,
                    foodDrinks.First().FoodName,
                    foodDrinks.First().FoodImg
                );

                return new OkObjectResult(result);
            }

            return new OkResult();
        }
    }

    public record ApiResponse(string DrinkName, string DrinkImg, string FoodName, string FoodImg);

    public class TableData : TableEntity
    {
        public string DrinkName { get; set; }
        public string DrinkImg { get; set; }
        public string FoodName { get; set; }
        public string FoodImg { get; set; }
    }
}
