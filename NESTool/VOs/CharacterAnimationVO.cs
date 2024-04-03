namespace NESTool.VOs;

public class CharacterAnimationVO
{
    public CharacterAnimationVO(int index, string id, string name)
    {
        Index = index;
        ID = id;
        Name = name;
    }

    public int Index { get; set; }
    public string ID { get; set; }
    public string Name { get; set; }
}