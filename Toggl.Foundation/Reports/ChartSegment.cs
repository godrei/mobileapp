using System;
namespace Toggl.Foundation.Reports
{
    public struct ChartSegment
    {
        public float TrackedSeconds { get; }

        public float BillableSeconds { get; }
        
        public string Name { get; }

        public string Color { get; }
        
        public ChartSegment(string name, float trackedSeconds, float billableSeconds, string color)
        {
            Name = name;
            Color = color;
            TrackedSeconds = trackedSeconds;
            BillableSeconds = billableSeconds;
        }
    }
}
