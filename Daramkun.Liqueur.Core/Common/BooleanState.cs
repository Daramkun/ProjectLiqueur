using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Daramkun.Liqueur.Common
{
	/// <summary>
	/// 참/거짓 상태
	/// </summary>
	public enum BooleanState
	{
		/// <summary>
		/// 잘 모르는 상태
		/// </summary>
		Unknown = -1,
		/// <summary>
		/// 거짓 상태
		/// </summary>
		False = 0,
		/// <summary>
		/// 참 상태
		/// </summary>
		True = 1,
	}
}
