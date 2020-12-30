﻿using System;
using System.Collections.Generic;

namespace Xsolla.Login
{
	[Serializable]
	public class UserPhoneNumber
	{
		/// <summary>
		/// User phone number.
		/// </summary>
		/// <see cref="https://developers.xsolla.com/user-account-api/user-phone-number/getusersmephone"/>
		/// <see cref="https://en.wikipedia.org/wiki/National_conventions_for_writing_telephone_numbers"/>
		public string phone_number;
	}
}