namespace Mascari4615
{
	public interface ISavable<T>
	{
		void Load(T saveData);
		T Save();
	}
}