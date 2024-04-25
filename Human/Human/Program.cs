using System;

namespace Human
{
    class Program
    {
        static void Main(string[] args)
        {
            YourName Jelyn = new YourName("Jelyn", "Jin", "5'04", 108, 24);
            YourName Mimi = new YourName("Mimi", "Cat", "2'62", 40, 3);

            Jelyn.introduction();
            Mimi.introduction();

            YourName Dog = new YourName();
            Dog.introduction();

            Console.ReadLine();
        }
    }
}