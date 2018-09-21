using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

/// <summary>
/// NativeElement is a wrapper for a single blittable element to be passed between managed and unmanaged code in jobs
/// </summary>
/// <typeparam name="T">The blittable type to be stored</typeparam>
[StructLayout(LayoutKind.Sequential)]
[NativeContainer]
[DebuggerDisplay("{Element}")]
public unsafe struct NativeElement<T> : IDisposable where T : struct
{
    // The pointer to the element
    [NativeDisableUnsafePtrRestriction]
    void* m_ptr;

#if ENABLE_UNITY_COLLECTIONS_CHECKS
    // Safety
    AtomicSafetyHandle m_Safety;

    [NativeSetClassTypeToNullOnSchedule]
    DisposeSentinel m_DisposeSentinel;
#endif

    // The allocator label
    Allocator m_AllocatorLabel;

    // Constructor
    public NativeElement(Allocator label)
    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        // Blittable check
        if (!UnsafeUtility.IsBlittable<T>())
            throw new ArgumentException(typeof(T) + " used in NativeElement must be blittable");
#endif

        // Label check
        if (label <= Allocator.None)
            throw new ArgumentException("NativeElement must be allocated using Job, TempJob or Persistent");

        // Label set
        m_AllocatorLabel = label;

        // Allocate memory for a single T
        m_ptr = UnsafeUtility.Malloc(UnsafeUtility.SizeOf<T>(), 1, label);

        // Create DisposeSentinel and AtomicSafetyHandle
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        DisposeSentinel.Create(out m_Safety, out m_DisposeSentinel, 0);
#endif

        // Create element to avoid unitialised data
        Element = new T();
    }

    // Property for the Element stored
    public T Element
    {
        get
        {
            // Read check
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckReadAndThrow(m_Safety);
#endif

            // Return the element
            return UnsafeUtility.ReadArrayElement<T>(m_ptr, 0);
        }
        set
        {
            // Write check
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            AtomicSafetyHandle.CheckWriteAndThrow(m_Safety);
#endif

            // Set the element
            UnsafeUtility.WriteArrayElement(m_ptr, 0, value);
        }
    }

    // Dispose of all resources
    public void Dispose()
    {
#if ENABLE_UNITY_COLLECTIONS_CHECKS
        DisposeSentinel.Dispose(m_Safety, ref m_DisposeSentinel);
#endif

        UnsafeUtility.Free(m_ptr, m_AllocatorLabel);
    }
}