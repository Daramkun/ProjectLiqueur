using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using csogg;
using csvorbis;

namespace Daramkun.Liqueur.Decoders.Sounds.OggVorbisCore
{
	internal class OggDecodeStream : Stream
	{
		private Stream decodedStream;
		private const int HEADER_SIZE = 36;

		public OggDecodeStream ( Stream input, bool skipWavHeader )
		{
			if ( input == null )
				throw new ArgumentNullException ();
			decodedStream = DecodeStream ( input, skipWavHeader );
		}

		public OggDecodeStream ( Stream input )
			: this ( input, false )
		{
		}

		Stream DecodeStream ( Stream input, bool skipWavHeader )
		{
			int convsize = 4096 * 2;
			byte [] convbuffer = new byte [ convsize ];

			Stream output = new MemoryStream ();

			if ( !skipWavHeader )
				output.Seek ( HEADER_SIZE, SeekOrigin.Begin );

			SyncState oy = new SyncState ();
			StreamState os = new StreamState ();
			Page og = new Page ();
			Packet op = new Packet ();

			Info vi = new Info ();
			Comment vc = new Comment ();
			DspState vd = new DspState ();
			Block vb = new Block ( vd );

			byte [] buffer;
			int bytes = 0;

			oy.init ();

			while ( true )
			{
				int eos = 0;

				int index = oy.buffer ( 4096 );
				buffer = oy.data;
				bytes = input.Read ( buffer, index, 4096 );
				oy.wrote ( bytes );

				if ( oy.pageout ( og ) != 1 )
				{
					if ( bytes < 4096 ) break;
					throw new Exception ( "Input does not appear to be an Ogg bitstream." );
				}

				os.init ( og.serialno () );

				vi.init ();
				vc.init ();
				if ( os.pagein ( og ) < 0 )
				{
					throw new Exception ( "Error reading first page of Ogg bitstream data." );
				}

				if ( os.packetout ( op ) != 1 )
				{
					throw new Exception ( "Error reading initial header packet." );
				}

				if ( vi.synthesis_headerin ( vc, op ) < 0 )
				{
					throw new Exception ( "This Ogg bitstream does not contain Vorbis audio data." );
				}

				int i = 0;

				while ( i < 2 )
				{
					while ( i < 2 )
					{

						int result = oy.pageout ( og );
						if ( result == 0 ) break;

						if ( result == 1 )
						{
							os.pagein ( og );
							while ( i < 2 )
							{
								result = os.packetout ( op );
								if ( result == 0 ) break;
								if ( result == -1 )
								{
									throw new Exception ( "Corrupt secondary header.  Exiting." );
								}
								vi.synthesis_headerin ( vc, op );
								i++;
							}
						}
					}
					index = oy.buffer ( 4096 );
					buffer = oy.data;
					bytes = input.Read ( buffer, index, 4096 );
					if ( bytes == 0 && i < 2 )
					{
						throw new Exception ( "End of file before finding all Vorbis headers!" );
					}
					oy.wrote ( bytes );
				}

				{
					byte [] [] ptr = vc.user_comments;
					for ( int j = 0; j < vc.user_comments.Length; j++ )
					{
						if ( ptr [ j ] == null ) break;
					}
				}

				convsize = 4096 / vi.channels;

				vd.synthesis_init ( vi );
				vb.init ( vd );

				float [] [] [] _pcm = new float [ 1 ] [][];
				int [] _index = new int [ vi.channels ];
				while ( eos == 0 )
				{
					while ( eos == 0 )
					{
						int result = oy.pageout ( og );
						if ( result == 0 ) break;
						if ( result == -1 )
							throw new Exception ( "Corrupt or missing data in bitstream; continuing..." );
						else
						{
							os.pagein ( og );
							while ( true )
							{
								result = os.packetout ( op );

								if ( result == 0 ) break;
								if ( result == -1 )
								{

								}
								else
								{
									int samples;
									if ( vb.synthesis ( op ) == 0 )
									{
										vd.synthesis_blockin ( vb );
									}

									while ( ( samples = vd.synthesis_pcmout ( _pcm, _index ) ) > 0 )
									{
										float [] [] pcm = _pcm [ 0 ];
										bool clipflag = false;
										int bout = ( samples < convsize ? samples : convsize );

										for ( i = 0; i < vi.channels; i++ )
										{
											int ptr = i * 2;
											int mono = _index [ i ];
											for ( int j = 0; j < bout; j++ )
											{
												int val = ( int ) ( pcm [ i ] [ mono + j ] * 32767.0 );
												if ( val > 32767 )
												{
													val = 32767;
													clipflag = true;
												}
												if ( val < -32768 )
												{
													val = -32768;
													clipflag = true;
												}
												if ( val < 0 ) val = val | 0x8000;
												convbuffer [ ptr ] = ( byte ) ( val );
												convbuffer [ ptr + 1 ] = ( byte ) ( ( uint ) val >> 8 );
												ptr += 2 * ( vi.channels );
											}
										}

										if ( clipflag )
										{

										}

										output.Write ( convbuffer, 0, 2 * vi.channels * bout );

										vd.synthesis_read ( bout );
									}
								}
							}
							if ( og.eos () != 0 ) eos = 1;
						}
					}
					if ( eos == 0 )
					{
						index = oy.buffer ( 4096 );
						buffer = oy.data;
						bytes = input.Read ( buffer, index, 4096 );
						oy.wrote ( bytes );
						if ( bytes == 0 ) eos = 1;
					}
				}

				os.clear ();

				vb.clear ();
				vd.clear ();
				vi.clear ();
			}

			oy.clear ();

			output.Seek ( 0, SeekOrigin.Begin );
			if ( !skipWavHeader )
			{
				WriteHeader ( output, ( int ) ( output.Length - HEADER_SIZE ), vi.rate, ( ushort ) 16, ( ushort ) vi.channels );
				output.Seek ( 0, SeekOrigin.Begin );
			}

			return output;
		}

