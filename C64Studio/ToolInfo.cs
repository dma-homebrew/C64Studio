﻿using System;
using System.Collections.Generic;
using System.Text;
using C64Studio.Types.ASM;

namespace C64Studio
{
  public class ToolInfo
  {
    public enum ToolType
    {
      UNKNOWN,
      ASSEMBLER,
      EMULATOR
    };

    public ToolType         Type = ToolType.UNKNOWN;
    public string           Name = "";
    public string           Filename = "";
    public string           WorkPath = "";
    public string           PRGArguments = "";
    public string           CartArguments = "";
    public string           DebugArguments = "";
    public string           TrueDriveOnArguments = "";
    public string           TrueDriveOffArguments = "";
    public bool             PassLabelsToEmulator = true;

    public GR.IO.FileChunk ToChunk()
    {
      GR.IO.FileChunk chunk = new GR.IO.FileChunk( Types.FileChunk.SETTINGS_TOOL );

      chunk.AppendU32( (uint)Type );
      chunk.AppendString( Name );
      chunk.AppendString( Filename );
      chunk.AppendString( PRGArguments );
      chunk.AppendString( DebugArguments );
      chunk.AppendString( WorkPath );
      chunk.AppendString( CartArguments );
      chunk.AppendString( TrueDriveOnArguments );
      chunk.AppendString( TrueDriveOffArguments );
      chunk.AppendU8( PassLabelsToEmulator ? (byte)0 : (byte)1 );

      return chunk;
    }

    public bool FromChunk( GR.IO.FileChunk Chunk )
    {
      if ( Chunk.Type != Types.FileChunk.SETTINGS_TOOL )
      {
        return false;
      }

      GR.IO.IReader reader = Chunk.MemoryReader();

      Type            = (ToolType)reader.ReadUInt32();
      Name            = reader.ReadString();
      Filename        = reader.ReadString();
      PRGArguments    = reader.ReadString();
      DebugArguments  = reader.ReadString();
      WorkPath        = reader.ReadString();
      CartArguments   = reader.ReadString();
      TrueDriveOnArguments = reader.ReadString();
      TrueDriveOffArguments = reader.ReadString();
      PassLabelsToEmulator = ( reader.ReadUInt8() == 0 );
      return true;
    }



    public override string ToString()
    {
      return Name;
    }



    internal LabelFileFormat LabelFormat()
    {
      string    filename = System.IO.Path.GetFileNameWithoutExtension( Filename ).ToUpper();
      if ( filename.StartsWith( "C64DEBUGGER" ) )
      {
        return LabelFileFormat.C64DEBUGGER;
      }
      return LabelFileFormat.VICE;
    }



    internal bool IsVICE()
    {
      // hackish way
      string    filename = System.IO.Path.GetFileNameWithoutExtension( Filename ).ToUpper();

      if ( ( filename.StartsWith( "X64" ) )
      ||   ( filename.StartsWith( "XVIC" ) )
      ||   ( filename.StartsWith( "X128" ) ) )
      {
        return true;
      }
      return false;
    }


  }
}
