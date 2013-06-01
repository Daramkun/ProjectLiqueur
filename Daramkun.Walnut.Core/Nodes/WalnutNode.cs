using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Contents;
using Daramkun.Liqueur.Datas.Json;
using Daramkun.Liqueur.Graphics;
using Daramkun.Liqueur.Graphics.Fonts;
using Daramkun.Liqueur.Nodes;
using Daramkun.Walnut.Scripts;

namespace Daramkun.Walnut.Nodes
{
	public class WalnutNode : Node
	{
		EventCallee eventCallee;

		public WalnutNode () { eventCallee = null; }
		public WalnutNode ( JsonEntry jsonEntry, ContentManager contentManager )
		{
			if ( jsonEntry.Contains ( "script" ) )
			{
				try
				{
					JsonEntry script = jsonEntry [ "script" ].Data as JsonEntry;
					if ( script [ "language" ].Data as string == WalnutSystem.ScriptEngine.ScriptLanguage )
					{
						string scriptText = contentManager.Load<string> ( script [ "file" ].Data as string );
						Type eventCalleeType = WalnutSystem.ScriptEngine.Run ( scriptText ) as Type;
						eventCallee = Activator.CreateInstance ( eventCalleeType ) as EventCallee;
					}
				}
				catch { }
			}

			if ( jsonEntry.Contains ( "objects" ) )
			{
				JsonArray objs = jsonEntry [ "objects" ].Data as JsonArray;
				foreach ( JsonItem obj in objs )
				{
					JsonEntry objEntry = obj.Data as JsonEntry;
					if ( objEntry [ "objtype" ].Data as string == "sprite" )
					{
						AddChild ( new Sprite ( objEntry, contentManager ) );
					}
					else if ( objEntry [ "objtype" ].Data as string == "label" )
					{
						if ( objEntry [ "fonttype" ].Data as string == "lsf" )
							AddChild ( new Label<LsfFont> ( objEntry, contentManager ) );
						else if ( objEntry [ "fonttype" ].Data as string == "ziplsf" )
							AddChild ( new Label<ZipLsfFont> ( objEntry, contentManager ) );
						else
							AddChild ( new Label<IFont> ( objEntry, contentManager ) );
					}
				}
			}
		}

		public override void OnInitialize ()
		{
			if ( eventCallee != null )
				eventCallee.onInitialize ();
			base.OnInitialize ();
		}

		public override void OnFinalize ()
		{
			if ( eventCallee != null )
				eventCallee.onFinalize ();
			base.OnFinalize ();
		}

		public override void OnUpdate ( Liqueur.Common.GameTime gameTime )
		{
			if ( eventCallee != null )
				eventCallee.onUpdate ( this, gameTime.ElapsedGameTime.Milliseconds / 1000.0f );
			base.OnUpdate ( gameTime );
		}

		public override void OnDraw ( Liqueur.Common.GameTime gameTime )
		{
			if ( eventCallee != null )
				eventCallee.onDraw ( this, gameTime.ElapsedGameTime.Milliseconds / 1000.0f );
			base.OnDraw ( gameTime );
		}
	}
}
