namespace WistCompiler;

using System.Runtime.InteropServices;

public static class WistObjectHandleExtensions
{
    public static IntPtr ToIntPtr(this object target) => GCHandle.Alloc(target).ToIntPtr();

    public static GCHandle ToGcHandle(this object target) => GCHandle.Alloc(target);

    public static IntPtr ToIntPtr(this GCHandle target) => GCHandle.ToIntPtr(target);
}