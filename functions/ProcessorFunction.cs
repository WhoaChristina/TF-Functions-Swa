using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.WebJobs.Extensions.DurableTask;
using System.Linq;
using System.Dynamic;
using Microsoft.Azure.WebJobs.Extensions.Storage;

namespace Teknikfördjupning
{
    public static class ProcessorFunction
    {
        

        [FunctionName("ProcessorFunction")]
        public static async Task<string> Run([ActivityTrigger] (string cocktail, string meal) input,
            [Table("DrinkFood"), StorageAccount("AzureWebJobsStorage")] ICollector<ProcessorFunction.TableData> msg)
            //StorageAccount skrivas om till connectionstring till azure storage account. den ligger i local.settings.json
            //Table vill skrivas om också, det ska vara tabellen som vi vill skriva till
        {
            string res = "";
            try
            {
                var jsonCocktail = JsonConvert.DeserializeObject<dynamic>(input.cocktail);
                var jsonMeal = JsonConvert.DeserializeObject<dynamic>(input.meal);

                string drink = jsonCocktail.drinks[0]["strDrink"];
                string drinkThumb = jsonCocktail.drinks[0]["strDrinkThumb"];

                string meal = jsonMeal.meals[0]["strMeal"];
                string mealThumb = jsonMeal.meals[0]["strMealThumb"];

                //string data = drinkName + "|" + drinkImg + "|" + mealName + "|" + mealImg;
                //string json = JsonConvert.SerializeObject(data);

                msg.Add(new ProcessorFunction.TableData("1", $"{(DateTimeOffset.MaxValue.Ticks-DateTime.Now.Ticks):d10}-{Guid.NewGuid():N}", drink, drinkThumb, meal, mealThumb));

                
                

                res = "done!";
            }
            catch (Exception x)
            {
                res = "Oops! something went wrong! " + x.Message;
            }
            return res;
        }
        public record TableData(string PartitionKey, string RowKey, string DrinkName, string DrinkImg, string FoodName, string FoodImg);
    }
}
