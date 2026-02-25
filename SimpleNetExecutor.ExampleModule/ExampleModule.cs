using System;

namespace SimpleNetExecutor.ExampleModule
{
    public class ExampleModule
    {
        public static void Main(Action<string> output)
        {
            output("Hello from ExampleModule!");
        }
    }
}