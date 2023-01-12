using System;
using System.Collections.Generic;
using System.Text;

namespace Shared.DTO
{
    public class OrderStatusDTO
    {
        public string Status { get; set; } = WAITING;


        public static string WAITING = "WAITING";
        public static string COMPLETED = "COMPLETED";
    }
}