		void WriteHeader ( Stream stream, int length, int audioSampleRate, ushort audioBitsPerSample, ushort audioChannels )
		{
			BinaryWriter bw = new BinaryWriter ( stream );

			bw.Write ( new char [ 4 ] { 'R', 'I', 'F', 'F' } );
			int fileSize = HEADER_SIZE + length;
			bw.Write ( fileSize );
			bw.Write ( new char [ 8 ] { 'W', 'A', 'V', 'E', 'f', 'm', 't', ' ' } );
			bw.Write ( ( int ) 16 );
			bw.Write ( ( short ) 1 );
			bw.Write ( ( short ) audioChannels );
			bw.Write ( audioSampleRate );
			bw.Write ( ( int ) ( audioSampleRate * ( ( audioBitsPerSample * audioChannels ) / 8 ) ) );
			bw.Write ( ( short ) ( ( audioBitsPerSample * audioChannels ) / 8 ) );
			bw.Write ( ( short ) audioBitsPerSample );

			bw.Write ( new char [ 4 ] { 'd', 'a', 't', 'a' } );
			bw.Write ( length );
		}

		public override bool CanRead
		{
			get { return true; }
		}

		public override bool CanSeek
		{
			get { return true; }
		}

		public override bool CanWrite
		{
			get { return false; }
		}

		public override void Flush ()
		{
			throw new NotImplementedException ();
		}

		public override long Length
		{
			get { return decodedStream.Length; }
		}

		public override long Position
		{
			get
			{
				return decodedStream.Position;
			}
			set
			{
				decodedStream.Position = value;
			}
		}

		public override int Read ( byte [] buffer, int offset, int count )
		{
			return decodedStream.Read ( buffer, offset, count );
		}

		public override long Seek ( long offset, SeekOrigin origin )
		{
			return Seek ( offset, origin );
		}

		public override void SetLength ( long value )
		{
			throw new NotImplementedException ();
		}

		public override void Write ( byte [] buffer, int offset, int count )
		{
			throw new NotImplementedException ();
		}
	}
}