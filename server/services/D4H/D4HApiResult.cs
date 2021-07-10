using System;

namespace Kcsara.Respond.Services.D4H
{
  public class D4HApiResult<T>
  {
    public int StatusCode { get; set; }
    public T Data { get; set; }
  }
}
