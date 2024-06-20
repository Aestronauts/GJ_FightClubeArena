#if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.
//------------------------------------------------------------------------------
// <auto-generated />
//
// This file was automatically generated by SWIG (https://www.swig.org).
// Version 4.1.1
//
// Do not make changes to this file unless you know what you are doing - modify
// the SWIG interface file instead.
//------------------------------------------------------------------------------


public class AkTriangle : global::System.IDisposable {
  private global::System.IntPtr swigCPtr;
  protected bool swigCMemOwn;

  internal AkTriangle(global::System.IntPtr cPtr, bool cMemoryOwn) {
    swigCMemOwn = cMemoryOwn;
    swigCPtr = cPtr;
  }

  internal static global::System.IntPtr getCPtr(AkTriangle obj) {
    return (obj == null) ? global::System.IntPtr.Zero : obj.swigCPtr;
  }

  internal virtual void setCPtr(global::System.IntPtr cPtr) {
    Dispose();
    swigCPtr = cPtr;
  }

  ~AkTriangle() {
    Dispose(false);
  }

  public void Dispose() {
    Dispose(true);
    global::System.GC.SuppressFinalize(this);
  }

  protected virtual void Dispose(bool disposing) {
    lock(this) {
      if (swigCPtr != global::System.IntPtr.Zero) {
        if (swigCMemOwn) {
          swigCMemOwn = false;
          AkSoundEnginePINVOKE.CSharp_delete_AkTriangle(swigCPtr);
        }
        swigCPtr = global::System.IntPtr.Zero;
      }
      global::System.GC.SuppressFinalize(this);
    }
  }

  public AkTriangle() : this(AkSoundEnginePINVOKE.CSharp_new_AkTriangle__SWIG_0(), true) {
  }

  public AkTriangle(ushort in_pt0, ushort in_pt1, ushort in_pt2, ushort in_surfaceInfo) : this(AkSoundEnginePINVOKE.CSharp_new_AkTriangle__SWIG_1(in_pt0, in_pt1, in_pt2, in_surfaceInfo), true) {
  }

  public ushort point0 { set { AkSoundEnginePINVOKE.CSharp_AkTriangle_point0_set(swigCPtr, value); }  get { return AkSoundEnginePINVOKE.CSharp_AkTriangle_point0_get(swigCPtr); } 
  }

  public ushort point1 { set { AkSoundEnginePINVOKE.CSharp_AkTriangle_point1_set(swigCPtr, value); }  get { return AkSoundEnginePINVOKE.CSharp_AkTriangle_point1_get(swigCPtr); } 
  }

  public ushort point2 { set { AkSoundEnginePINVOKE.CSharp_AkTriangle_point2_set(swigCPtr, value); }  get { return AkSoundEnginePINVOKE.CSharp_AkTriangle_point2_get(swigCPtr); } 
  }

  public ushort surface { set { AkSoundEnginePINVOKE.CSharp_AkTriangle_surface_set(swigCPtr, value); }  get { return AkSoundEnginePINVOKE.CSharp_AkTriangle_surface_get(swigCPtr); } 
  }

  public void Clear() { AkSoundEnginePINVOKE.CSharp_AkTriangle_Clear(swigCPtr); }

  public static int GetSizeOf() { return AkSoundEnginePINVOKE.CSharp_AkTriangle_GetSizeOf(); }

  public void Clone(AkTriangle other) { AkSoundEnginePINVOKE.CSharp_AkTriangle_Clone(swigCPtr, AkTriangle.getCPtr(other)); }

}
#endif // #if ! (UNITY_DASHBOARD_WIDGET || UNITY_WEBPLAYER || UNITY_WII || UNITY_WIIU || UNITY_NACL || UNITY_FLASH || UNITY_BLACKBERRY) // Disable under unsupported platforms.