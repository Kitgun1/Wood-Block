namespace WoodBlock
{
    public interface ICell
    {
        public bool TryInsert(IBlock block);
        public void Remove();
    }
}