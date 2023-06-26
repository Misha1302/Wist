namespace Backend;

using System.Runtime.InteropServices;

public class WistGcHandleProvider : IDisposable
{
    private GCHandle _handle;

    public WistGcHandleProvider(object target)
    {
        _handle = target.ToGcHandle();
    }

    public IntPtr Pointer => _handle.ToIntPtr();

    public void Dispose()
    {
        ReleaseUnmanagedResources();
        GC.SuppressFinalize(this);
    }

    private void ReleaseUnmanagedResources()
    {
        if (_handle.IsAllocated) _handle.Free();
    }

    ~WistGcHandleProvider()
    {
        ReleaseUnmanagedResources();
    }
}