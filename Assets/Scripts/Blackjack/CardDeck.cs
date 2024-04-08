using System;
using System.Collections.Generic;

public class CardDeck {
    private readonly Random random = new();
    private readonly List<Card> deck = new();

    public void CreateDeck() {
        deck.Clear();

        for (int suit = 0; suit < 4; suit++) {
            for (int i = 0; i < 13; i++)
                deck.Add(new Card((CardValues)(i + 1), (CardSuit)suit));
        }
    }

    // Draws random card from deck
    public Card PickCard() {
        var card = deck[random.Next(deck.Count)];
        deck.Remove(card);
        return card;
    }
}

[Serializable]
public struct Card {
    public CardValues value;
    public CardSuit suit;

    public Card(CardValues value, CardSuit suit) {
        this.value = value;
        this.suit = suit;
    }
}

public enum CardSuit {
    Spades,
    Hearts,
    Clubs,
    Diamonds,
    FaceDown
}

public enum CardValues {
    FaceDown = 0,
    Ace = 1,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    Jack = 11,
    Queen = 12,
    King = 13
}