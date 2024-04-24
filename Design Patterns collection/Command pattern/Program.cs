namespace Command_pattern
{
	public class BankAccount
	{
		private int balance;
		private int overdraftLimit = -500;

		public void Deposit(int amount)
		{
			balance += amount;
			Console.WriteLine($"Deposited ${amount}, balance is now {balance}");
		}

		public bool Withdraw(int amount)
		{ 
			if ( balance - amount >= overdraftLimit )
			{
				balance -= amount;
				Console.WriteLine($"Withdrew ${amount}, balance is now {balance}");
				return true;
			}
			return false;
		}

		public override string? ToString()
		{
			return $"Account balance: {balance}";
		}
	}

	public interface ICommand
	{
		void Call();
		void Undo();
		bool Success { get; set; }
	}

	public class BankAccountCommand : ICommand
	{
		private BankAccount acccount;
		public enum Action
		{
			Deposit, 
			Withdraw,
		}

		private Action action;
		private int amount;

		public BankAccountCommand(BankAccount acccount, Action action, int amount)
		{
			this.acccount = acccount ?? throw new ArgumentNullException(nameof(acccount));
			this.action = action;
			this.amount = amount;
		}

		public void Call()
		{
			switch (action)
			{
				case Action.Deposit:
					acccount.Deposit(amount);
					Success = true;
					break;
				case Action.Withdraw:
					Success = acccount.Withdraw(amount);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public void Undo()
		{
			if (!Success) 
			{
				return;
			}

			switch (action)
			{
				case Action.Deposit:
					acccount.Withdraw(amount);
					break;
				case Action.Withdraw:
					acccount.Deposit(amount);
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}

		public bool Success { get; set; }
	}

	public class CompositeBankAccountCommand : List<BankAccountCommand>, ICommand
	{
		public CompositeBankAccountCommand()
		{
		}

		public CompositeBankAccountCommand(IEnumerable<BankAccountCommand> collection) : base(collection)
		{
		}

		public bool Success 
		{ 
			get
			{
				return this.All(cmd => cmd.Success);
			} 
			set
			{
				foreach(var  cmd in this)
				{
					cmd.Success = value;
				}
			}
		}

		public virtual void Call()
		{
			ForEach(cmd => cmd.Call());
		}

		public virtual void Undo()
		{
			foreach (var cmd in ((IEnumerable<BankAccountCommand>)this).Reverse())
			{
				if (cmd.Success)
				{
					cmd.Undo();
				}
			}
		}
	}

	public class MoneyTransferCommand : CompositeBankAccountCommand
	{
		public MoneyTransferCommand(BankAccount from, BankAccount to, int amount)
		{
			AddRange(new[]
			{
				new BankAccountCommand(from, BankAccountCommand.Action.Withdraw, amount),
				new BankAccountCommand(to, BankAccountCommand.Action.Deposit, amount),
			});
		}

		public override void Call()
		{
			BankAccountCommand last = null;
			foreach (var cmd in this)
			{
				if (last == null || last.Success)
				{
					cmd.Call();
					last = cmd;
				}
				else
				{
					cmd.Undo();
					break;	
				}
			}
		}
	}


	internal class Program
	{
		static void Main(string[] args)
		{

			//Normal command structure
			//var bankAccount = new BankAccount();
			//var commands = new List<BankAccountCommand>
			//{
			//	new BankAccountCommand(bankAccount, BankAccountCommand.Action.Deposit, 100),
			//	new BankAccountCommand(bankAccount, BankAccountCommand.Action.Withdraw, 1000),
			//};

			//Console.WriteLine(bankAccount);

			//foreach (var command in commands)
			//{
			//	command.Call();
			//}

			//foreach (var command in Enumerable.Reverse(commands))
			//{
			//	command.Undo();
			//}


			// Composite command structure
			//var bankAccount2 = new BankAccount();
			//var deposit = new BankAccountCommand(bankAccount2, BankAccountCommand.Action.Deposit, 100);
			//var withdraw = new BankAccountCommand(bankAccount2, BankAccountCommand.Action.Withdraw, 50);

			//var composite = new CompositeBankAccountCommand(new[] { deposit, withdraw });

			//composite.Call();
			//Console.WriteLine(bankAccount2);

			//composite.Undo();
			//Console.WriteLine(bankAccount2);

			//Composite command structure with virtuals commands in a command class
			var from = new BankAccount();
			from.Deposit(100);
			var to = new BankAccount();

			Console.WriteLine(from);
			Console.WriteLine(to);

			var mtc = new MoneyTransferCommand(from, to, 1000);
			mtc.Call();

			Console.WriteLine(from);
			Console.WriteLine(to);

			mtc.Undo();

			Console.WriteLine(from);
			Console.WriteLine(to);

			Console.ReadKey();
		}
	}
}
