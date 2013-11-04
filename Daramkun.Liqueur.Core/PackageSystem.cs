using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Contents;
using Daramkun.Liqueur.Contents.Decoder.Packages;

namespace Daramkun.Liqueur
{
	public static class PackageSystem
	{
		public static PackageInfo MainPackage { get; private set; }

		static List<PackageInfo> subPacks = new List<PackageInfo> ();
		public static PackageInfo [] SubPackages { get { return subPacks.ToArray (); } }

		public static Guid LoadSubPackage ( Stream stream )
		{
			PackageInfo packInfo;
			try
			{
				packInfo = new PackageDecoder ().Decode ( stream );
				subPacks.Add ( packInfo );
			}
			catch { return Guid.Empty; }
			return packInfo.PackageID;
		}

		public static void UnloadSubPackage ( Guid guid )
		{
			foreach ( PackageInfo p in from p in subPacks where p.PackageID == guid select p )
				subPacks.Remove ( p );
		}
	}
}
