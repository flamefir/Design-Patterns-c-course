﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Command_pattern
{
	public class Command
	{
		public enum Action
		{
			Deposit,
			Withdraw
		}

		public Action TheAction;
		public int Amount;
		public bool Success;
	}

	public class Account
	{
		public int Balance { get; set; }

		public void Process(Command c)
		{
			switch(c.TheAction)
			{ 
				case Command.Action.Deposit:
					Balance += c.Amount;
					c.Success = true;
					break;
				case Command.Action.Withdraw:
					c.Success = false;
					if (Balance - c.Amount > 0)
					{
						Balance -= c.Amount;
						c.Success = true;
					}
					break;
				default:
					throw new ArgumentOutOfRangeException();
			}
		}
	}
}
