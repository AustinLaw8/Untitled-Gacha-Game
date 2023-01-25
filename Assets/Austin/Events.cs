using UnityEngine;

public static class Events
{
    public static HealEvent HealEvent = new HealEvent();
    public static DamageEvent DamageEvent = new DamageEvent();
    public static DeathEvent DeathEvent = new DeathEvent();
}

public class HealEvent : GameEvent
{
    public int amount;
}

public class DamageEvent : GameEvent
{
    public int amount;
}

public class DeathEvent : GameEvent
{}