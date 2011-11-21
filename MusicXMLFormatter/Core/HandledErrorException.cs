using System;

namespace MusicXMLFormatter.Core
{
  public class HandledErrorException : Exception
  {
    private readonly string _header;

    public HandledErrorException(string header, string message) : base(message)
    {
      _header = header;
    }

    public string Header
    {
      get { return this._header; }
    }
  }
}