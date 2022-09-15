using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServerTests
{
    /*
     * Nick Chapsas - Youtube
     * https://www.youtube.com/watch?v=VuFQtyRmS0E
     * 
     * Rozszerza IResult poprzez dodanie metod odsłanianijących wewnętrzne pola  
     * 
     * Extends IResult by adding methods that expose internal fields
     */
    public static class IResultExtensions
    {
        public static T? GetOkObjectResultValue<T>(this IResult result)
        {
            return (T?)Type.GetType("Microsoft.AspNetCore.Http.Result.OkObjectResult, Microsoft.AspNetCore.Http.Results")?
                .GetProperty("Value",
                        System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public)?
                        .GetValue(result);
        }
        public static int? GetOkObjectResultStatusCode(this IResult result)
        {
            return (int?)Type.GetType("Microsoft.AspNetCore.Http.Result.OkObjectResult, Microsoft.AspNetCore.Http.Results")?
                .GetProperty("StatusCode",
                        System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public)?
                        .GetValue(result);
        }
        public static int? GetNotFoundResultStatusCode(this IResult result)
        {
            return (int?)Type.GetType("Microsoft.AspNetCore.Http.Result.OkObjectResult, Microsoft.AspNetCore.Http.Results")?
                .GetProperty("StatusCode",
                        System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public)?
                        .GetValue(result);
        }
    }
}
