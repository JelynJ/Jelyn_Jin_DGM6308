using System;
using System.Collections.Generic;
using System.Text;

namespace Human
{
	public class YourName
	{
		public string firstname = "";
		public string lastname = "";
		public string height = "";
		public int weight = 0;
		public int age = 0;

		public YourName()
		{
			Console.WriteLine("Your Name was Called");
		}
		public YourName(string firstname, string lastname, string Height, int Weight, int Age)
		{
			this.firstname = firstname;
			this.lastname = lastname;
			this.height = Height;
			this.weight = Weight;
			this.age = Age;
		}

		public void introduction()
		{
			if (age >= 18)
				Console.WriteLine("Hello my name is {0} {1}, I am {2} and weigh {3}. Also I am {4} years old.", firstname, lastname, height, weight, age);
			else
				Console.WriteLine("Sorry I can't introduce myself, I am underage.");

		}
	}
}

