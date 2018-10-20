# NativeElement-UnityCollections
A NativeContainer for a single blittable element for use in the Unity Job System.

Access the element inside using the 'Element' property.

    [BurstCompile]
    private struct AddJob : IJob
    {
        [ReadOnly] public int a;
        [ReadOnly] public int b;
        [WriteOnly] public NativeElement<int> result;

        public void Execute()
        {
            result.Element = a + b;
        }
    }

## Which file do I use?
Use the [2018.3+ version](https://github.com/Tom01098/NativeElement-UnityCollections/blob/master/NativeElement-2018.3%2B) if you're running Unity version 2018.3 or greater. This is because this version has the unmanaged constraint, leading to proper static type checking and the use of generic pointers.

Use the [2018.2- version](https://github.com/Tom01098/NativeElement-UnityCollections/blob/master/NativeElement-2018.2-.cs) if you're running a lesser Unity version. This comes at the cost of an extra check on instantiation and a slightly slower read/write, but it should  be fast enough for any high-performance needs.
