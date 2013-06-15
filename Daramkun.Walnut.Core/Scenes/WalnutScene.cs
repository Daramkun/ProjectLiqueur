using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Daramkun.Liqueur.Common;
using Daramkun.Liqueur.Contents;
using Daramkun.Liqueur.Datas.Json;
using Daramkun.Liqueur.Graphics;
using Daramkun.Liqueur.Graphics.Fonts;
using Daramkun.Liqueur.Scenes;
using Daramkun.Walnut.Contents;
using Daramkun.Walnut.Nodes;
using Daramkun.Walnut.Scripts;

namespace Daramkun.Walnut.Scenes
{
	public class WalnutScene : Scene
	{
		EventCallee eventCallee;

		JsonEntry jsonEntry;
		ContentManager contentManager;

		public WalnutScene ( Stream sceneStream, ContentManager contentManager )
		{
			jsonEntry = JsonParser.Parse ( sceneStream );
			this.contentManager = contentManager;
		}

		public WalnutScene ( JsonEntry jsonEntry, ContentManager contentManager )
		{
			this.jsonEntry = jsonEntry;
			this.contentManager = contentManager;
		}

		public override void OnInitialize ()
		{
			AnalyzeJsonData ( jsonEntry, contentManager );
			if ( eventCallee != null )
				eventCallee.onInitialize ();
			base.OnInitialize ();
		}

		private void AnalyzeJsonData ( JsonEntry jsonEntry, ContentManager contentManager )
		{
			JsonEntry sceneInfo = jsonEntry [ "info" ].Data as JsonEntry;
			SceneName = sceneInfo [ "scenename" ].Data as string;

			if ( jsonEntry.Contains ( "script" ) )
			{
				try
				{
					JsonEntry script = jsonEntry [ "script" ].Data as JsonEntry;
					if ( script [ "language" ].Data as string == WalnutSystem.ScriptEngine.ScriptLanguage )
					{
						string scriptText;
						scriptText = contentManager.Load<string> ( script [ "file" ].Data as string );
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
						AddChild ( new Label ( objEntry, contentManager ) );
					}
				}
			}
		}

		public override void OnFinalize ()
		{
			if ( eventCallee != null )
				eventCallee.onFinalize ();
			base.OnFinalize ();
		}

		public override void OnUpdate ( GameTime gameTime )
		{
			if ( eventCallee != null )
				eventCallee.onUpdate ( this, gameTime.ElapsedGameTime.Milliseconds / 1000.0f );
			base.OnUpdate ( gameTime );
		}

		public override void OnDraw ( GameTime gameTime )
		{
			if ( eventCallee != null )
				eventCallee.onDraw ( this, gameTime.ElapsedGameTime.Milliseconds / 1000.0f );
			base.OnDraw ( gameTime );
		}
	}
}
