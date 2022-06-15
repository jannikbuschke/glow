namespace Glow.Sample;

public record AttackModifier(int BaseAttack);
public record Protection(int BaseDamageReduction);


public record Item(string Name, string Icon, int Regeneration, AttackModifier AttackModifier, Protection Protection)
{
    public static Item New(string name, string icon, int regeneration)
    {
        return new(name, icon, regeneration, new AttackModifier(0), new Protection(0));
    }
}