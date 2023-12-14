namespace Domain.Entities
{
    public class BbqShoppingList
    {
        public const float MEAT_IN_GRAMS_PER_NON_VEGAN_PERSON = 300;
        public const float VEG_IN_GRAMS_PER_NON_VEGAN_PERSON = 300;
        public const float VEG_IN_GRAMS_PER_VEGAN_PERSON = 600;

        public BbqShoppingList()
        {
        }

        public BbqShoppingList(float meatQty, float vegQty)
        {
            MeatQtyInGrams = meatQty;
            VegQtyInGrams = vegQty;
        }

        public float MeatQtyInGrams { get; set; }
        public float VegQtyInGrams { get; set; }

        public void UpdateShoppingList(bool isVeganPerson)
        {
            if (isVeganPerson)
            {
                VegQtyInGrams += VEG_IN_GRAMS_PER_VEGAN_PERSON;
            }
            else
            {
                MeatQtyInGrams += MEAT_IN_GRAMS_PER_NON_VEGAN_PERSON;
                VegQtyInGrams += VEG_IN_GRAMS_PER_NON_VEGAN_PERSON;
            }
        }

        public BbqShoppingList GetShoppingList()
        {
            return this;
        }
    }
}
