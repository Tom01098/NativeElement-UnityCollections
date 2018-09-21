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
