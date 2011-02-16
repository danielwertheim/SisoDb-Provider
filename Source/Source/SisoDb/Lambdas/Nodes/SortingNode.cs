namespace SisoDb.Lambdas.Nodes
{
    public class SortingNode : INode
    {
        private static readonly NameStrategy NameStrategy = new NameStrategy();

        public string Name { get; private set; }

        public SortDirections Direction { get; private set; }

        public SortingNode(string name, SortDirections direction = SortDirections.Asc)
        {
            Name = NameStrategy.Apply(name.AssertNotNull("name"));
            Direction = direction;
        }

        public override string ToString()
        {
            return string.Format("{0} {1}", Name, Direction);
        }
    }
}