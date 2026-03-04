namespace ClassLibrary
{
    public abstract class Hero
    {
        public string Name { get; set; }
        public abstract int GetPower();
        public abstract string GetDescription();
    }

    public class Warrior : Hero
    {
        public Warrior() { Name = "Warrior"; }
        public override int GetPower() => 10;
        public override string GetDescription() => Name;
    }

    public class Mage : Hero
    {
        public Mage() { Name = "Mage"; }
        public override int GetPower() => 8;
        public override string GetDescription() => Name;
    }

    public class Palladin : Hero
    {
        public Palladin() { Name = "Palladin"; }
        public override int GetPower() => 12;
        public override string GetDescription() => Name;
    }

    public abstract class InventoryDecorator : Hero
    {
        protected Hero _hero;
        public InventoryDecorator(Hero hero) { _hero = hero; }
    }

    public class Weapon : InventoryDecorator
    {
        public Weapon(Hero hero) : base(hero) { }
        public override int GetPower() => _hero.GetPower() + 15;
        public override string GetDescription() => _hero.GetDescription() + " + Sword";
    }

    public class Armor : InventoryDecorator
    {
        public Armor(Hero hero) : base(hero) { }
        public override int GetPower() => _hero.GetPower() + 5;
        public override string GetDescription() => _hero.GetDescription() + " + Steel Armor";
    }

    public class Artifact : InventoryDecorator
    {
        public Artifact(Hero hero) : base(hero) { }
        public override int GetPower() => _hero.GetPower() + 20;
        public override string GetDescription() => _hero.GetDescription() + " + Magic Ring";
    }
}