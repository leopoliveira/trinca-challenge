using System;

using Domain.Extensions;

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

        public void AddItemsToShoppingList(bool isVeganPerson)
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

        public void RemoveItemsFromShoppingList(bool isVeganPerson)
        {
            if (isVeganPerson)
            {
                VegQtyInGrams = Math.Max(0, VegQtyInGrams - VEG_IN_GRAMS_PER_VEGAN_PERSON);
            }
            else
            {
                MeatQtyInGrams  = Math.Max(0, MeatQtyInGrams - MEAT_IN_GRAMS_PER_NON_VEGAN_PERSON);
                VegQtyInGrams = Math.Max(0, VegQtyInGrams - VEG_IN_GRAMS_PER_NON_VEGAN_PERSON);
            }
        }

        public BbqShoppingList GetShoppingList()
        {
            return this;
        }

        public object TakeSnapshot()
        {
            return new
            {
                Summary = $"Actual Churras Shopping List: Meat: {MeatQtyInGrams.ToKilogram()}Kg, Vegetables: {VegQtyInGrams.ToKilogram()}Kg.",
                Meat = MeatQtyInGrams.ToKilogram(),
                Veg = VegQtyInGrams.ToKilogram()
            };
        }
    }
}
