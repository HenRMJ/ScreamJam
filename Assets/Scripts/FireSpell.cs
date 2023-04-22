public class FireSpell : SpellAction
{
    public override void CastSpell(CardSlot cardSlot)
    {
        cardSlot.Card.GetComponent<CardData>().SetBloodCost(0);
        Destroy(cardSlot.Card.gameObject);

        cardSlot.Card = null;

        Destroy(gameObject);
    }
}
