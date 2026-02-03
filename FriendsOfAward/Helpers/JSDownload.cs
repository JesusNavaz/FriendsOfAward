using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

public static class JSDownload
{
    public static ValueTask SaveAsFile(this IJSRuntime js, string fileName, byte[] bytes)
    {
        return js.InvokeVoidAsync("saveAsFile", fileName, Convert.ToBase64String(bytes));
    }
}
